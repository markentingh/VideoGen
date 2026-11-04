import { lazy } from 'react';

export default {
  key: "openai",
  name: "OpenAI",
  description: "Connect to any LLM that supports the OpenAI API model.",
  icon: "openai.png",
  serviceConfig: lazy(() => import('./openai/serviceConfig')),
  projectDetails: lazy(() => import('./openai/projectDetails'))
}
