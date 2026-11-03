import React, { useState, Suspense } from 'react';
import ButtonOutline from '@/components/ui/button-outline';
import Icon from '@/components/ui/icon';
import Spinner from '@/components/ui/spinner';
import { serviceModules } from '@/helpers/services';

export default function ServiceList({ services = [], installed = {}, installing, onInstall }) {
  const [expanded, setExpanded] = useState(null);

  if (services.length === 0) {
    return <p className="text-gray-500 dark:text-gray-400">No services available.</p>;
  }

  const sorted = [...services].sort((a, b) => (installed[b.key] ? 1 : 0) - (installed[a.key] ? 1 : 0));
  return (
    <ul className="space-y-3">
      {sorted.map((service) => {
        const installedService = installed[service.key];
        const serviceModule = serviceModules[service.key];
        const ServiceConfig = serviceModule?.serviceConfig;
        const canExpand = !!installedService && !!ServiceConfig;
        const isExpanded = expanded === service.key;
        const icon = installedService?.icon ? `/services/${installedService.icon}` : serviceModule?.icon ? `/services/${serviceModule.icon}` : service.iconurl;
        return (
          <li
            key={service.key || service.gitrepo}
            className="rounded bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700"
          >
            <div
              className={`flex items-center gap-3 p-3 ${canExpand ? 'cursor-pointer select-none' : ''}`}
              onClick={() => {
                if (canExpand) setExpanded(isExpanded ? null : service.key);
              }}
            >
              {icon && <img src={icon} alt="" className="w-8 h-8 rounded" />}
              <div className="min-w-0 flex-1">
                <p className="font-medium">{service.name}</p>
                <p className="text-sm text-gray-500 dark:text-gray-400 truncate">{service.summary}</p>
              </div>
              <span className="text-sm text-gray-500 dark:text-gray-400 shrink-0">
                v{installedService?.version || service.version}
              </span>
              {installedService ? (
                <span className="shrink-0 inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400">
                  Installed
                </span>
              ) : (
                <ButtonOutline
                  size="small"
                  onClick={() => onInstall(service)}
                  disabled={installing === service.key}
                  className="shrink-0"
                >
                  {installing === service.key ? 'Installing...' : 'Install'}
                </ButtonOutline>
              )}
              {canExpand && (
                <Icon
                  name={isExpanded ? 'expand_less' : 'expand_more'}
                  className="text-gray-400 dark:text-gray-500 shrink-0"
                />
              )}
            </div>
            {isExpanded && ServiceConfig && (
              <div className="border-t border-gray-200 dark:border-gray-700 p-4">
                <Suspense fallback={<div className="flex justify-center py-4"><Spinner className="text-2xl" /></div>}>
                  <ServiceConfig />
                </Suspense>
              </div>
            )}
          </li>
        );
      })}
    </ul>
  );
}
