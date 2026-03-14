<div align="center">

```
██╗   ██╗███████╗██████╗ ████████╗██╗ ██████╗  ██████╗     ███████╗██████╗ ██╗███╗   ██╗
██║   ██║██╔════╝██╔══██╗╚══██╔══╝██║██╔════╝ ██╔═══██╗    ██╔════╝██╔══██╗██║████╗  ██║
██║   ██║█████╗  ██████╔╝   ██║   ██║██║  ███╗██║   ██║    ███████╗██████╔╝██║██╔██╗ ██║
╚██╗ ██╔╝██╔══╝  ██╔══██╗   ██║   ██║██║   ██║██║   ██║    ╚════██║██╔═══╝ ██║██║╚██╗██║
 ╚████╔╝ ███████╗██║  ██║   ██║   ██║╚██████╔╝╚██████╔╝    ███████║██║     ██║██║ ╚████║
  ╚═══╝  ╚══════╝╚═╝  ╚═╝   ╚═╝   ╚═╝ ╚═════╝  ╚═════╝     ╚══════╝╚═╝     ╚═╝╚═╝  ╚═══╝
```

### `> Wheel of Fortune Mobile Game`

<img src="https://readme-typing-svg.demolab.com?font=Fira+Code&size=18&duration=3000&pause=1000&color=00D4FF&center=true&vCenter=true&width=500&lines=Spin+the+wheel.+Collect+rewards.;Risk+it+all+%E2%80%94+or+walk+away.;Normal+%E2%86%92+Safe+%E2%86%92+Super+zones;Built+with+Unity+%26+C%23+for+mobile" alt="Typing SVG" />

<br/>

