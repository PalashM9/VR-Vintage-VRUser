# VR Vintage Car Showcase (XR Prototype)

<img width="1209" height="445" alt="image" src="https://github.com/user-attachments/assets/05008f75-1f9b-4950-9cb1-31bd584e621c" />


An immersive **VR/XR prototype** showcasing vintage automobiles within an interactive racing-style environment. The project explores **spatial interaction, locomotion, object placement, and XR input handling** using Unity’s XR stack, with a focus on realism, scale, and user orientation.

---

## Project Overview

This project presents a **first-person XR experience** where users can:

- Navigate a curated environment

- Interact using hand/controller input

- View and approach vintage vehicles placed within a structured track setting

- Experience spatial cues, depth, and scale in a VR-ready scene

The prototype is designed as a **foundation for a multi-floor or multi-exhibit VR experience**, such as a virtual museum, portfolio space, or interactive storytelling environment.

---

## Key Features

- **XR Device Simulator Support**

Allows full testing without a physical headset using keyboard and mouse input.

- **Hand-Based Interaction**

Virtual hands rendered in first-person for realistic interaction and orientation.

- **Guided Navigation Cues**

Visual direction indicators (ground paths and markers) to guide user movement.

- **Vintage Vehicle Assets**

Imported and positioned car models, optimized for real-time rendering.

- **Outdoor Track Environment**

Open environment with banners, signage, and depth cues for immersion.

---

## Controls (XR Device Simulator)

| Action | Input |

|------|------|

| Move | `W A S D` |

| Look | Mouse |

| Toggle XR Input | `Tab` |

| Switch Controllers | `U` |

| Reset Position | `R` |

| Hands / Controller Toggle | `T` |

> These controls are visible in the **XR Device Simulator overlay** during Play Mode.

---

## Technical Stack

- **Engine:** Unity (XR-enabled)

- **XR Framework:** Unity XR Interaction Toolkit

- **Simulation:** XR Device Simulator

- **Rendering:** Real-time 3D (URP / Built-in, depending on project setup)

- **Assets:** GLB/Prefab-based vehicle models

- **Input:** XR Input System (Keyboard + Mouse fallback)

---

## Project Structure

Assets/

├── Prefab/

│ └── Cars/

│ ├── IntroCar.glb

│ ├── vintage_race_car.prefab

│ └── SpawnCars/

├── Environments/

├── Scenes/

├── Scripts/

├── XR/

└── VRSYS/

---

## Current Status

- Environment layout complete

- XR input and hand rendering functional

- Vehicle placement and orientation validated

- Navigation cues implemented

---

## Known Limitations

- Some GLB assets may require re-export without compression (e.g., Draco)

- No physics-based interaction with vehicles yet

- Single-scene prototype (multi-scene flow planned)

---

## Planned Enhancements

- Multi-floor or elevator-based navigation

- Vehicle interaction (inspect, rotate, metadata display)

- Audio narration and spatial sound

- Performance optimization for standalone VR headsets

- Networked multi-user walkthrough (future scope)

---

## Use Cases

- Virtual museum or exhibition

- Interactive portfolio experience

- VR storytelling or guided tours

- XR research and prototyping

---

Focus: XR systems, conversational AI, immersive interaction design

---

## License

This project is for **educational and prototyping purposes**.

Asset licenses remain with their respective creators.
