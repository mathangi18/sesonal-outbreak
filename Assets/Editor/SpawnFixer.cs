#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public class SpawnFixer : EditorWindow
{
    [MenuItem("Tools/Fix Spawn System")]
    public static void FixSpawnSystem()
    {
        Debug.Log("Starting Safe Spawn System Fix...");

        // 1. Fix Patient Prefab (Non-destructive)
        string prefabPath = "Assets/Prefabs/Patient.prefab";
        GameObject patientPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (patientPrefab == null)
        {
            Debug.LogWarning($"Patient prefab not found at {prefabPath}. Creating a new one.");
            GameObject go = new GameObject("Patient");
            patientPrefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            DestroyImmediate(go);
        }

        using (var editScope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
        {
            GameObject prefabRoot = editScope.prefabContentsRoot;
            
            if (!prefabRoot.GetComponent<PatientController>()) 
            {
                Debug.Log("Adding missing PatientController to prefab.");
                prefabRoot.AddComponent<PatientController>();
            }
            
            if (!prefabRoot.GetComponent<PatientVisual>()) 
            {
                Debug.Log("Adding missing PatientVisual to prefab.");
                prefabRoot.AddComponent<PatientVisual>();
            }
            
            if (!prefabRoot.GetComponent<SpriteRenderer>())
            {
                Debug.Log("Adding missing SpriteRenderer to prefab.");
                SpriteRenderer sr = prefabRoot.AddComponent<SpriteRenderer>();
                sr.color = new Color(0.4f, 0.8f, 0.9f);
                Sprite knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
                if (knob != null) sr.sprite = knob;
            }
            
            PatientVisual visual = prefabRoot.GetComponent<PatientVisual>();
            if (visual != null && visual.body == null) 
            {
                visual.body = prefabRoot.transform;
                Debug.Log("Assigned PatientVisual body reference.");
            }
        }

        // 2. Fix Scene (Non-destructive)
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");
        
        GameObject spawnPointGO = GameObject.Find("SpawnPoint");
        if (spawnPointGO == null)
        {
            Debug.Log("Creating missing SpawnPoint GameObject.");
            spawnPointGO = new GameObject("SpawnPoint");
            spawnPointGO.transform.position = Vector3.zero;
        }

        // Create Spawn Positions only if missing
        Transform[] spawnPointsArray = new Transform[3];
        spawnPointsArray[0] = EnsureChild(spawnPointGO.transform, "SpawnPos1", new Vector3(0, 0, 0));
        spawnPointsArray[1] = EnsureChild(spawnPointGO.transform, "SpawnPos2", new Vector3(2, 0, 0));
        spawnPointsArray[2] = EnsureChild(spawnPointGO.transform, "SpawnPos3", new Vector3(-2, 0, 0));

        // 3. Configure Spawner (Force Refresh to fix Serialization)
        PatientSpawner spawner = spawnPointGO.GetComponent<PatientSpawner>();
        if (spawner != null)
        {
            Debug.Log("Removing old PatientSpawner component to clear serialization mismatch.");
            DestroyImmediate(spawner);
        }
        
        Debug.Log("Adding fresh PatientSpawner component.");
        spawner = spawnPointGO.AddComponent<PatientSpawner>();
        
        // Assign references
        spawner.patientPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        Debug.Log("Assigned Patient Prefab to Spawner.");

        spawner.spawnPoints = new List<Transform>(spawnPointsArray);
        Debug.Log("Assigned Spawn Points List to Spawner.");

        spawner.spawnRate = 20f;
        Debug.Log("Set Spawn Rate to 20.");

        // 4. Validation Test
        bool testSuccess = false;
        try 
        {
            if (spawner.patientPrefab != null && spawner.spawnPoints != null && spawner.spawnPoints.Count > 0 && spawner.spawnPoints[0] != null)
            {
                // Instantiate in Editor
                GameObject testObj = PrefabUtility.InstantiatePrefab(spawner.patientPrefab, spawner.spawnPoints[0]) as GameObject;
                if (testObj != null)
                {
                    // Verify components
                    if (testObj.GetComponent<PatientController>() != null && testObj.GetComponent<PatientVisual>() != null)
                    {
                        testSuccess = true;
                    }
                    DestroyImmediate(testObj);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Test Spawn Failed: " + e.Message);
        }

        EditorUtility.SetDirty(spawnPointGO);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.Refresh();

        Debug.Log($"SUMMARY:\n- Patient prefab assigned: {(spawner.patientPrefab != null ? "Yes" : "No")}\n- Spawn points count: {(spawner.spawnPoints != null ? spawner.spawnPoints.Count : 0)}\n- Test spawn instantiation success: {testSuccess}");
    }

    static Transform EnsureChild(Transform parent, string name, Vector3 localPos)
    {
        Transform child = parent.Find(name);
        if (child == null)
        {
            Debug.Log($"Creating missing child: {name}");
            GameObject go = new GameObject(name);
            child = go.transform;
            child.SetParent(parent, false);
            child.localPosition = localPos;
        }
        child.localPosition = localPos; // Ensure position is correct even if exists
        return child;
    }
}
#endif
