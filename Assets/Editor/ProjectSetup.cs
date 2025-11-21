#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class ProjectSetup : EditorWindow
{
    [MenuItem("Tools/Setup Project")]
    public static void Setup()
    {
        SetupDirectories();
        SetupScene();
        SetupPrefabs();
        Debug.Log("Project Setup Complete! Open 'Assets/Scenes/Main.unity'");
    }

    static void SetupDirectories()
    {
        if (!Directory.Exists("Assets/Scenes")) Directory.CreateDirectory("Assets/Scenes");
        if (!Directory.Exists("Assets/Prefabs")) Directory.CreateDirectory("Assets/Prefabs");
        AssetDatabase.Refresh();
    }

    static void SetupScene()
    {
        // Create new scene
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects, UnityEditor.SceneManagement.NewSceneMode.Single);
        
        // Create Managers
        GameObject managers = new GameObject("Managers");
        managers.AddComponent<GameClock>();
        managers.AddComponent<HybridGameManager>();
        managers.AddComponent<PatientSpawner>();
        managers.AddComponent<MOManager>();
        managers.AddComponent<ResearchLogger>();
        managers.AddComponent<ExperimentManager>();

        // Create UI
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Create UIManager
        GameObject uiManagerObj = new GameObject("UIManager");
        UIManager uiManager = uiManagerObj.AddComponent<UIManager>();
        uiManager.transform.SetParent(canvasObj.transform, false);

        // Create HUD Panel
        GameObject panel = CreatePanel("HUD", canvasObj.transform);
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.sizeDelta = new Vector2(0, 100);
        rt.anchoredPosition = new Vector2(0, 0);

        // Create Texts
        uiManager.timeText = CreateText("TimeText", "Day 1 08:00", panel.transform, new Vector2(10, -10));
        uiManager.patientsText = CreateText("PatientsText", "Patients: 0", panel.transform, new Vector2(200, -10));
        uiManager.attText = CreateText("ATTText", "ATT: 0h", panel.transform, new Vector2(400, -10));
        uiManager.acpeText = CreateText("ACPEText", "ACPE: $0", panel.transform, new Vector2(600, -10));

        // Create Buttons
        CreateButton("ToggleAwareness", "Awareness Campaign", panel.transform, new Vector2(10, -60), () => uiManager.OnToggleAwareness(true));
        
        // Create EventSystem
        new GameObject("EventSystem").AddComponent<UnityEngine.EventSystems.EventSystem>().gameObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        // Create Facilities
        CreateFacility("PHC", new Vector3(-5, 0, 0), FacilityType.PHC);
        CreateFacility("PrivateHospital", new Vector3(5, 0, 0), FacilityType.PrivateHospital);

        // Save Scene
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, "Assets/Scenes/Main.unity");
    }

    static void SetupPrefabs()
    {
        // Patient Prefab
        GameObject patientGO = new GameObject("Patient");
        patientGO.AddComponent<SpriteRenderer>().color = new Color(0.4f, 0.8f, 0.9f); // Pastel Blue
        patientGO.AddComponent<CircleCollider2D>();
        patientGO.AddComponent<PatientController>();
        patientGO.AddComponent<PatientVisual>().body = patientGO.transform;
        
        // Save as prefab
        PrefabUtility.SaveAsPrefabAsset(patientGO, "Assets/Prefabs/Patient.prefab");
        GameObject.DestroyImmediate(patientGO);

        // Assign prefab to Spawner
        PatientSpawner spawner = GameObject.FindObjectOfType<PatientSpawner>();
        if (spawner != null)
        {
            spawner.patientPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Patient.prefab");
            // Create spawn points
            GameObject spawnPoint = new GameObject("SpawnPoint");
            spawner.spawnPoints = new Transform[] { spawnPoint.transform };
        }
    }

    static GameObject CreatePanel(string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.8f);
        return go;
    }

    static TextMeshProUGUI CreateText(string name, string content, Transform parent, Vector2 pos)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        TextMeshProUGUI txt = go.AddComponent<TextMeshProUGUI>();
        txt.text = content;
        txt.fontSize = 24;
        txt.color = Color.black;
        go.GetComponent<RectTransform>().anchoredPosition = pos;
        return txt;
    }

    static void CreateButton(string name, string label, Transform parent, Vector2 pos, UnityEngine.Events.UnityAction action)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.color = new Color(0.9f, 0.9f, 0.9f);
        Button btn = go.AddComponent<Button>();
        // Note: Can't easily assign persistent listeners in Editor script without more complex serialization
        // But we can set up the structure
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(go.transform, false);
        TextMeshProUGUI txt = textObj.AddComponent<TextMeshProUGUI>();
        txt.text = label;
        txt.fontSize = 18;
        txt.color = Color.black;
        txt.alignment = TextAlignmentOptions.Center;
        
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, 40);
        rt.anchoredPosition = pos;
        
        RectTransform textRT = textObj.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
    }

    static void CreateFacility(string name, Vector3 pos, FacilityType type)
    {
        GameObject go = new GameObject(name);
        go.transform.position = pos;
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.color = type == FacilityType.PHC ? new Color(0.4f, 0.9f, 0.6f) : new Color(0.9f, 0.6f, 0.6f); // Pastel Green/Red
        go.transform.localScale = new Vector3(2, 2, 1);
        
        Facility f = go.AddComponent<Facility>();
        f.facilityType = type;
        f.facilityName = name;
        
        GameObject queueStart = new GameObject("QueueStart");
        queueStart.transform.SetParent(go.transform, false);
        queueStart.transform.localPosition = new Vector3(-1.5f, -1, 0);
        f.queueStartPoint = queueStart.transform;
    }
}
#endif
