import React from 'react';
import ServiceList from './ServiceList';

export default function ImageLibrary({ services = [], ...props }) {
  return (
    <div>
      <h2 className="text-2xl font-bold mb-4">Image Library</h2>
      <ServiceList services={services} {...props} />
    </div>
  );
}
