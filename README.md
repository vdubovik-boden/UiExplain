# UiExplain

## Overview

UiExplain is a Blazor-based web application designed to analyze user interface (UI) screenshots using advanced AI models. The app allows users to upload images of UI designs, which are then processed through a two-stage AI pipeline:

1. **Image Captioning**: Utilizes Hugging Face's vision models to generate descriptive captions of the uploaded UI screenshots.
2. **UX Analysis**: Leverages Groq's LLM to provide detailed insights on the UI, including summaries, main elements, UX issues, and accessibility suggestions.

The application consists of two main projects:
- **UiExplain.Web**: A Blazor Web App providing the user interface, including pages for home, UI explanation, and admin dashboard.
- **UiExplain.Api**: An ASP.NET Core API handling image processing, AI integrations, and data orchestration.

Built with .NET 9, the app emphasizes production-ready practices such as input validation, error handling, logging, and responsive design using Bootstrap.

## Features

- **AI-Powered Analysis**: Integrates with Hugging Face and Groq APIs for accurate UI insights.
- **Responsive UI**: Mobile-friendly design with Bootstrap components.
- **Admin Dashboard**: View previous analysis summaries in a card-based layout.
- **Structured Logging**: Comprehensive logging for AI requests, responses, and errors.
- **Validation**: Client and server-side checks for image uploads (size, type).

## Getting Started

1. Clone the repository.
2. Set API keys in `UiExplain.Api\appsettings.json`:
   - `HuggingFaceApiKey`
   - `GroqApiKey`
3. Run both projects (Web and API) simultaneously.
4. Navigate to the Web app and upload a UI screenshot on the `/explain` page.

## Architecture

- **Frontend**: Blazor Server with Interactive Server rendering for dynamic components.
- **Backend**: ASP.NET Core API with clean architecture (Controllers, Services, Models).
- **AI Integration**: External APIs for vision and language processing.
- **Styling**: Bootstrap for responsive layouts and icons.

---

# Prompt & Workflow Log

## 1. Overview of AI Usage in Building the App

This document outlines how I used AI (specifically GitHub Copilot in Visual Studio and Grok Code Fast as an agent) to build the UiExplain application, a Blazor-based UI analysis tool that leverages AI for image captioning and UX insights. The app consists of a Blazor Web App (UiExplain.Web) and an ASP.NET Core API (UiExplain.Api), targeting .NET 9.

### Key Prompts Used (in Order)
- **Initial Setup**: "Configure the solution so the Web project can call the API project locally."
- **UI Creation**: "Create the following pages in UiExplain.Web: • Explain UI page ("/explain") add to the menu"
- **API Integration**: "use WebApi call in OnInitializedAsync"
- **UI Refinements**: "let move Explain button below the card. And fix the gap between for mobile view"
- **Icon Fixes**: "Can not see icons. it might need to ass some bootsrap link?"
- **Styling**: "let pretify Result Panel. make it stylish for h6 as a parent and their child."
- **Admin Features**: "Create an AdminLayout.razor that is different from MainLayout. It should include: • A side navigation menu • A header with "Admin Area" • A content section Then create Admin page ("/Admin") within AdminLayout applied Implement the Admin page using in-memory fake data. Show previous explanation summaries in a card list. No database required."
- **Renaming**: "please rename IExplainUiService accordingly"
- **API Structure**: "In UiExplain.Api, create a clean structure with: • Controllers • Services • Models Add an ExplainUiController with a POST endpoint that accepts an image. The controller should delegate logic to a service. Create an interface-based service for explaining a UI screenshot. The service should: • Accept an image stream • Return a structured response model"
- **AI Integration (Captioning)**: "Implement IImageCaptionService using Hugging Face Inference API. Requirements: - Use the model "Salesforce/blip-image-captioning-base" - Use HttpClient - Send the image as raw bytes - Extract the generated_text from the response JSON - Handle basic HTTP errors"
- **AI Integration (Analysis)**: "Create an interface named IUiAnalysisService. It should: - Accept a UI caption string - Return an ExplainUiResponse Implement IUiAnalysisService using the Groq OpenAI-compatible API. Requirements: - Endpoint: https://api.groq.com/openai/v1/chat/completions - Model: llama-3.1-8b-instant - Temperature: 0.2 - Send a system message establishing UX and accessibility expertise - Send a user prompt that requests JSON ONLY output - Deserialize the model response into ExplainResult"
- **API Consumption**: "update Explain.razor to call this endpoint instead of using mock data"
- **Debugging**: "i have 404 response from Api should we api controllers?"
- **Refactoring**: "Please rename ExplainUiService to UiExplainOrchestrator Update ExplainUiController so that: - It accepts an image via multipart/form-data - It calls UiExplainOrchestrator - It returns ExplainResult - It validates image size and type"
- **Further Renaming**: "please rename IExplainUiService accordingly"
- **Model Update**: "Let use other AI model in CaptionImageAsync method Here is a CURL request: [curl command] please align our request accordingly"
- **Logging**: "pleasea add structured logging around where it nice to have, eg.: - Vision caption output - LLM request/response timing - Errors from external AI services"
- **Content Update**: "update home page content describe what application do and how does it work do it stylish"
- **Admin Refinement**: "Create an AdminLayout.razor that is different from MainLayout. It should include: • A side navigation menu • A header with "Admin Area" • A content section Then create Admin page ("/Admin") within AdminLayout applied Implement the Admin page using in-memory fake data. Show previous explanation summaries in a card list. No database required. Ask with variants of implementation if need"
- **Review & Improvements**: "Review the solution and suggest: - SSR-specific best practices - Performance improvements - Common pitfalls to avoid Please suggest improvements with variants or questions. I will select prefered"
- **Implementation**: "please do all you sugested with keep the implementation minimal and production-oriented."

