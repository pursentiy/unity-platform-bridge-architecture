# Unity Platform-Agnostic Bridge Architecture

A lightweight bridge that keeps game logic separate from platform SDKs (WebGL, mobile ads, social features, etc.).

## Key Features
* **Platform Abstraction:** IPlatformAds, IPlatformSocial, and IPlatformEnvironment interfaces let you swap SDK implementations without touching core gameplay code.
* **Robust Async Handling (UniTask):** Full UniTask + CancellationTokenSource support with double timeouts (loading + hard) so a hung ad network can’t freeze the game.
* **Mock Framework:** Complete mock implementations for in-editor testing without real SDK builds.
* **Event-Driven Pause Control:** Game pause (Time.timeScale) is handled through signals via GlobalEventsService, keeping the platform layer decoupled.

##Tech Stack
* **Engine:** Unity
* **Language:** C#
* **Async Library:** UniTask
