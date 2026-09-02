# AI-Powered Trainee Help Desk Assistant — Frontend (trainee-helpdesk-ui)

Angular 20.3 standalone application providing the chat UI for the
AI-Powered Trainee Help Desk Assistant (RAG-based backend).

## Tech stack

- Angular 20.3 (standalone components, signals)
- Angular Forms (`ngModel`) for the message input
- `HttpClient` (via `provideHttpClient`) to call the ASP.NET Core Web API

## Project structure

```
src/app/
  core/
    models/chat.model.ts       -> AskRequest/AskResponse/ChatMessage interfaces
    services/chat.service.ts   -> HttpClient calls to the backend API
  features/
    chat/
      chat.ts                  -> chat component logic (signals)
      chat.html                -> chat UI template
      chat.scss                -> chat UI styles
  app.ts / app.html / app.scss -> root shell component
  app.config.ts                -> app-wide providers (HttpClient, zone change detection)
src/environments/
  environment.ts                -> apiBaseUrl for local dev (https://localhost:7250/api)
  environment.prod.ts           -> apiBaseUrl for production (/api)
```

## Setup instructions

### 1. Install dependencies

```bash
cd trainee-helpdesk-ui
npm install
```

### 2. Point the app at your backend

By default the app calls the backend at `https://localhost:7250/api`
(see `src/environments/environment.ts`). Update this if your
`TrainingHelpDeskApi` backend runs on a different port.

### 3. Run the app

```bash
npm start
```

The app runs at `http://localhost:4200`. Make sure the backend's
`Cors:AllowedOrigin` setting in `appsettings.json` matches this URL
(it does, by default).

## Features

- Chat-style interface where trainees type a question and receive an
  AI-generated answer sourced from the Trainee Knowledge Base.
- Quick-start suggested questions for common topics.
- Displays which knowledge base document(s) an answer came from.
- Friendly "typing" indicator while waiting for a response.
- Clear error banner if the backend API is unreachable.

## Build for production

```bash
npm run build
```

Output is generated in `dist/trainee-helpdesk-ui`.
