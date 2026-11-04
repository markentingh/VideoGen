import React from 'react';
import ServiceList from './ServiceList';

export default function LLMs({ services = [], ...props }) {
  return (
    <div>
      <h2 className="text-2xl font-bold mb-4">LLMs</h2>
      <ServiceList services={services} {...props} />
    </div>
  );
}
