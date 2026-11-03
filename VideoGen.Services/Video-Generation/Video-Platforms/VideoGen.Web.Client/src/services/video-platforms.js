import { lazy } from 'react';

export default {
  key: "video-platforms",
  name: "Video Platforms",
  description: "Configure API credentials for popular AI video generation platforms.",
  icon: "video-platforms.png",
  serviceConfig: lazy(() => import('./video-platforms/serviceConfig')),
  projectDetails: lazy(() => import('./video-platforms/projectDetails'))
}
