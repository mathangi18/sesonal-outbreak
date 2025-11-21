# Patient Prefab Missing Issue Report

## Actions Taken
1.  **Backup**: Created branch `ag/repair-auto` and committed current state.
2.  **Analysis**:
    *   Identified `Patient.prefab` GUID: `367b071be807d754bbb34905a2377bda`.
    *   Attempted to locate `patientPrefab` assignment in `Main.unity` but could not find the explicit field name. This suggests it might be serialized under a different name (e.g., `_patientPrefab` or just a fileID reference in a list) or the component is missing the reference entirely.
3.  **Instrumentation**:
    *   Added debug logging to `PatientSpawner.cs` (`Awake`, `Start`, `Update`) to track the `patientPrefab` field.
    *   This will log `[PS DEBUG]` messages to the Console to help pinpoint when/if the prefab is assigned or cleared.

## Next Steps
1.  **Run Repair Tool**: In Unity, go to **Tools > Fix Spawn System (Safe)**. This script (`SpawnFixer.cs`) has logic to reassign the prefab if it's missing.
2.  **Check Console**: Play the game and look for `[PS DEBUG]` logs.
    *   If it says `prefab=null` in `Awake`, the reference is missing in the Scene/Prefab.
    *   If it says `prefab=Patient` in `Awake` but then `patientPrefab changed...` later, some code is clearing it.
3.  **Manual Fix**: If the tool doesn't work, select the `SpawnPoint` object in the Scene and manually drag `Assets/Prefabs/Patient.prefab` into the `Patient Prefab` slot of the `PatientSpawner` component.
