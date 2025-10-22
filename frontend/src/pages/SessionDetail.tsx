import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { sessionsApi, matchesApi, usersApi, type SessionDetails, type User } from '../services/api';
import Modal from '../components/Modal';
import ConfirmDialog from '../components/ConfirmDialog';
import './SessionDetail.css';

function SessionDetail() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [session, setSession] = useState<SessionDetails | null>(null);
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(false);
  const [addUserError, setAddUserError] = useState<string | null>(null);
  const [customMatchError, setCustomMatchError] = useState<string | null>(null);
  const [showAddUser, setShowAddUser] = useState(false);
  const [showCustomMatch, setShowCustomMatch] = useState(false);
  const [confirmDialog, setConfirmDialog] = useState<{
    isOpen: boolean;
    title: string;
    message: string;
    onConfirm: () => void;
  }>({ isOpen: false, title: '', message: '', onConfirm: () => {} });
  const [selectedUserId, setSelectedUserId] = useState('');
  const [generateMatches, setGenerateMatches] = useState(true);
  const [customMatch, setCustomMatch] = useState({
    team1: [] as string[],
    team2: [] as string[],
  });

  useEffect(() => {
    if (id) {
      loadSession();
      loadUsers();
    }
  }, [id]);

  const loadSession = async () => {
    if (!id) return;
    try {
      setLoading(true);
      const response = await sessionsApi.getById(id);
      setSession(response.data);
    } catch (err: any) {
      console.error('Failed to load session:', err);
    } finally {
      setLoading(false);
    }
  };

  const loadUsers = async () => {
    try {
      const response = await usersApi.getAll();
      setUsers(response.data);
    } catch (err) {
      console.error('Failed to load users:', err);
    }
  };

  const handleUpdateScore = async (matchId: string, team1Score: number, team2Score: number) => {
    try {
      await matchesApi.updateScore(matchId, team1Score, team2Score);
      loadSession();
    } catch (err: any) {
      console.error('Failed to update score:', err);
    }
  };

  const handleDeleteMatch = (matchId: string) => {
    setConfirmDialog({
      isOpen: true,
      title: 'Delete Match',
      message: 'Are you sure you want to delete this match?',
      onConfirm: async () => {
        try {
          await matchesApi.delete(matchId);
          loadSession();
        } catch (err: any) {
          console.error('Failed to delete match:', err);
        }
      }
    });
  };

  const handleEndSession = () => {
    if (!id) return;
    setConfirmDialog({
      isOpen: true,
      title: 'End Session',
      message: 'Are you sure you want to end this session? This action cannot be undone.',
      onConfirm: async () => {
        try {
          await sessionsApi.end(id);
          navigate('/');
        } catch (err: any) {
          console.error('Failed to end session:', err);
        }
      }
    });
  };

  const handleAddUser = async () => {
    if (!id || !selectedUserId) return;
    try {
      await sessionsApi.addUser(id, selectedUserId, generateMatches);
      setShowAddUser(false);
      setSelectedUserId('');
      setAddUserError(null);
      loadSession();
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || err.message || 'Failed to add user';
      setAddUserError(errorMessage);
      console.error(err);
    }
  };

  const handleCreateCustomMatch = async () => {
    if (!id || customMatch.team1.length === 0 || customMatch.team2.length === 0) {
      setCustomMatchError('Both teams must have at least one player');
      return;
    }
    try {
      await matchesApi.createCustom(id, customMatch.team1, customMatch.team2);
      setShowCustomMatch(false);
      setCustomMatch({ team1: [], team2: [] });
      setCustomMatchError(null);
      loadSession();
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || err.message || 'Failed to create custom match';
      setCustomMatchError(errorMessage);
      console.error(err);
    }
  };

  const togglePlayerInTeam = (userId: string, team: 'team1' | 'team2') => {
    setCustomMatch(prev => ({
      ...prev,
      [team]: prev[team].includes(userId)
        ? prev[team].filter(id => id !== userId)
        : [...prev[team], userId]
    }));
  };

  const getAvailableUsers = () => {
    if (!session) return [];
    const sessionUserIds = session.users.map((u) => u.userId);
    return users.filter((u) => !sessionUserIds.includes(u.id));
  };

  if (loading) return <div className="loading">Loading...</div>;
  if (!session) return <div className="error-message">Session not found</div>;

  return (
    <div className="session-detail-page">
      <div className="page-header">
        <div>
          <h2>{session.name}</h2>
          <p className="session-info">
            Started: {new Date(session.startDate).toLocaleString()} | 
            Status: {session.status} | 
            Type: {session.matchType}
          </p>
        </div>
        <div className="button-group">
          {session.status === 'Active' && (
            <>
              <button
                onClick={() => setShowAddUser(!showAddUser)}
                className="btn btn-secondary btn-icon-mobile"
              >
                <span className="btn-icon">➕</span>
                <span className="btn-text">Add Player</span>
              </button>
              <button
                onClick={() => setShowCustomMatch(!showCustomMatch)}
                className="btn btn-secondary btn-icon-mobile"
              >
                <span className="btn-icon">⚽</span>
                <span className="btn-text">Custom Match</span>
              </button>
              <button onClick={handleEndSession} className="btn btn-danger btn-icon-mobile">
                <span className="btn-icon">🛑</span>
                <span className="btn-text">End Session</span>
              </button>
            </>
          )}
          <button onClick={() => navigate('/')} className="btn btn-secondary btn-icon-mobile">
            <span className="btn-icon">←</span>
            <span className="btn-text">Back</span>
          </button>
        </div>
      </div>

      <Modal 
        isOpen={showAddUser} 
        onClose={() => {
          setShowAddUser(false);
          setAddUserError(null);
        }}
        title="Add Player to Session"
        size="small"
      >
        <div className="modal-form">
          {addUserError && <div className="error-message">{addUserError}</div>}
          <div className="form-group">
            <label htmlFor="player-select">Select Player</label>
            <select
              id="player-select"
              value={selectedUserId}
              onChange={(e) => setSelectedUserId(e.target.value)}
              className="input"
            >
              <option value="">Choose a player...</option>
              {getAvailableUsers().map((user) => (
                <option key={user.id} value={user.id}>
                  {user.name}
                </option>
              ))}
            </select>
          </div>
          <div className="form-group">
            <label className="checkbox-label">
              <input
                type="checkbox"
                checked={generateMatches}
                onChange={(e) => setGenerateMatches(e.target.checked)}
              />
              <span>Generate missing matches with this player</span>
            </label>
          </div>
          <div className="modal-actions">
            <button 
              onClick={handleAddUser} 
              className="btn btn-primary"
              disabled={!selectedUserId}
            >
              Add Player
            </button>
            <button onClick={() => setShowAddUser(false)} className="btn btn-secondary">
              Cancel
            </button>
          </div>
        </div>
      </Modal>

      <Modal
        isOpen={showCustomMatch}
        onClose={() => {
          setShowCustomMatch(false);
          setCustomMatchError(null);
        }}
        title="Create Custom Match"
        size="medium"
      >
        <div className="modal-form">
          {customMatchError && <div className="error-message">{customMatchError}</div>}
          <p className="form-hint">Select players for each team. Teams don't need to be balanced.</p>
          <div className="team-selection">
            <div className="team-column">
              <h4>Team 1 ({customMatch.team1.length} {customMatch.team1.length === 1 ? 'player' : 'players'})</h4>
              <div className="player-checkboxes">
                {session.users.map((user) => (
                  <label 
                    key={user.userId} 
                    className={`player-checkbox ${customMatch.team2.includes(user.userId) ? 'disabled' : ''}`}
                  >
                    <input
                      type="checkbox"
                      checked={customMatch.team1.includes(user.userId)}
                      onChange={() => togglePlayerInTeam(user.userId, 'team1')}
                      disabled={customMatch.team2.includes(user.userId)}
                    />
                    <span>{user.userName}</span>
                  </label>
                ))}
              </div>
            </div>
            <div className="team-column">
              <h4>Team 2 ({customMatch.team2.length} {customMatch.team2.length === 1 ? 'player' : 'players'})</h4>
              <div className="player-checkboxes">
                {session.users.map((user) => (
                  <label 
                    key={user.userId} 
                    className={`player-checkbox ${customMatch.team1.includes(user.userId) ? 'disabled' : ''}`}
                  >
                    <input
                      type="checkbox"
                      checked={customMatch.team2.includes(user.userId)}
                      onChange={() => togglePlayerInTeam(user.userId, 'team2')}
                      disabled={customMatch.team1.includes(user.userId)}
                    />
                    <span>{user.userName}</span>
                  </label>
                ))}
              </div>
            </div>
          </div>
          <div className="modal-actions">
            <button 
              onClick={handleCreateCustomMatch} 
              className="btn btn-primary"
              disabled={customMatch.team1.length === 0 || customMatch.team2.length === 0}
            >
              Create Match
            </button>
            <button onClick={() => setShowCustomMatch(false)} className="btn btn-secondary">
              Cancel
            </button>
          </div>
        </div>
      </Modal>

      <div className="session-content">
        <div className="players-section">
          <h3>Players ({session.users.length})</h3>
          <div className="players-list">
            {session.users.map((user) => (
              <div key={user.userId} className="player-card">
                <span>{user.userName}</span>
                <small>Joined: {new Date(user.joinedAt).toLocaleDateString()}</small>
              </div>
            ))}
          </div>
        </div>

        <div className="matches-section">
          <h3>Matches ({session.matches.filter(m => m.isCompleted).length}/{session.matches.length})</h3>
          <div className="matches-list">
            {session.matches.length === 0 ? (
              <p className="empty-state">No matches generated yet</p>
            ) : (
              session.matches.map((match) => (
                <MatchCard
                  key={match.id}
                  match={match}
                  sessionStatus={session.status}
                  onUpdateScore={handleUpdateScore}
                  onDelete={handleDeleteMatch}
                />
              ))
            )}
          </div>
        </div>
      </div>

      <ConfirmDialog
        isOpen={confirmDialog.isOpen}
        onClose={() => setConfirmDialog({ ...confirmDialog, isOpen: false })}
        onConfirm={confirmDialog.onConfirm}
        title={confirmDialog.title}
        message={confirmDialog.message}
        confirmText="Yes"
        cancelText="No"
        danger={true}
      />
    </div>
  );
}