### Context Provided
- **Project Description**: A Blazor app for UI analysis using AI (Hugging Face for vision, Groq for LLM). Includes Web and API projects.
- **Constraints**: .NET 9, prioritize Blazor over MVC/Razor Pages, production-oriented, minimal implementations.
- **Existing Code**: Initial Blazor templates with weather forecast API, basic components, and navigation.

### Models/Tools Used
- **GitHub Copilot in VS**: Used for code generation, refactoring, and suggestions based on prompts. It provided completions, error fixes, and structural improvements.
- **Grok Code Fast as an agent**: Assisted with prompt refinement, debugging, and high-level planning for AI integrations and UI flows.
- **No External Models**: All AI integrations (Hugging Face, Groq) were implemented via API calls, not local models.

## 2. Important Steps with Prompts and Results

### Step 1: Initial Setup
**Prompt**: "Configure the solution so the Web project can call the API project locally."  
**Result**: Added CORS to API, configured HttpClient in Web with base address, updated WeatherForecastService to call API instead of mock data. Enabled local communication between projects.

### Step 2: UI Creation
**Prompt**: "Create the following pages in UiExplain.Web: • Explain UI page ("/explain") add to the menu"  
**Result**: Created Explain.razor page with upload card and result panel, added to NavMenu with icon. Integrated Bootstrap for responsive design.

### Step 3: API Structure
**Prompt**: "In UiExplain.Api, create a clean structure with: • Controllers • Services • Models Add an ExplainUiController with a POST endpoint that accepts an image. The controller should delegate logic to a service. Create an interface-based service for explaining a UI screenshot. The service should: • Accept an image stream • Return a structured response model"  
**Result**: Established Controllers/Services/Models folders, created ExplainUiController with POST endpoint, IExplainUiService interface, and ExplainUiService implementation with mock logic.

### Step 4: AI Integration (Captioning)
**Prompt**: "Implement IImageCaptionService using Hugging Face Inference API. Requirements: - Use the model "Salesforce/blip-image-captioning-base" - Use HttpClient - Send the image as raw bytes - Extract the generated_text from the response JSON - Handle basic HTTP errors"  
**Result**: Implemented ImageCaptionService with HttpClient calls to Hugging Face, base64 encoding, response parsing, and error handling. Updated orchestrator to use it.

### Step 5: AI Integration (Analysis)
**Prompt**: "Create an interface named IUiAnalysisService. It should: - Accept a UI caption string - Return an ExplainUiResponse Implement IUiAnalysisService using the Groq OpenAI-compatible API. Requirements: - Endpoint: https://api.groq.com/openai/v1/chat/completions - Model: llama-3.1-8b-instant - Temperature: 0.2 - Send a system message establishing UX and accessibility expertise - Send a user prompt that requests JSON ONLY output - Deserialize the model response into ExplainResult"  
**Result**: Created IUiAnalysisService and UiAnalysisService with Groq API integration, JSON prompting, and deserialization. Integrated into orchestrator for full AI pipeline.