![Unity](https://img.shields.io/badge/Unity-2021.3_LTS-00d4ff?style=for-the-badge&logo=unity&logoColor=white&labelColor=0a0a12)
![C#](https://img.shields.io/badge/C%23-.NET_Standard_2.1-7b2fff?style=for-the-badge&logo=csharp&logoColor=white&labelColor=0a0a12)
![Android](https://img.shields.io/badge/Android-APK-3DDC84?style=for-the-badge&logo=android&logoColor=white&labelColor=0a0a12)
![iOS](https://img.shields.io/badge/iOS-Supported-ff2d95?style=for-the-badge&logo=apple&logoColor=white&labelColor=0a0a12)

</div>

---

### `// About`

A **Wheel of Fortune** mobile game built with **Unity 2021.3 LTS** for **Vertigo Games**. Players spin a wheel with reward slices and one bomb — each zone raises the stakes with better rewards and higher risk. Spin the wheel, collect rewards, risk it all — or walk away with your winnings.

**Features:**
- Editor-configurable wheel slices via ScriptableObjects
- Zone progression with escalating rewards
- Safe zones, super zones, and bomb risk
- Revive system with in-game currency
- Haptic feedback on iOS + Android
- Object pooling for smooth performance

---

### `// Gameplay`

### Zone Types

| Zone | Frequency | Bomb | Can Leave |
|:-----|:----------|:-----|:----------|
| **Normal** | Default | Yes | No |
| **Safe** (Silver) | Every 5th | No | Yes |
| **Super** (Gold) | Every 30th | No | Yes |

- **Bomb hit** = lose all collected rewards, game restarts
- **Revive** = optional continue with in-game currency

---

### `// Getting Started`

### Prerequisites

- **Unity 2021.3.45f1 LTS** (or compatible 2021.3.x)
- Android SDK (for APK builds)
- Xcode (for iOS builds, macOS only)

### Setup

```bash
# Clone the repository
git clone https://github.com/SHop4R/VertigoSpin.git

# Open in Unity Hub → Add project from disk
# Wait for asset import & compilation
# Open Assets/Project/Scenes/Game.unity
```

### Building

<details>
<summary><strong>Android APK</strong></summary>
<br>

1. `File` → `Build Settings` → `Android`
2. Switch platform if needed
3. Click `Build` or `Build and Run`

</details>

<details>
<summary><strong>iOS</strong></summary>
<br>

1. `File` → `Build Settings` → `iOS`
2. Build to Xcode project
3. Archive and distribute from Xcode

</details>

---

### `// Architecture`

### Scene Flow

```
┌──────────────────────┐           ┌──────────────────────┐
│    LoadingScreen      │  async    │        Game           │
│    (Build Index 0)    │ ───────►  │    (Build Index 1)    │
│                       │   load    │                       │
│  ILoadingStep Pipeline│           │   Main Gameplay       │
│  ───► Fade Out        │           │   Scene               │
└──────────────────────┘           └──────────────────────┘
```

### Manager Singletons

All managers inherit from `MonoSingleton<T>` (lazy find-or-create):

```
                       ┌───────────────────┐
                       │  MonoSingleton<T>  │
                       └─────────┬─────────┘
           ┌───────────┬─────────┼─────────┬───────────┐
           ▼           ▼         ▼         ▼           ▼
     ┌──────────┐ ┌─────────┐ ┌──────┐ ┌─────────┐ ┌───────┐
     │   Game   │ │  Audio  │ │  UI  │ │ Haptic  │ │ Event │
     │ Manager  │ │ Manager │ │ Mgr  │ │ Manager │ │  Mgr  │
     └──────────┘ └─────────┘ └──────┘ └─────────┘ └───────┘
      DOTween      Sound       Panel    iOS/Android   Event
      60fps        System      State    Vibration     Bus
```

### Key Systems

- **Audio** — `SoundData` (ScriptableObject) → `CreatedSound` (AudioSource wrapper) → `AudioManager`. Pitch ramping, random clip selection, auto-reset timers
- **Object Pooling** — Generic `Pool<T>` with `IPoolable` callbacks (`OnSpawn`/`OnReturn`)
- **Haptics** — NiceVibrations on iOS, custom `AndroidHaptic` with JNI on Android
- **Utilities** — `WaitHelper` (GC-free cached waits), `CameraHelper`, `SceneHelper` (async loading), `ListExtensions` (Fisher-Yates shuffle)

---

### `// Project Structure`

```
Assets/
├── Plugins/
│   ├── Demigiant/                  # DOTween / DOTween Pro
│   └── Android/                    # Android-specific plugins
│
├── Project/
│   ├── Graphics/2D/                # Sprites & sprite atlases
│   ├── Prefabs/                    # Reusable GameObjects
│   ├── Resources/                  # Runtime-loaded assets (SoundData)
│   ├── Scenes/
│   │   ├── LoadingScreen.unity     # Entry point (build index 0)
│   │   └── Game.unity              # Main gameplay (build index 1)
│   ├── Scripts/
│   │   ├── Animations/             # DOTween animations, VFX
│   │   ├── Audio/                  # Sound system
│   │   ├── Data/                   # Game state, models
│   │   ├── Game/                   # Zones, spin controller
│   │   ├── Haptic/                 # Platform haptic feedback
│   │   ├── Managers/               # Singleton managers
│   │   ├── Pooling/                # Object pool system
│   │   ├── UI/                     # Panels & components
│   │   ├── Utils/                  # Helpers & extensions
│   │   └── Wheel/                  # Wheel controller & VFX
│   └── Sounds/                     # Audio clips (.mp3, .wav)
│
└── TextMesh Pro/                    # TMP essentials
```

---

### `// Dependencies`

<div align="center">

![Unity](https://img.shields.io/badge/Unity-000000?style=for-the-badge&logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-9B4F96?style=for-the-badge&logo=csharp&logoColor=white)
![DOTween](https://img.shields.io/badge/DOTween_Pro-00d4ff?style=for-the-badge&labelColor=0a0a12)
![TextMeshPro](https://img.shields.io/badge/TextMeshPro-7b2fff?style=for-the-badge&labelColor=0a0a12)
![NiceVibrations](https://img.shields.io/badge/Nice_Vibrations-ff2d95?style=for-the-badge&labelColor=0a0a12)

</div>

| Package | Purpose | Location |
|:--------|:--------|:---------|
| **DOTween Pro** | Procedural & UI animations | `Assets/Plugins/Demigiant/` |
| **Nice Vibrations** (Lofelt) | iOS haptic feedback | Package Manager |
| **TextMeshPro** | All text rendering | Built-in Unity package |

---

### `// Author`

<div align="center">

<a href="https://github.com/SHop4R">
  <img src="https://github.com/SHop4R.png" width="80" style="border-radius:50%"><br>
  <strong>Ege Akarsu</strong>
</a>

<br/><br/>

[![Website](https://img.shields.io/badge/Website-00d4ff?style=for-the-badge&logo=googlechrome&logoColor=white&labelColor=0a0a12)](https://egeakarsu.dev)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white&labelColor=0a0a12)](https://linkedin.com/in/egeakarsu)
[![Email](https://img.shields.io/badge/Email-ff2d95?style=for-the-badge&logo=maildotru&logoColor=white&labelColor=0a0a12)](mailto:akarsu.ege@gmail.com)

</div>

---

<div align="center">
<img src="https://capsule-render.vercel.app/api?type=waving&color=0:7b2fff,100:00d4ff&height=80&section=footer" width="100%" />
</div>