interface MatchCardProps {
  match: any;
  sessionStatus: 'Active' | 'Completed';
  onUpdateScore: (matchId: string, team1Score: number, team2Score: number) => void;
  onDelete: (matchId: string) => void;
}

function MatchCard({ match, sessionStatus, onUpdateScore, onDelete }: MatchCardProps) {
  const [showScoreModal, setShowScoreModal] = useState(false);
  const [team1Score, setTeam1Score] = useState(match.team1Score ?? 0);
  const [team2Score, setTeam2Score] = useState(match.team2Score ?? 0);
  
  const isSessionActive = sessionStatus === 'Active';

  const openScoreModal = () => {
    setTeam1Score(match.team1Score ?? 0);
    setTeam2Score(match.team2Score ?? 0);
    setShowScoreModal(true);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onUpdateScore(match.id, team1Score, team2Score);
    setShowScoreModal(false);
  };

  const adjustScore = (team: 'team1' | 'team2', delta: number) => {
    if (team === 'team1') {
      setTeam1Score((prev: number) => Math.max(0, prev + delta));
    } else {
      setTeam2Score((prev: number) => Math.max(0, prev + delta));
    }
  };

  const team1Names = match.team1Players.map((p: any) => p.userName).join(' & ');
  const team2Names = match.team2Players.map((p: any) => p.userName).join(' & ');

  return (
    <div className={`match-card ${match.isCompleted ? 'completed' : 'pending'}`}>
      <div className="match-header">
        <span className="match-badge">
          {match.isGenerated ? '🤖 Auto' : '✏️ Custom'}
        </span>
        {match.isCompleted && match.playedAt && (
          <span className="match-date">
            {new Date(match.playedAt).toLocaleDateString()}
          </span>
        )}
      </div>

      <div className="match-content">
        <div className="team team-1">
          <div className="team-label">Team 1</div>
          <div className="team-players">{team1Names}</div>
        </div>

        <div className="match-score-section">
          <div className="score-display">
            {match.isCompleted ? (
              <>
                <div className="score-value">
                  {match.team1Score} : {match.team2Score}
                </div>
                {isSessionActive && (
                  <button
                    onClick={openScoreModal}
                    className="btn btn-secondary btn-sm"
                  >
                    ✏️ Edit
                  </button>
                )}
              </>
            ) : (
              isSessionActive && (
                <button
                  onClick={openScoreModal}
                  className="btn btn-primary btn-sm"
                >
                  ➕ Add Score
                </button>
              )
            )}
            {isSessionActive && (
              <button
                onClick={() => onDelete(match.id)}
                className="btn btn-danger btn-sm"
                title="Delete match"
              >
                🗑️ Delete
              </button>
            )}
          </div>
        </div>

        <div className="team team-2">
          <div className="team-label">Team 2</div>
          <div className="team-players">{team2Names}</div>
        </div>
      </div>

      <Modal
        isOpen={showScoreModal}
        onClose={() => setShowScoreModal(false)}
        title={match.isCompleted ? "Edit Match Score" : "Add Match Score"}
        size="medium"
      >
        <form onSubmit={handleSubmit} className="score-modal-form">
          <div className="score-modal-teams">
            <div className="score-modal-team">
              <div className="team-name-header">{team1Names}</div>
              <div className="score-controls">
                <button
                  type="button"
                  onClick={() => adjustScore('team1', -1)}
                  className="btn-score-adjust"
                  disabled={team1Score === 0}
                >
                  −
                </button>
                <div className="score-display-large">{team1Score}</div>
                <button
                  type="button"
                  onClick={() => adjustScore('team1', 1)}
                  className="btn-score-adjust"
                >
                  +
                </button>
              </div>
            </div>

            <div className="score-separator-large">:</div>

            <div className="score-modal-team">
              <div className="team-name-header">{team2Names}</div>
              <div className="score-controls">
                <button
                  type="button"
                  onClick={() => adjustScore('team2', -1)}
                  className="btn-score-adjust"
                  disabled={team2Score === 0}
                >
                  −
                </button>
                <div className="score-display-large">{team2Score}</div>
                <button
                  type="button"
                  onClick={() => adjustScore('team2', 1)}
                  className="btn-score-adjust"
                >
                  +
                </button>
              </div>
            </div>
          </div>

          <div className="modal-actions">
            <button type="submit" className="btn btn-primary">
              💾 Save Score
            </button>
            <button
              type="button"
              onClick={() => setShowScoreModal(false)}
              className="btn btn-secondary"
            >
              Cancel
            </button>
          </div>
        </form>
      </Modal>
    </div>
  );
}

export default SessionDetail;
