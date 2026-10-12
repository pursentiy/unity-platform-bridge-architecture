# Unity Platform Bridge Architecture

A code sample from my Unity WebGL projects. It shows how game code talks to web platform SDKs (ads, social, auth, storage, lifecycle) without knowing which platform it runs on.

In the real project the same interfaces have implementations for Playgama Bridge (Yandex Games, GameDistribution, VK/OK, Playgama) and a separate CrazyGames SDK integration. This repo contains the interfaces, the services on top of them and the editor mocks. The SDK adapters are left out.

## How it fits together

```
Game code
   │
   ▼
Services/        AdsService, SocialShareService  (both use Overlay/OverlayTaskRunner)
   │                 │
   │                 └── pause on/off ──► Core/Events/GlobalEventsService ──► Gameplay/OnGamePauseManager (Time.timeScale)
   ▼
Platform/Abstractions/   IPlatformAds, IPlatformSocial, IPlatformAuth, IPlatformStorage, ...
   │
   ├── Platform/Mocks/   editor and test builds
   └── (Playgama / CrazyGames adapters, not included)
```

## What to look at

- **`Services/Overlay/OverlayTaskRunner.cs`**: every platform overlay (ad, share, invite) runs through it. It pauses the game, starts the SDK call and watches it with a hard timeout in case the SDK never calls back. The result resolves exactly once, and the game unpauses on every path: success, error, timeout or dispose. Only one overlay runs at a time.
- **`Services/AdsService.cs`, `Services/SocialShareService.cs`**: thin services on top of the runner. Each one checks what the platform supports and sets its own timeouts (35s interstitial, 90s rewarded, 5-10 min for share and invite dialogs).
- **`Platform/Abstractions/`**: one small interface per platform concern, so a platform can support ads but not invites, for example. Services check `IsXSupported` before calling.
- **`Gameplay/OnGamePauseManager.cs`**: pause is driven by an event, so the platform layer never touches `Time.timeScale` directly. It restores the previous time scale, not just `1`.
- **`Installers/`**: VContainer setup. The platform is picked at compile time by scripting define (`USE_MOCK_PLATFORM`, `WEBGL_PLAYGAMA`, `WEBGL_CRAZYGAMES`), and the editor always gets mocks.

## Folders

| Folder | Contents |
| --- | --- |
| `Core/Base` | `DisposableService` base class and disposable helpers |
| `Core/Logging` | `LoggerService` |
| `Core/Events` | `GlobalEventsService` |
| `Platform/Abstractions` | `IPlatform*` interfaces |
| `Platform/Enums` | `PlatformType`, `PlatformDeviceType` |
| `Platform/Mocks` | editor implementations of every interface |
| `Services` | game-facing services |
| `Services/Overlay` | `OverlayTaskRunner`, shared pause and timeout flow |
| `Gameplay` | `OnGamePauseManager` |
| `Installers` | VContainer bindings (`ProjectInstaller` is an excerpt) |

## Tech Stack

- Unity, C#
- UniTask (async, cancellation, timeouts)
- VContainer (DI)

## How it was made

The architecture and the code are mine, taken from my WebGL projects. I use Claude Code every day in my own AI-assisted workflow (skills, code rules, reviewer agents, hooks). In this repo it helped with the cleanup: extracting `OverlayTaskRunner` from two duplicated services, a review pass that found a missing unpause and a broken busy check, and the README.
