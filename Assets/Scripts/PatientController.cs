using UnityEngine;
using System.Collections;

public enum PatientState
{
    Idle,
    MovingToFacility,
    Waiting,
    InService,
    Completed
}

public class PatientController : MonoBehaviour
{
    [Header("Identity")]
    public int patientId;
    public string patientName;

    [Header("State")]
    public PatientState currentState = PatientState.Idle;
    public Facility targetFacility;
    public Vector3 targetPosition;
    
    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Data")]
    public float spawnTime;
    public float arrivalTime;
    public float serviceStartTime;
    public float completionTime;
    public bool isCured = false; // ATT success?

    private PatientVisual visual;

    private void Awake()
    {
        visual = GetComponent<PatientVisual>();
    }

    private void Start()
    {
        spawnTime = GameClock.Instance.gameHours;
        ChooseFacility();
    }

    private void Update()
    {
        if (currentState == PatientState.MovingToFacility || currentState == PatientState.Waiting)
        {
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            visual?.SetWalking(true);
        }
        else
        {
            visual?.SetWalking(false);
            if (currentState == PatientState.MovingToFacility)
            {
                // Arrived
                arrivalTime = GameClock.Instance.gameHours;
                if (targetFacility != null)
                {
                    targetFacility.JoinQueue(this);
                }
            }
        }
    }

    public void SetState(PatientState newState)
    {
        currentState = newState;
        // Update visuals based on state if needed
    }

    private void ChooseFacility()
    {
        // Logic to choose facility based on distance, cost, awareness, etc.
        // For now, random or find nearest.
        // We will let the Spawner or Manager assign this, or do it here.
        // Let's assume Spawner assigns it for now or we find one.
        
        // Simple logic: Find PHC
        Facility[] facilities = FindObjectsOfType<Facility>();
        if (facilities.Length > 0)
        {
            // Pick random for variety in this demo
            targetFacility = facilities[Random.Range(0, facilities.Length)];
            targetPosition = targetFacility.queueStartPoint.position; // Initial target
            currentState = PatientState.MovingToFacility;
        }
    }

    public void CompleteTreatment()
    {
        completionTime = GameClock.Instance.gameHours;
        currentState = PatientState.Completed;
        isCured = true; // Assume success for now
        
        // Notify Manager
        HybridGameManager.Instance.RegisterPatientCompletion();
        
        // Destroy or pool
        Destroy(gameObject, 1f);
    }
}
