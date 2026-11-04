import { lazy } from 'react';

export default {
  key: "ollama",
  name: "Ollama",
  description: "Connect to an Ollama server and select supported LLM models to download and use.",
  icon: "ollama.png",
  serviceConfig: lazy(() => import('./ollama/serviceConfig')),
  projectDetails: lazy(() => import('./ollama/projectDetails'))
}
