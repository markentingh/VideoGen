# OpenAI

**Type:** `llms`

Connects VideoGen to any LLM that supports the OpenAI API model.

The user configures an API key and base URL, and the OpenAI service exposes a
uniform chat-completions interface to the rest of the pipeline — usable for
transcript generation, prompt expansion, and any other LLM-backed step. Works
with OpenAI itself as well as any provider that exposes an OpenAI-compatible API
(Azure OpenAI, OpenRouter, local inference servers, etc.).

## Features

- API key + base URL configuration — point at any OpenAI-compatible endpoint.
- Model selection for LLM-backed pipeline steps.
- Unified interface consumed by other VideoGen services.

## Notes

- Requires valid API credentials for the configured endpoint.
- API usage is billed by the provider under the user's own account.
