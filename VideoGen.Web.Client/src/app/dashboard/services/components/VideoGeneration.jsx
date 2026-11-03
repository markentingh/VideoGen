import React from 'react';
import ServiceList from './ServiceList';

export default function VideoGeneration({ services = [], ...props }) {
  return (
    <div>
      <h2 className="text-2xl font-bold mb-4">Video Generation</h2>
      <ServiceList services={services} {...props} />
    </div>
  );
}
