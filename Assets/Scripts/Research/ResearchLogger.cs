using UnityEngine;
using System.IO;
using System.Text;

public class ResearchLogger : MonoBehaviour
{
    public static ResearchLogger Instance;
    private string logPath;
    private StringBuilder csvBuffer = new StringBuilder();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        string dir = Path.Combine(Application.persistentDataPath, "research_runs");
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        
        logPath = Path.Combine(dir, $"run_{System.DateTime.UtcNow.Ticks}.csv");
        csvBuffer.AppendLine("participant_id,arm,patient_index,choice,tele,mobile,travel_time,wait_time,diag_delay,ATT,ACPE,PHC_staff,Private_staff,mobile_active,timestamp_utc,gamehours");
    }

    public void LogPatient(int id, string choice, float att, float acpe)
    {
        // Simplified log entry
        string line = $"user,A,{id},{choice},0,0,0,0,0,{att},{acpe},1,5,0,{System.DateTime.UtcNow},{GameClock.Instance.gameHours}";
        csvBuffer.AppendLine(line);
    }

    private void OnApplicationQuit()
    {
        File.WriteAllText(logPath, csvBuffer.ToString());
        Debug.Log($"Log saved to {logPath}");
    }
}
