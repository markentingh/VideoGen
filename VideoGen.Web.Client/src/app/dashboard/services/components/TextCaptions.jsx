import React from 'react';
import ServiceList from './ServiceList';

export default function TextCaptions({ services = [], ...props }) {
  return (
    <div>
      <h2 className="text-2xl font-bold mb-4">Text Captions</h2>
      <ServiceList services={services} {...props} />
    </div>
  );
}
