# Copilot Coding Agent — Repository Onboarding

Purpose: give a coding agent the minimal, high-value facts and verified commands
needed to implement, build, run, and validate changes in this repository quickly.

Summary
- This repo implements UiExplain: a Blazor Web front-end (`UiExplain.Web`) and
  an ASP.NET Core API (`UiExplain.Api`) that analyze UI screenshots using external
  AI services (Hugging Face for image captioning; Groq for LLM-based analysis).
- Target runtime: .NET 9 (see project files targeting `net9.0`).

Repo type & size
- Solution: `UiExplain.sln` (two projects). Small-to-medium codebase (~dozens
  of C# source files, static web assets). No test projects detected.

Essential facts the agent should trust (don’t re-discover unless inconsistent)
- SDK: use .NET 9 SDK. Always run `dotnet --version` and ensure it reports a
  .NET 9.x SDK before building.
- Projects and roles:
  - `UiExplain.Api` — API: controllers, services, AI integrations.
  - `UiExplain.Web` — Blazor Web app (Interactive Server), calls the API.
- Local dev runtime ports (from launchSettings): API HTTPS default is
  `https://localhost:7149` (API also listens on http 5069). Web app launch uses
  `https://localhost:7175` (http 5211). The Web project configures an
  `HttpClient` with base `https://localhost:7149/`.
- External secrets: `HuggingFaceApiKey` and `GroqApiKey` expected in
  `UiExplain.Api/appsettings.json` or environment variables. Do not hardcode.

Build / bootstrap / run (verified from project files and launch settings)
Note: follow the sequence exactly to avoid transient failures.

Prerequisites (always do these first)
- Install .NET 9 SDK (required). Confirm with:

  `dotnet --version`

- From repository root, always restore packages before build:

  `dotnet restore UiExplain.sln`

Build
- Build solution:

  `dotnet build UiExplain.sln -c Debug`

- Common issues: missing SDK -> `dotnet` fails; fix by installing .NET 9.

Run locally (recommended order)
1. Start the API project first (it exposes endpoints the Web app expects):

   `dotnet run --project UiExplain.Api --launch-profile "https"`

   - This will use the API `https` profile (ports configured in
     `UiExplain.Api/Properties/launchSettings.json` — default `7149`).
   - If you can't use HTTPS locally, run the `http` profile instead, and
     update the Web project's `HttpClient` base address accordingly.

2. Start the Web project:

   `dotnet run --project UiExplain.Web --launch-profile "https"`

3. Open the Web app at the URL shown in the `dotnet run` output (launchSettings
   uses `7175` by default). Visit the `/explain` page to exercise image upload.

Validation / tests
- There are no automated test projects in the repository. Validation is manual:
  - Confirm both projects run concurrently.
  - Upload an image on `/explain` and observe API calls (the UI calls the API
    endpoint that delegates to the orchestrator and AI services).

Linting / formatting
- No repository-level linting (no editorconfig or analyzers enforced here).
  Use standard `dotnet format` if desired, but not required for CI.

Common pitfalls & mitigation
- Mismatched ports: the Web project’s `HttpClient` is set to
  `https://localhost:7149/`. If the API runs on a different port/profile,
  update `UiExplain.Web/Program.cs` `HttpClient` registration or run the API
  with the expected profile. Always prefer updating runtime args over changing
  code in quick patches.
- Missing API keys: The app will fail AI calls without `HuggingFaceApiKey` or
  `GroqApiKey`. For local testing, set these as environment variables before
  starting the API (preferred) or place them in `UiExplain.Api/appsettings.json`.
- Timeouts on external calls: HttpClient timeouts are configured to 60s in
  `UiExplain.Api/Program.cs` for image and analysis clients. If debugging slow
  remote services, increase timeouts locally.

Checks and CI
- There are no GitHub Actions workflows in `.github/workflows` in this repo.
  That means code-review + local build are primary validators. To reduce PR rejections:
  - Always run `dotnet build UiExplain.sln` and confirm the solution builds.
  - Run both projects locally to validate end-to-end behavior.
  - Check logging output from the API for structured errors around AI calls.

Project layout (priority list — where to make changes)
- Solution root:
  - `UiExplain.sln` (solution file)
  - `README.md` (usage notes)
- `UiExplain.Api/` (primary backend changes)
  - `Program.cs` — app startup, registered services, HttpClient timeouts, CORS.
  - `Controllers/ExplainUiController.cs` — controller entry point for uploads.
  - `Services/` — `IImageCaptionService`, `ImageCaptionService`, `IUiAnalysisService`, `UiAnalysisService`, `UiExplainOrchestrator`.
  - `Models/` — request and response models, `ApiSettings.cs` contains config keys.
  - `appsettings.json` / `appsettings.Development.json` — local config and keys.
  - `Properties/launchSettings.json` — local ports and launch profiles.
- `UiExplain.Web/` (UI changes)
  - `Program.cs` — Blazor setup and `HttpClient` registration (base address matters).
  - `Pages/Explain.razor` — UI for uploading images and showing results.
  - `Components/ResultPanel.razor` — displays `ExplainResult`.
  - `wwwroot/` — static assets (css/js/bootstrap, etc.).

Quick editing heuristics for the agent
- Prefer small, focused changes. Update a single service/controller at a time.
- When changing API surface (routes/models), update both `UiExplain.Api` and
  `UiExplain.Web` client code to match.
- If a change touches configuration (ports, appsettings keys), prefer to
  document it in `README.md` and keep defaults backward-compatible.

What to run before opening a PR (in order)
1. `dotnet restore UiExplain.sln`
2. `dotnet build UiExplain.sln -c Debug`
3. Run `UiExplain.Api` then `UiExplain.Web` as shown above and manually exercise `/explain`.
4. Verify logs for no errors and that the UI receives `ExplainResult`.

When to re-run a code search
- Trust this file for the standard build/run flows. Only re-run repository
  searches if observed behavior contradicts these instructions (e.g., a file
  mentioned here is missing or heavily changed), or if you need to find
  specific helper functions not documented above.

Contact points (for reviewers)
- If a PR changes startup, HttpClient base addresses, or AI integration
  prompts, leave clear notes in the PR description stating how you validated
  the change locally (build output, ports used, environment variables set).

End — keep changes focused and test end-to-end locally before PR.
