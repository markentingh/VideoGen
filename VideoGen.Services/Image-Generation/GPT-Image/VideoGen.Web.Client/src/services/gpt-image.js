import { lazy } from 'react';

export default {
  key: "gpt-image",
  name: "GPT Image",
  description: "Generate images via the OpenAI GPT Image 2.0 API.",
  icon: "gpt-image.png",
  serviceConfig: lazy(() => import('./gpt-image/serviceConfig')),
  projectDetails: lazy(() => import('./gpt-image/projectDetails'))
}
