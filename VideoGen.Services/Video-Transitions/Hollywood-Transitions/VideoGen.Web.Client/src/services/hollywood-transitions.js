import { lazy } from 'react';

export default {
  key: "hollywood-transitions",
  name: "Hollywood Transitions",
  description: "Stitch project videos together with cinematic transitions.",
  icon: "hollywood-transitions.png",
  serviceConfig: lazy(() => import('./hollywood-transitions/serviceConfig')),
  projectDetails: lazy(() => import('./hollywood-transitions/projectDetails'))
}
