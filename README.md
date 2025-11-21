# OutbreakResponseGame

A serious game for seasonal outbreak response optimization, built in Unity 2D.

## Setup Instructions

1. **Open in Unity**: Open this folder as a project in Unity (2021.3 or later recommended).
2. **Generate Project Content**:
   - In the Unity Editor menu bar, click **Tools > Setup Project**.
   - This will automatically generate the `Main` scene, create Prefabs, and setup the UI.
3. **Open Scene**: Double-click `Assets/Scenes/Main.unity`.

## How to Play

1. Press **Play** in the Unity Editor.
2. **Patients** will spawn and move to facilities (PHC or Private Hospital).
3. **UI Controls**:
   - Toggle "Awareness Campaign" to influence patient behavior.
   - (Future) Adjust staff levels.
4. **KPIs**: Monitor ATT (Average Treatment Time) and ACPE (Average Cost Per Episode) in the top HUD.

## Research Mode

- Data is automatically logged to `Application.persistentDataPath/research_runs/`.
- Logs include patient choices, wait times, and costs in CSV format.
- To configure experiments, modify `Assets/Scripts/Research/ExperimentManager.cs` or load a config JSON (implementation ready).

## Folder Structure

- `Assets/Scripts/`: Core game logic (`GameClock`, `PatientController`, `Facility`).
- `Assets/Scripts/Research/`: Data logging and experiment management.
- `Assets/Editor/`: Automation tools for project setup.
- `Assets/Prefabs/`: Generated patient and facility objects.

## Troubleshooting

- If the scene looks empty, ensure you ran **Tools > Setup Project**.
- If TextMeshPro is missing, import "TMP Essentials" when prompted by Unity.
