# Short Transcripts

**Type:** `transcript-generation`

Generates video transcripts purpose-built for short-form content on TikTok,
Instagram Reels, and YouTube Shorts.

Given a prompt describing the video, Short Transcripts produces a `Transcript`
made up of short, punchy `Chapter`s (title + text) tuned for the pacing of
sub-60-second vertical video — hooks up front, tight beats, and a clear
call-to-action at the end.

## Output

| Field | Description |
|-------|-------------|
| `Chapters[].Title` | Short chapter heading (used for captions/overlays) |
| `Chapters[].Text`  | Spoken narration text for the chapter |

## Notes

- Optimized for vertical short-form platforms; not intended for long-form scripts.
- Chapter text is written to be read aloud at natural short-form pacing.