### Step 6: Refactoring & Validation
**Prompt**: "Please rename ExplainUiService to UiExplainOrchestrator Update ExplainUiController so that: - It accepts an image via multipart/form-data - It calls UiExplainOrchestrator - It returns ExplainResult - It validates image size and type"  
**Result**: Renamed service, added validation (size/type checks), updated controller to use orchestrator. Ensured production-ready input handling.

### Step 7: Logging
**Prompt**: "pleasea add structured logging around where it nice to have, eg.: - Vision caption output - LLM request/response timing - Errors from external AI services"  
**Result**: Added ILogger injections and structured logs for AI requests/responses, timings, and errors across services and orchestrator.

### Step 8: UI Improvements
**Prompt**: "let pretify Result Panel. make it stylish for h6 as a parent and their child."  
**Result**: Enhanced ResultPanel with Bootstrap icons, colors (success/warning/info), and spacing for better visual hierarchy.

### Step 9: Home Page Update
**Prompt**: "update home page content describe what application do and how does it work do it stylish"  
**Result**: Redesigned Home.razor with hero section, feature cards, and workflow explanation using Bootstrap for responsive, professional styling.

### Step 10: Admin Features
**Prompt**: "Create an AdminLayout.razor that is different from MainLayout. It should include: • A side navigation menu • A header with "Admin Area" • A content section Then create Admin page ("/Admin") within AdminLayout applied Implement the Admin page using in-memory fake data. Show previous explanation summaries in a card list. No database required."  
**Result**: Created AdminLayout with sidebar/header, Admin.razor page with fake summaries in cards, added to navigation.

### Step 11: Review & Production Improvements
**Prompt**: "Review the solution and suggest: - SSR-specific best practices - Performance improvements - Common pitfalls to avoid Please suggest improvements with variants or questions. I will select prefered" followed by "please do all you sugested with keep the implementation minimal and production-oriented."  
**Result**: Implemented client-side validation, HttpClient timeouts, stream-safe handling, LLM JSON cleanup, and error resilience across Web and API. Ensured minimal, production-ready code.

## 3. Insights

### Observations and Learnings
- **Copilot works better with backend rather than CSS styles**: For C# code, interfaces, services, and API integrations, Copilot provided highly accurate completions and refactoring suggestions, often generating production-ready code with minimal tweaks. However, for CSS styling and visual design, it required more manual adjustments—suggestions were basic and often needed customization for responsive layouts, colors, and Bootstrap integration.
- **AI-Assisted Workflow Efficiency**: Combining GitHub Copilot for code generation with Grok Code Fast for high-level planning and prompt refinement accelerated development, especially for complex integrations like AI APIs. However, iterative testing was crucial to catch edge cases (e.g., stream handling, JSON parsing).
- **Production Readiness**: Minimal implementations with validation, logging, and error handling proved effective for a demo app, but real-world scaling would need caching, retries, and monitoring.
- **Blazor SSR Nuances**: Ensuring consistent state during prerendering and handling file uploads required careful event management, highlighting the importance of SSR-specific practices.

SCREENSHOTS:
<img width="1897" height="881" alt="image" src="https://github.com/user-attachments/assets/e94bdcc4-fc18-4367-9157-9a09e15cf6d0" />
<img width="205" height="443" alt="image" src="https://github.com/user-attachments/assets/1103a325-9804-44c0-bc89-b525c1827718" />
<img width="206" height="445" alt="image" src="https://github.com/user-attachments/assets/7189d2b8-f911-4290-bc09-39aee9838cc8" />
<img width="1882" height="878" alt="image" src="https://github.com/user-attachments/assets/92c6b71b-3751-4009-a215-09f90882c7b8" />
<img width="427" height="770" alt="image" src="https://github.com/user-attachments/assets/f1eb84b8-8e89-4efc-960f-7ac97ceaeb85" />
<img width="303" height="662" alt="image" src="https://github.com/user-attachments/assets/8b242087-22dd-47a4-b99e-f994093260c9" />
<img width="302" height="628" alt="image" src="https://github.com/user-attachments/assets/17d9ec8a-c08d-4c40-8b3c-6fe49f64f17b" />
<img width="305" height="667" alt="image" src="https://github.com/user-attachments/assets/3e7af146-253d-488a-bbeb-3496ae3a28df" />
<img width="1898" height="617" alt="image" src="https://github.com/user-attachments/assets/e159515e-e694-47d3-9e8b-3b9590784f28" />








