import { Api } from '@/api/Api';

const OpenAI = (args) => Api({ ...args }).endpoints(({ api }) => {
  const apiPath = '/api/services/openai';
  return {
    getPlatforms: () => api.get(`${apiPath}/get-platforms`),
    savePlatform: (platform) => api.post(`${apiPath}/save-platform`, platform),
    deletePlatform: (id) => api.post(`${apiPath}/delete-platform`, { Id: id }),
  };
});

export { OpenAI };
