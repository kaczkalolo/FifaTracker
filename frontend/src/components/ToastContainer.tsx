import { useState, useCallback, useEffect } from 'react';
import Toast from './Toast';

interface ToastData {
  id: number;
  message: string;
  type: 'error' | 'success' | 'info';
}

let toastId = 0;
let addToastFn: ((message: string, type: 'error' | 'success' | 'info') => void) | null = null;

export const showToast = (message: string, type: 'error' | 'success' | 'info' = 'error') => {
  if (addToastFn) {
    addToastFn(message, type);
  }
};

function ToastContainer() {
  const [toasts, setToasts] = useState<ToastData[]>([]);

  const addToast = useCallback((message: string, type: 'error' | 'success' | 'info') => {
    const id = toastId++;
    setToasts((prev) => [...prev, { id, message, type }]);
  }, []);

  const removeToast = useCallback((id: number) => {
    setToasts((prev) => prev.filter((toast) => toast.id !== id));
  }, []);

  useEffect(() => {
    addToastFn = addToast;
    return () => {
      addToastFn = null;
    };
  }, [addToast]);

  return (
    <>
      {toasts.map((toast) => (
        <Toast
          key={toast.id}
          message={toast.message}
          type={toast.type}
          onClose={() => removeToast(toast.id)}
        />
      ))}
    </>
  );
}

export default ToastContainer;
