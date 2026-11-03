import React from 'react';
import ServiceList from './ServiceList';

export default function TextToSpeech({ services = [], ...props }) {
  return (
    <div>
      <h2 className="text-2xl font-bold mb-4">Text-To-Speech</h2>
      <ServiceList services={services} {...props} />
    </div>
  );
}
