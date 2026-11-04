import React, { useEffect, useState } from 'react';
import { useSession } from '@/context/session';
import { useModal } from '@/context/modal';
import { OpenAI } from './api';
import platforms from './helpers/platforms';
import Select from '@/components/forms/select';
import Input from '@/components/forms/input';
import Button from '@/components/ui/button';
import ButtonIcon from '@/components/ui/button-icon';
import Message from '@/components/ui/message';

const CUSTOM = '__custom';

export default function ServiceConfig() {
  const session = useSession();
  const { showModal, closeModal } = useModal();
  const { getPlatforms, savePlatform, deletePlatform } = OpenAI(session);

  const [platform, setPlatform] = useState(platforms[0]?.name || '');
  const [apiKey, setApiKey] = useState('');
  const [custom, setCustom] = useState({ endpoint: '', model: '', name: '', apiKey: '' });
  const [added, setAdded] = useState([]);
  const [message, setMessage] = useState(null);

  const isCustom = platform === CUSTOM;
  const selectedPlatform = platforms.find((p) => p.name === platform);

  const fetchPlatforms = () => {
    getPlatforms().then((response) => {
      if (response.data.success) {
        setAdded(response.data.data || []);
      }
    }).catch(() => {});
  };

  useEffect(() => {
    fetchPlatforms();
  }, []);

  const requireApiKey = (name, key) => {
    if (key.trim()) return false;
    showModal({
      title: 'API Key Required',
      icon: 'error',
      message: `Please specify your API Key for ${name} before continuing.`,
      buttons: <Button onClick={closeModal}>OK</Button>
    });
    return true;
  };

  const handleAdd = () => {
    setMessage(null);
    let record;
    if (isCustom) {
      const name = custom.endpoint ? new URL(custom.endpoint).hostname : 'Custom';
      if (requireApiKey(name, custom.apiKey)) return;
      record = { Name: name, Endpoint: custom.endpoint, Model: custom.model, ModelName: custom.name, ApiKey: custom.apiKey };
    } else {
      if (requireApiKey(platform, apiKey)) return;
      record = { Name: platform, Endpoint: selectedPlatform?.endpoint || '', ApiKey: apiKey };
    }
    savePlatform(record).then((response) => {
      if (response.data.success) {
        setApiKey('');
        setCustom({ endpoint: '', model: '', name: '', apiKey: '' });
        fetchPlatforms();
      } else {
        setMessage({ type: 'error', text: response.data.message || 'Failed to add platform' });
      }
    }).catch((error) => {
      setMessage({ type: 'error', text: error?.response?.data?.message || 'Failed to add platform' });
    });
  };

  const handleDelete = (id) => {
    deletePlatform(id).then((response) => {
      if (response.data.success) {
        fetchPlatforms();
      } else {
        setMessage({ type: 'error', text: response.data.message || 'Failed to delete platform' });
      }
    }).catch((error) => {
      setMessage({ type: 'error', text: error?.response?.data?.message || 'Failed to delete platform' });
    });
  };

  const platformOptions = [
    ...platforms.map((p) => ({ value: p.name, label: p.name })),
    { value: CUSTOM, label: 'Custom...' }
  ];

  return (
    <div>
      {message && (
        <Message type={message.type} onClose={() => setMessage(null)}>
          {message.text}
        </Message>
      )}
      <div className="flex items-end gap-3">
        <Select
          name="platform"
          label="Platform"
          options={platformOptions}
          value={platform}
          onChange={(e) => setPlatform(e.target.value)}
          className="flex-1"
        />
        {!isCustom && (
          <>
            <Input
              name="apiKey"
              label="API Key"
              type="password"
              value={apiKey}
              onInput={(e) => setApiKey(e.target.value)}
              className="flex-1"
            />
            <Button className="mb-4 shrink-0" onClick={handleAdd}>
              + Add Platform
            </Button>
          </>
        )}
      </div>
      {isCustom && (
        <>
          <div className="flex gap-3">
            <Input
              name="endpoint"
              label="API Endpoint"
              value={custom.endpoint}
              onInput={(e) => setCustom((prev) => ({ ...prev, endpoint: e.target.value }))}
              placeholder="https://api.example.com/v1"
              className="flex-[2]"
            />
            <Input
              name="customModel"
              label="Model Key"
              value={custom.model}
              onInput={(e) => setCustom((prev) => ({ ...prev, model: e.target.value }))}
              placeholder="gpt-6-astra"
              className="flex-1"
            />
            <Input
              name="customModelName"
              label="Model Name"
              value={custom.name}
              onInput={(e) => setCustom((prev) => ({ ...prev, name: e.target.value }))}
              className="flex-1"
            />
            <Input
              name="customApiKey"
              label="API Key"
              type="password"
              value={custom.apiKey}
              onInput={(e) => setCustom((prev) => ({ ...prev, apiKey: e.target.value }))}
              className="flex-[2]"
            />
          </div>
          <Button onClick={handleAdd}>+ Add Platform</Button>
        </>
      )}
      {added.length > 0 && (
        <ul className="mt-6 space-y-2">
          {added.map((item) => (
            <li
              key={item.id}
              className="flex items-center gap-3 p-3 rounded bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700"
            >
              <div className="min-w-0 flex-1">
                <p className="font-medium">{item.name}</p>
                <p className="text-sm text-gray-500 dark:text-gray-400 truncate">
                  {item.modelName || item.model}{item.endpoint ? ` — ${item.endpoint}` : ''}
                </p>
              </div>
              <ButtonIcon name="delete" color="red" title="Delete platform" onClick={() => handleDelete(item.id)} />
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
