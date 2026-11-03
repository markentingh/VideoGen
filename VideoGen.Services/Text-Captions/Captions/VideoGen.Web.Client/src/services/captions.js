import { lazy } from 'react';

export default {
  key: "captions",
  name: "Captions",
  description: "Basic timed text captions for generated videos.",
  icon: "captions.png",
  serviceConfig: lazy(() => import('./captions/serviceConfig')),
  projectDetails: lazy(() => import('./captions/projectDetails'))
}
