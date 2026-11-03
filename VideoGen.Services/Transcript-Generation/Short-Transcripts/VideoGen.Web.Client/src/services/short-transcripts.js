import { lazy } from 'react';

export default {
  key: "short-transcripts",
  name: "Short Transcripts",
  description: "Generates video transcripts for TikTok, Instagram Reels, and YouTube Shorts.",
  icon: "short-transcripts.png",
  serviceConfig: lazy(() => import('./short-transcripts/serviceConfig')),
  projectDetails: lazy(() => import('./short-transcripts/projectDetails'))
}
