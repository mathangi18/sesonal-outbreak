using UnityEngine;
using System.Collections.Generic;

public class HybridGameManager : MonoBehaviour
{
    public static HybridGameManager Instance;

    [Header("Game Settings")]
    public int totalPatients = 30;
    public float phcBaseCost = 100f;
    public float privateBaseCost = 2000f;
    public float teleCost = 200f;
    public float mobileUseCost = 500f;
    public float travelCostPerKm = 20f;

    [Header("State")]
    public bool isGameActive = false;
    public int patientsProcessed = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        isGameActive = true;
        Debug.Log("Game Started");
    }

    public void EndGame()
    {
        isGameActive = false;
        Debug.Log("Game Ended");
        // Trigger final analytics or research logging dump if needed
    }

    public void RegisterPatientCompletion()
    {
        patientsProcessed++;
        if (patientsProcessed >= totalPatients)
        {
            EndGame();
        }
    }
}
