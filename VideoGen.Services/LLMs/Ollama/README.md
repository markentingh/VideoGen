# Ollama

**Type:** `llms`

Connects VideoGen to an Ollama server and lets the user select any supported
LLM model for Ollama to download and run locally.

The user points the service at an Ollama server URL, browses the catalog of
supported models, and picks which ones to pull. Selected models run entirely
on the Ollama server — no external API keys or per-request costs — and back
the same LLM-backed pipeline steps as any other LLM service (transcript
generation, prompt expansion, etc.).

## Features

- Connects to a local or remote Ollama server by URL.
- Browse supported models and trigger Ollama downloads from the service UI.
- Local inference — open-weight models, no API keys, no usage billing.

## Notes

- Requires a running Ollama server and disk space for model weights.
- Model downloads can be large; first use of a model may take a while.
