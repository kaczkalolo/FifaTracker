import { useEffect, useState } from 'react';
import { usersApi, type LeaderboardEntry } from '../services/api';
import './Leaderboard.css';

function Leaderboard() {
  const [leaderboard, setLeaderboard] = useState<LeaderboardEntry[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>('');
  const [activeTab, setActiveTab] = useState<'standard' | 'simple'>('standard');

  useEffect(() => {
    loadLeaderboard();
  }, []);

  const loadLeaderboard = async () => {
    try {
      setLoading(true);
      const response = await usersApi.getLeaderboard();
      setLeaderboard(response.data);
      setError('');
    } catch (err) {
      setError('Failed to load leaderboard');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const calculateSimpleScore = (entry: LeaderboardEntry) => {
    const earnedPoints = entry.wins * 3 + entry.draws;
    const maxPoints = entry.totalMatches * 3;
    return maxPoints > 0 ? earnedPoints / maxPoints : 0;
  };

  const getDisplayedLeaderboard = () => {
    if (activeTab === 'standard') {
      return leaderboard;
    }
    // Simple: sortuj według Si = (3W + D) / (3M)
    return [...leaderboard].sort((a, b) => {
      const scoreA = calculateSimpleScore(a);
      const scoreB = calculateSimpleScore(b);
      if (scoreB !== scoreA) return scoreB - scoreA;
      // Jeśli skuteczność równa, sortuj po goal difference
      if (b.goalDifference !== a.goalDifference) return b.goalDifference - a.goalDifference;
      // Jeśli goal difference równe, sortuj po goals scored
      return b.goalsScored - a.goalsScored;
    });
  };

  const displayedLeaderboard = getDisplayedLeaderboard();

  if (loading) {
    return <div className="leaderboard-page">Loading...</div>;
  }

  return (
    <div className="leaderboard-page">
      <h1 className="black">🏆 Leaderboard</h1>

      {error && <div className="error-message">{error}</div>}

      {leaderboard.length === 0 ? (
        <p className="no-data">No matches played yet. Start a session to see statistics!</p>
      ) : (
        <>
          <div className="tabs">
            <button
              className={`tab ${activeTab === 'standard' ? 'active' : ''}`}
              onClick={() => setActiveTab('standard')}
            >
              Standard Scoring
            </button>
            <button
              className={`tab ${activeTab === 'simple' ? 'active' : ''}`}
              onClick={() => setActiveTab('simple')}
            >
              Effectiveness Scoring
            </button>
          </div>

          <div className="leaderboard-table-container">
          <table className="leaderboard-table">
            <thead>
              <tr>
                <th className="rank-col">Rank</th>
                <th className="player-col">Player</th>
                <th>{activeTab === 'standard' ? 'Points' : 'Score'}</th>
                {activeTab === 'standard' && <th>Played</th>}
                <th className="highlight-col">W</th>
                <th>D</th>
                <th className="highlight-col">L</th>
                {activeTab === 'standard' && <th>Win %</th>}
                <th>GS</th>
                <th>GC</th>
                <th className="highlight-col">GD</th>
              </tr>
            </thead>
            <tbody>
              {displayedLeaderboard.map((entry, index) => {
                const displayValue = activeTab === 'standard' 
                  ? entry.points 
                  : (calculateSimpleScore(entry) * 100);
                const displayText = activeTab === 'standard' 
                  ? displayValue.toString() 
                  : `${displayValue.toFixed(1)}%`;
                return (
                  <tr key={entry.userId} className={index < 3 ? `top-${index + 1}` : ''}>
                    <td className="rank-col">
                      {index === 0 && '🥇'}
                      {index === 1 && '🥈'}
                      {index === 2 && '🥉'}
                      {index > 2 && index + 1}
                    </td>
                    <td className="player-col">{entry.userName}</td>
                    <td className="points-col"><strong>{displayText}</strong></td>
                    {activeTab === 'standard' && <td className="highlight-col black">{entry.totalMatches}</td>}
                    <td className="highlight-col wins">{entry.wins}</td>
                    <td className="highlight-col draws">{entry.draws}</td>
                    <td className="highlight-col losses">{entry.losses}</td>
                    {activeTab === 'standard' && <td className="highlight-col black">{entry.winRate.toFixed(1)}%</td>}
                    <td className="highlight-col black">{entry.goalsScored}</td>
                    <td className="highlight-col black">{entry.goalsConceded}</td>
                    <td className={`highlight-col ${entry.goalDifference > 0 ? 'positive' : entry.goalDifference < 0 ? 'negative' : 'black'}`}>
                      {entry.goalDifference > 0 ? '+' : ''}{entry.goalDifference}
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>

        <div className="legend">
          <h3>Scoring System</h3>
          {activeTab === 'standard' ? (
            <ul>
              <li><strong>Formula:</strong> P<sub>i</sub> = 3W<sub>i</sub> + 1D<sub>i</sub> + 0L<sub>i</sub></li>
              <li><strong>Effectiveness:</strong> S<sub>i</sub> = P<sub>i</sub> / P<sub>max,i</sub> = (3W<sub>i</sub> + D<sub>i</sub>) / (3M<sub>i</sub>)</li>
              <li><strong>Sorting:</strong> By effectiveness (S<sub>i</sub>), then by goal difference</li>
              <li><strong>W/D/L:</strong> Wins / Draws / Losses</li>
              <li><strong>GS/GC:</strong> Goals Scored / Goals Conceded</li>
              <li><strong>GD:</strong> Goal Difference (GS - GC)</li>
            </ul>
          ) : (
            <ul>
              <li><strong>Formula:</strong> S<sub>i</sub> = (3W<sub>i</sub> + D<sub>i</sub>) / (3M<sub>i</sub>)</li>
              <li><strong>Where:</strong> M<sub>i</sub> = W<sub>i</sub> + D<sub>i</sub> + L<sub>i</sub> (total matches)</li>
              <li><strong>Sorting:</strong> By effectiveness (S<sub>i</sub>), then by goal difference, then by goals scored</li>
              <li><strong>Note:</strong> Same formula as Standard, but shows percentage directly</li>
              <li><strong>W/D/L:</strong> Wins / Draws / Losses</li>
              <li><strong>GS/GC:</strong> Goals Scored / Goals Conceded</li>
              <li><strong>GD:</strong> Goal Difference (GS - GC)</li>
            </ul>
          )}
        </div>
        </>
      )}
    </div>
  );
}

export default Leaderboard;
