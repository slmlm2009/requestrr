# Implementation Plan: Discord Media Request Quality Selection

## Summary
Introduce a quality profile selection step in the Discord request workflows for both TV shows (Sonarr) and movies (Radarr). After a user picks a title (and a season for TV), they must choose a quality profile matching the profiles defined in the associated download client category before confirming the request.

## Detailed Steps

### 1. Domain Model & Client Enhancements
- Extend `TvShowRequest` and `MovieRequest` to carry optional `QualityProfileId` and `QualityProfileName`.
- Add a reusable `QualityProfile` DTO plus interface(s) that expose per-category profiles:
  - Implement Sonarr-specific provider using existing `GetProfiles` APIs.
  - Implement Radarr-specific provider similarly.
- Update Sonarr and Radarr request logic to honor an override when present, falling back to the category’s default profile otherwise.

### 2. Workflow Adjustments
- Update workflow factories (`TvShowWorkflowFactory`, `MovieWorkflowFactory`) to inject the profile providers.
- Expand TV season workflows (`Normal`, `All`, `Future`) to:
  - Detect when multiple profiles are available and no selection has been made yet.
  - Prompt user for a quality profile, then re-run the workflow with the chosen profile before confirmation.
- Modify `MovieRequestingWorkflow` to insert the quality selection step between title selection and confirmation.
- Add `HandleQualitySelectionAsync` (or equivalent) in both workflows to process the new dropdown responses and stash the chosen profile alongside the request.

### 3. Discord UI Updates
- Extend `ITvShowUserInterface` and `IMovieUserInterface` with `DisplayQualitySelectionAsync`.
- Implement quality-selection rendering in `DiscordTvShowUserInterface` and `DiscordMovieUserInterface`:
  - Create select menus (`TQS` / `MQS`) listing available profiles, defaulting to the category’s configured profile.
  - Reflect the chosen quality in subsequent confirmation messages and embeds.
  - Update `AddPreviousDropdownsAsync` helpers to keep previously rendered selectors so users can revise their picks.

### 4. Interaction Handling
- Update `ChatBot.HandleTvRequestAsync` and `HandleMovieRequestAsync`:
  - Parse new component IDs (`tqs`, `mqs`) and delegate to the workflows.
  - Amend confirmation button handlers (`trc`, `mrc`) to pass along the selected quality profile IDs.
- Ensure slash-command generation remains unchanged (quality choices are runtime, not compile-time).

### 5. Localization
- Introduce new language tokens such as:
  - `Discord.Command.Media.SelectQuality`
  - `Discord.Command.Media.QualitySelected`
- Add entries for all existing locale JSON files (reuse English text where translations are unavailable).

### 6. Testing
- Run `dotnet build Requestrr.WebApi/Requestrr.WebApi.csproj`.
- Optionally run `npm run build` in `ClientApp` to verify locale updates don’t break the SPA.
