// Popular LLM platforms that support the OpenAI API format.
// endpoint is the OpenAI-compatible base URL (prefills the form when known).
const platforms = [
  {
    name: 'Chat GPT',
    endpoint: 'https://api.openai.com/v1',
    models: [
      { name: 'GPT 6 Astra', model: 'gpt-6-astra' },
      { name: 'GPT 5.6 Sol', model: 'gpt-5.6-sol' },
      { name: 'GPT 5.5', model: 'gpt-5.5' },
      { name: 'GPT 5 Mini', model: 'gpt-5-mini' },
      { name: 'o4 Reasoning', model: 'o4' }
    ]
  },
  {
    name: 'OpenRouter',
    endpoint: 'https://openrouter.ai/api/v1',
    models: [
      { name: 'Auto Router', model: 'openrouter/auto' },
      { name: 'Claude Sonnet', model: 'anthropic/claude-sonnet-4.5' },
      { name: 'Llama 4 Maverick', model: 'meta-llama/llama-4-maverick' },
      { name: 'DeepSeek V3.2', model: 'deepseek/deepseek-v3.2' }
    ]
  },
  {
    name: 'Google Gemini',
    endpoint: 'https://generativelanguage.googleapis.com/v1beta/openai/',
    models: [
      { name: 'Gemini 3 Pro', model: 'gemini-3.0-pro' },
      { name: 'Gemini 3 Flash', model: 'gemini-3.0-flash' },
      { name: 'Gemini 2.5 Pro', model: 'gemini-2.5-pro' }
    ]
  },
  {
    name: 'xAI',
    endpoint: 'https://api.x.ai/v1',
    models: [
      { name: 'Grok 4.1', model: 'grok-4.1' },
      { name: 'Grok 4 Fast', model: 'grok-4-fast' },
      { name: 'Grok Code Fast', model: 'grok-code-fast-1' }
    ]
  },
  {
    name: 'DeepSeek',
    endpoint: 'https://api.deepseek.com/v1',
    models: [
      { name: 'DeepSeek Chat', model: 'deepseek-chat' },
      { name: 'DeepSeek Reasoner', model: 'deepseek-reasoner' }
    ]
  },
  {
    name: 'Mistral AI',
    endpoint: 'https://api.mistral.ai/v1',
    models: [
      { name: 'Mistral Large 3', model: 'mistral-large-latest' },
      { name: 'Mistral Medium', model: 'mistral-medium-latest' },
      { name: 'Codestral', model: 'codestral-latest' },
      { name: 'Ministral 8B', model: 'ministral-8b-latest' }
    ]
  },
  {
    name: 'Groq',
    endpoint: 'https://api.groq.com/openai/v1',
    models: [
      { name: 'Llama 4 Maverick 17B', model: 'meta-llama/llama-4-maverick-17b-128e-instruct' },
      { name: 'Llama 3.3 70B', model: 'llama-3.3-70b-versatile' },
      { name: 'GPT OSS 120B', model: 'openai/gpt-oss-120b' },
      { name: 'Qwen 3 32B', model: 'qwen/qwen3-32b' }
    ]
  },
  {
    name: 'Together AI',
    endpoint: 'https://api.together.xyz/v1',
    models: [
      { name: 'Llama 4 Maverick', model: 'meta-llama/Llama-4-Maverick-17B-128E-Instruct-FP8' },
      { name: 'DeepSeek V3.1', model: 'deepseek-ai/DeepSeek-V3.1' },
      { name: 'Qwen 3 235B', model: 'Qwen/Qwen3-235B-A22B-fp8' }
    ]
  },
  {
    name: 'Fireworks AI',
    endpoint: 'https://api.fireworks.ai/inference/v1',
    models: [
      { name: 'Llama 4 Maverick', model: 'accounts/fireworks/models/llama4-maverick-instruct-basic' },
      { name: 'DeepSeek V3.2', model: 'accounts/fireworks/models/deepseek-v3p2' },
      { name: 'GLM 4.6', model: 'accounts/fireworks/models/glm-4p6' }
    ]
  },
  {
    name: 'Cerebras',
    endpoint: 'https://api.cerebras.ai/v1',
    models: [
      { name: 'Llama 3.3 70B', model: 'llama-3.3-70b' },
      { name: 'GPT OSS 120B', model: 'gpt-oss-120b' },
      { name: 'Qwen 3 235B', model: 'qwen-3-235b-a22b-instruct-2507' }
    ]
  },
  {
    name: 'Perplexity',
    endpoint: 'https://api.perplexity.ai',
    models: [
      { name: 'Sonar Pro', model: 'sonar-pro' },
      { name: 'Sonar', model: 'sonar' },
      { name: 'Sonar Reasoning Pro', model: 'sonar-reasoning-pro' }
    ]
  },
  {
    name: 'Azure OpenAI',
    endpoint: '',
    models: [
      { name: 'GPT 5.6', model: 'gpt-5.6' },
      { name: 'GPT 5 Mini', model: 'gpt-5-mini' }
    ]
  },
  {
    name: 'Ollama',
    endpoint: 'http://localhost:11434/v1',
    models: [
      { name: 'Llama 4', model: 'llama4' },
      { name: 'Qwen 3', model: 'qwen3' },
      { name: 'Mistral', model: 'mistral' },
      { name: 'DeepSeek R1', model: 'deepseek-r1' }
    ]
  },
  {
    name: 'LM Studio',
    endpoint: 'http://localhost:1234/v1',
    models: [
      { name: 'Local Model', model: 'local-model' }
    ]
  }
];

export default platforms;
