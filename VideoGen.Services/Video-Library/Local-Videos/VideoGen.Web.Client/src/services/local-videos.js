import { lazy } from 'react';

export default {
  key: "local-videos",
  name: "Local Videos",
  description: "Use local file system folders as a video asset library for generated videos.",
  icon: "local-videos.png",
  serviceConfig: lazy(() => import('./local-videos/serviceConfig')),
  projectDetails: lazy(() => import('./local-videos/projectDetails'))
}
