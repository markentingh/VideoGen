import React, { useEffect, useState } from 'react';
import { useSession } from '@/context/session';
import { useModal } from '@/context/modal';
import { Services } from '@/api/admin/services';
import Button from '@/components/ui/button';
import ButtonIcon from '@/components/ui/button-icon';
import ButtonOutline from '@/components/ui/button-outline';
import Message from '@/components/ui/message';
import Spinner from '@/components/ui/spinner';
import Settings from './components/Settings';
import LLMs from './components/LLMs';
import TranscriptGeneration from './components/TranscriptGeneration';
import VideoLibrary from './components/VideoLibrary';
import VideoGeneration from './components/VideoGeneration';
import VideoTransitions from './components/VideoTransitions';
import MusicLibrary from './components/MusicLibrary';
import MusicGeneration from './components/MusicGeneration';
import ImageLibrary from './components/ImageLibrary';
import ImageGeneration from './components/ImageGeneration';
import TextToSpeech from './components/TextToSpeech';
import SoundFxLibrary from './components/SoundFxLibrary';
import SoundFxGeneration from './components/SoundFxGeneration';
import TextCaptions from './components/TextCaptions';

const menuItems = [
  { key: 'llms', label: 'LLMs', Component: LLMs },
  { key: 'transcript-generation', label: 'Transcript Generation', Component: TranscriptGeneration },
  { key: 'image-library', label: 'Image Library', Component: ImageLibrary },
  { key: 'image-generation', label: 'Image Generation', Component: ImageGeneration },
  { key: 'video-library', label: 'Video Library', Component: VideoLibrary },
  { key: 'video-generation', label: 'Video Generation', Component: VideoGeneration },
  { key: 'video-transitions', label: 'Video Transitions', Component: VideoTransitions },
  { key: 'music-library', label: 'Music Library', Component: MusicLibrary },
  { key: 'music-generation', label: 'Music Generation', Component: MusicGeneration },
  { key: 'sound-fx-library', label: 'Sound FX Library', Component: SoundFxLibrary },
  { key: 'sound-fx-generation', label: 'Sound FX Generation', Component: SoundFxGeneration },
  { key: 'text-to-speech', label: 'Text-To-Speech', Component: TextToSpeech },
  { key: 'text-captions', label: 'Text Captions', Component: TextCaptions },
];

export default function DashboardServices() {
  const session = useSession();
  const { showModal, closeModal } = useModal();
  const { getAvailableServices, getInstalled, install, uninstall } = Services(session);
  const [active, setActive] = useState('llms');
  const [services, setServices] = useState([]);
  const [installed, setInstalled] = useState({});
  const [installing, setInstalling] = useState(null);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState(null);

  const fetchInstalled = () => {
    getInstalled().then((response) => {
      if (response.data.success) {
        setInstalled(Object.fromEntries((response.data.data || []).map((service) => [service.key, service])));
      }
    }).catch(() => {});
  };

  const fetchServices = () => {
    setLoading(true);
    getAvailableServices().then((response) => {
      setLoading(false);
      if (response.data.success) {
        setServices(response.data.data || []);
      } else {
        setMessage({ type: 'error', text: response.data.message || 'Failed to fetch services' });
      }
    }).catch((error) => {
      setLoading(false);
      setMessage({ type: 'error', text: error?.response?.data?.message || 'Failed to fetch services' });
    });
  };

  const handleInstall = (service) => {
    setInstalling(service.key);
    setMessage(null);
    install(service).then((response) => {
      setInstalling(null);
      if (response.data.success) {
        fetchInstalled();
        showRestartingModal(service.name, 'installing');
      } else {
        setMessage({ type: 'error', text: response.data.message || `Failed to install ${service.name}` });
      }
    }).catch((error) => {
      setInstalling(null);
      setMessage({ type: 'error', text: error?.response?.data?.message || `Failed to install ${service.name}` });
    });
  };

  const showRestartingModal = (serviceName, verb) => {
    showModal({
      title: 'Server Restarting',
      icon: 'info',
      message: `The API server is restarting to finish ${verb} the ${serviceName} service and will be back online within the next 60 seconds.`,
      buttons: <Button onClick={closeModal}>OK</Button>
    });
  };

  const handleUninstall = (service) => {
    showModal({
      title: `Uninstall ${service.name}`,
      icon: 'warning',
      message: 'Do you really want to uninstall this service? Warning: Any database records associated with this service may be permanently deleted.',
      buttons: (
        <>
          <ButtonOutline
            color="green"
            onClick={() => {
              closeModal();
              uninstall(service.key).then((response) => {
                if (response.data.success) {
                  fetchInstalled();
                  showRestartingModal(service.name, 'uninstalling');
                } else {
                  setMessage({ type: 'error', text: response.data.message || `Failed to uninstall ${service.name}` });
                }
              }).catch((error) => {
                setMessage({ type: 'error', text: error?.response?.data?.message || `Failed to uninstall ${service.name}` });
              });
            }}
          >
            Yes
          </ButtonOutline>
          <ButtonOutline color="gray" onClick={closeModal}>
            Cancel
          </ButtonOutline>
        </>
      )
    });
  };

  useEffect(() => {
    fetchServices();
    fetchInstalled();
  }, []);

  const activeItem = menuItems.find((item) => item.key === active);
  const activeServices = activeItem ? services.filter((service) => service.type === activeItem.key) : [];

  return (
    <div className="flex gap-6 items-start">
      <aside className="w-60 shrink-0 rounded-xl bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 shadow">
        <div className="flex items-center justify-between p-4 border-b border-gray-200 dark:border-gray-700">
          <h2 className="text-lg font-bold">Services</h2>
          <ButtonIcon
            name="settings"
            title="Settings"
            color={active === 'settings' ? 'blue' : 'gray'}
            onClick={() => setActive('settings')}
          />
        </div>
        <nav className="p-2">
          <ul className="space-y-1">
            {menuItems.map((item) => (
              <li key={item.key}>
                <button
                  onClick={() => setActive(item.key)}
                  className={`w-full text-left px-3 py-2 rounded transition ${
                    active === item.key
                      ? 'bg-primary-100 text-primary-700 dark:bg-primary-900 dark:text-primary-300'
                      : 'hover:bg-gray-100 dark:hover:bg-gray-700'
                  }`}
                >
                  {item.label}
                </button>
              </li>
            ))}
          </ul>
        </nav>
      </aside>
      <div className="flex-1 min-w-0">
        {message && (
          <Message type={message.type} onClose={() => setMessage(null)}>
            {message.text}
          </Message>
        )}
        {loading ? (
          <div className="flex items-center justify-center gap-3 py-16 text-gray-500 dark:text-gray-400">
            <Spinner className="text-2xl" />
            <span>Finding available services...</span>
          </div>
        ) : active === 'settings'
          ? <Settings onSaved={fetchServices} />
          : activeItem && <activeItem.Component services={activeServices} installed={installed} installing={installing} onInstall={handleInstall} onUninstall={handleUninstall} />}
      </div>
    </div>
  );
}
