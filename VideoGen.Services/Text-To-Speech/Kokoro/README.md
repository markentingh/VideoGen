# Kokoro

**Type:** `text-to-speech`

Converts transcript text into natural-sounding speech using Kokoro — the best
English open-weight text-to-speech model available.

Kokoro turns each chapter's narration text into an audio voiceover track that
can be layered under the generated video. Because the model is open-weight, it
runs locally with no per-request API costs and no external dependencies.

## Features

- High-quality English speech synthesis from plain text.
- Open-weight model — runs fully local/offline.
- Per-chapter audio output aligned with the transcript structure.

## Notes

- Requires the Kokoro model weights to be installed (see `install.yaml`).
- English voices only; other languages are not supported by the model.
