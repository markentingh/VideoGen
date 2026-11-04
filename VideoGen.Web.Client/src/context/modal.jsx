import React, { createContext, useContext, useState } from 'react';
import Modal from '@/components/ui/modal';
import Icon from '@/components/ui/icon';

const ModalContext = createContext(null);

const iconColors = {
  error: 'text-red-500',
  info: 'text-blue-500',
  warning: 'text-yellow-500'
};

export function ModalProvider({ children }) {
  const [modal, setModal] = useState(null);
  const showModal = (options) => setModal(options);
  const closeModal = () => {
    if (modal?.onClose) modal.onClose();
    setModal(null);
  };

  return (
    <ModalContext.Provider value={{ showModal, closeModal }}>
      {children}
      {modal && (
        <Modal title={modal.title} onClose={closeModal}>
          {modal.icon ? (
            <div className="flex items-start gap-3">
              <Icon name={modal.icon} className={`flex-shrink-0 text-2xl ${iconColors[modal.icon] || iconColors.info}`} />
              <div className="flex-1">{modal.message || modal.content}</div>
            </div>
          ) : (
            modal.content || modal.message
          )}
          {modal.buttons && <div className="buttons flex gap-3 justify-end mt-6">{modal.buttons}</div>}
        </Modal>
      )}
    </ModalContext.Provider>
  );
}

export function useModal() {
  const context = useContext(ModalContext);
  if (!context) {
    throw new Error('useModal must be used within a ModalProvider');
  }
  return context;
}
