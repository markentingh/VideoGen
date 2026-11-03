import React, { useEffect, useState } from 'react';
import { useSession } from '@/context/session';
import { Services } from '@/api/admin/services';
import Input from '@/components/forms/input';
import ButtonOutline from '@/components/ui/button-outline';
import Message from '@/components/ui/message';

const DEFAULT_URL = 'https://localhost:7717/services.json';

export default function Settings({ onSaved }) {
  const session = useSession();
  const { getUrls, saveUrls } = Services(session);
  const [urls, setUrls] = useState(DEFAULT_URL);
  const [savedUrls, setSavedUrls] = useState(DEFAULT_URL);
  const [message, setMessage] = useState(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    getUrls().then((response) => {
      setLoading(false);
      if (response.data.success) {
        const value = response.data.data?.urls || DEFAULT_URL;
        setUrls(value);
        setSavedUrls(value);
      } else {
        setMessage({ type: 'error', text: response.data.message || 'Failed to load settings' });
      }
    }).catch((error) => {
      setLoading(false);
      setMessage({ type: 'error', text: error?.response?.data?.message || 'Failed to load settings' });
    });
  }, []);

  const dirty = urls !== savedUrls;

  const handleSave = () => {
    setSaving(true);
    saveUrls(urls).then((response) => {
      setSaving(false);
      if (response.data.success) {
        setSavedUrls(urls);
        setMessage({ type: 'info', text: response.data.message || 'Settings saved' });
        if (onSaved) onSaved();
      } else {
        setMessage({ type: 'error', text: response.data.message || 'Failed to save settings' });
      }
    }).catch((error) => {
      setSaving(false);
      setMessage({ type: 'error', text: error?.response?.data?.message || 'Failed to save settings' });
    });
  };

  return (
    <div>
      <h2 className="text-2xl font-bold mb-4">Settings</h2>
      {message && (
        <Message type={message.type} onClose={() => setMessage(null)}>
          {message.text}
        </Message>
      )}
      <div className="max-w-lg">
        <Input
          name="availableServicesUrl"
          label="Available Services URL"
          value={urls}
          onInput={(e) => setUrls(e.target.value)}
          disabled={loading || saving}
          note="Separate multiple URLs with a semicolon (;)"
        />
        {dirty && (
          <ButtonOutline onClick={handleSave} disabled={saving}>
            Save Changes
          </ButtonOutline>
        )}
      </div>
    </div>
  );
}
