import { lazy } from 'react';

export default {
  key: "kokoro",
  name: "Kokoro",
  description: "English text-to-speech via the Kokoro open-weight model.",
  icon: "kokoro.png",
  serviceConfig: lazy(() => import('./kokoro/serviceConfig')),
  projectDetails: lazy(() => import('./kokoro/projectDetails'))
}
