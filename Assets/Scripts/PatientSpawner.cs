using UnityEngine;
using System.Collections.Generic;

public class PatientSpawner : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject patientPrefab;
    public List<Transform> spawnPoints = new List<Transform>();
    public float spawnRate = 20f; // Patients per game hour

    private float nextSpawnTime = 0f;

    // TEMP DEBUG - remove after investigation
    private GameObject _prevPrefab;
    void Awake() {
        _prevPrefab = patientPrefab;
        Debug.Log($"[PS DEBUG] Awake on '{gameObject.name}' prefab={patientPrefab}", this);
    }
    void Start() {
        Debug.Log($"[PS DEBUG] Start on '{gameObject.name}' prefab={patientPrefab}", this);
    }

    private void Update()
    {
        if (_prevPrefab != patientPrefab) {
            Debug.LogError($"[PS DEBUG] patientPrefab changed for '{gameObject.name}': now={patientPrefab}\nStack:\n{new System.Diagnostics.StackTrace(true)}", this);
            _prevPrefab = patientPrefab;
        }

        // Defensive check for Managers
        if (HybridGameManager.Instance == null || !HybridGameManager.Instance.isGameActive) return;
        if (GameClock.Instance == null) return;

        if (GameClock.Instance.gameHours >= nextSpawnTime)
        {
            SpawnPatient();
            
            // Schedule next spawn
            float rate = spawnRate > 0 ? spawnRate : 1f; // Prevent div by zero
            float interval = 1f / rate;
            interval *= Random.Range(0.8f, 1.2f);
            nextSpawnTime = GameClock.Instance.gameHours + interval;
        }
    }

    private void SpawnPatient()
    {
        // 1. Prefab Check
        if (patientPrefab == null)
        {
            Debug.LogWarning("PatientSpawner: Patient Prefab is missing! Cannot spawn.");
            return;
        }

        // 2. Spawn Points Check
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("PatientSpawner: No spawn points assigned! Cannot spawn.");
            return;
        }
        
        // 3. Find Valid Spawn Point
        Transform spawnPoint = null;
        foreach (var sp in spawnPoints)
        {
            if (sp != null)
            {
                spawnPoint = sp;
                break;
            }
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("PatientSpawner: All assigned spawn points are null/destroyed!");
            return;
        }

        // 4. Instantiate Safely
        GameObject p = Instantiate(patientPrefab, spawnPoint.position, Quaternion.identity);
        
        // 5. Defensive Component Checks (Self-Healing)
        PatientController pc = p.GetComponent<PatientController>();
        if (pc == null)
        {
            Debug.LogWarning("PatientSpawner: Spawned patient missing PatientController. Auto-adding it.");
            pc = p.AddComponent<PatientController>();
        }
        
        // Assign ID safely
        if (HybridGameManager.Instance != null)
        {
            pc.patientId = HybridGameManager.Instance.patientsProcessed + 1;
        }

        // Visual checks
        PatientVisual visual = p.GetComponent<PatientVisual>();
        if (visual == null)
        {
            Debug.LogWarning("PatientSpawner: Spawned patient missing PatientVisual. Auto-adding it.");
            visual = p.AddComponent<PatientVisual>();
        }

        if (visual.body == null)
        {
            visual.body = p.transform; // Fallback to self
        }
    }
}
