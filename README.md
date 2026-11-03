# VideoGen

AI-powered short-form video generator — a modular ASP.NET Core + React application that turns prompts into finished videos for TikTok, Instagram Reels, and YouTube Shorts.

VideoGen is built around an installable **services architecture**: each capability of the generation pipeline — transcripts, video assets, video generation, transitions, image generation, text-to-speech, and captions — is a service that can be installed into the application at runtime. See [Docs/Services.md](Docs/Services.md) for the full architecture.

## How it works

1. A **service registry** (a JSON file at a URL or filesystem path) lists available services. Registry URLs are configured in the dashboard under **Services → Settings** (stored as `Services:Urls` in appsettings).
2. The dashboard's **Services** page fetches and merges every configured registry, then groups services by type.
3. Clicking **Install** downloads the service repo (git clone or local path), copies its `VideoGen.Web.Client` React components into the client, its `VideoGen.Plugins` `.cs` files into the plugin library, and applies any `VideoGen.SQL` migrations.
4. Installed services expose lazy-loaded React components (`serviceConfig`, `projectDetails`) and server-side plugins compiled into `VideoGen.Plugins`.

Service types:

- `transcript-generation` — generate transcripts/scripts (e.g. Short Transcripts for TikTok, Reels & Shorts)
- `video-library` — source video assets (e.g. Local Videos from filesystem folders)
- `video-generation` — AI video platform integrations
- `video-transitions` — stitch clips together with transitions
- `image-generation` — AI image generation (e.g. GPT Image)
- `text-to-speech` — narration voiceover (e.g. Kokoro open-weight TTS)
- `text-captions` — burn timed captions into the final video

## Projects

- **VideoGen.API** — ASP.NET Core API controllers (admin users, services registry/installer).
- **VideoGen.Auth** — JWT authentication, policies, and account controller.
- **VideoGen.Data** — Dapper data access layer (auth + installed-services repositories).
- **VideoGen.SQL** — PostgreSQL schema scripts; `gulp update-deploy` regenerates `deploy.sql`.
- **VideoGen.Plugins** — class library of interfaces/models that service plugins implement and compile into.
- **VideoGen.Web.Server** — ASP.NET Core host serving the API and SPA.
- **VideoGen.Web.Client** — React + Vite + TailwindCSS dashboard.
- **VideoGen.Services** — local service registry source; each service lives in `{type}/{repo-name}`.

## Prerequisites

- **.NET 9 SDK** or later
- **PostgreSQL** — a running server with credentials that can create the database
- **Node.js LTS** + npm
- **Git**

## Setup

1. Clone the repository.

2. Create `VideoGen.Web.Server/appsettings.json` from `appsettings.template.json` and set your PostgreSQL connection string in `ConnectionStrings:Database`.

3. Deploy the database schema (creates the `videogen` database if needed):

   ```bash
   cd VideoGen.SQL
   .\deploy.bat
   ```

   Or manually: `psql -U <username> -d template1 -f deploy.sql`

4. Install client dependencies and start the Vite dev server (port 7717, proxies `/api` to the server):

   ```bash
   cd VideoGen.Web.Client
   npm install
   npm run dev
   ```

5. Start the API/web server (ports 7715/7716):

   ```bash
   cd VideoGen.Web.Server
   dotnet run
   ```

6. Open `https://localhost:7717`. The first account you create is automatically promoted to admin — sign up, then head to **Services** to install services from the registry (defaults to the built-in `services.json` at `https://localhost:7717/services.json`).
