# Video Platforms

**Type:** `video-generation`

Central hub for connecting popular AI video generation platforms to VideoGen.

The user configures API credentials for each supported platform once, and
Video Platforms brokers generation requests to the chosen provider — producing
video clips from prompts, transcripts, or storyboard data supplied by other
services.

## Features

- Per-platform API credential management (keys are stored by the user, not in the repo).
- Unified generation interface across providers.
- Pluggable provider model — new platforms can be added as they gain API support.

## Notes

- Requires valid API credentials for at least one provider before generation works.
- Provider availability and pricing are external to VideoGen.
