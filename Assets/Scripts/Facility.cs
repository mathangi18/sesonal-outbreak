using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum FacilityType
{
    PHC,
    PrivateHospital,
    MobileUnit,
    Telemedicine
}

public class Facility : MonoBehaviour
{
    public FacilityType facilityType;
    public string facilityName;
    
    [Header("Stats")]
    public int staffUnits = 1;
    public float serviceRatePerHour = 6f; // Patients per hour per staff
    public float diagnosticDelayHours = 0.5f;
    public float baseCost = 0f;

    [Header("Queue")]
    public List<PatientController> queue = new List<PatientController>();
    public List<PatientController> currentlyServing = new List<PatientController>();

    [Header("Visuals")]
    public Transform queueStartPoint;
    public float queueSpacing = 0.5f;

    private void Update()
    {
        ProcessQueue();
        UpdateQueueVisuals();
    }

    private void ProcessQueue()
    {
        // Check if we have capacity
        int capacity = staffUnits;
        
        // Move from queue to serving if capacity allows
        while (currentlyServing.Count < capacity && queue.Count > 0)
        {
            PatientController patient = queue[0];
            queue.RemoveAt(0);
            currentlyServing.Add(patient);
            StartCoroutine(ServePatient(patient));
        }
    }

    private System.Collections.IEnumerator ServePatient(PatientController patient)
    {
        patient.SetState(PatientState.InService);
        
        // Calculate service time
        // Rate is per hour, so hours per patient is 1/Rate
        // We add diagnostic delay
        float serviceTimeHours = (1f / serviceRatePerHour) + diagnosticDelayHours;
        float serviceTimeSeconds = serviceTimeHours * GameClock.Instance.hourSeconds;

        yield return new WaitForSeconds(serviceTimeSeconds);

        currentlyServing.Remove(patient);
        patient.CompleteTreatment();
    }

    public void JoinQueue(PatientController patient)
    {
        if (!queue.Contains(patient) && !currentlyServing.Contains(patient))
        {
            queue.Add(patient);
            patient.SetState(PatientState.Waiting);
        }
    }

    private void UpdateQueueVisuals()
    {
        for (int i = 0; i < queue.Count; i++)
        {
            if (queue[i] != null)
            {
                // Simple line queue
                Vector3 targetPos = queueStartPoint.position + (Vector3.right * i * queueSpacing); // Or some direction
                // For a "cute" look, maybe spiral or random cluster? Let's stick to line for now but maybe jitter
                queue[i].targetPosition = targetPos;
            }
        }
    }

    public float GetEstimatedWaitTime()
    {
        // Simple heuristic: (QueueLength / (Staff * Rate))
        if (staffUnits <= 0) return 999f;
        return (queue.Count + currentlyServing.Count) / (staffUnits * serviceRatePerHour);
    }
}
