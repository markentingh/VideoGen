import { Api } from '@/api/Api';

const Services = (args) => Api({ ...args }).endpoints(({ api }) => {
  const apiPath = '/api/admin/services';
  return {
    getAvailableServices: () => api.get(`${apiPath}/get-available-services`),
    getInstalled: () => api.get(`${apiPath}/get-installed`),
    install: (service) => api.post(`${apiPath}/install`, service),
    getUrls: () => api.get(`${apiPath}/get-urls`),
    saveUrls: (urls) => api.post(`${apiPath}/save-urls`, { Urls: urls }),
  };
});

export { Services };
