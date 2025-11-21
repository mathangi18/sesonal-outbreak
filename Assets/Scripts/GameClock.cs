using UnityEngine;
using System;

public class GameClock : MonoBehaviour
{
    public static GameClock Instance;

    [Header("Time Settings")]
    public float hourSeconds = 1.0f; // Realtime seconds per game hour
    
    [Header("Current Time")]
    public float gameHours = 0f;
    public int day = 0;
    public int hour = 0;

    public event Action<int> OnHourChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        gameHours += Time.deltaTime / hourSeconds;
        
        int currentTotalHours = Mathf.FloorToInt(gameHours);
        int currentDay = currentTotalHours / 24;
        int currentHour = currentTotalHours % 24;

        if (currentHour != hour)
        {
            hour = currentHour;
            OnHourChanged?.Invoke(hour);
        }

        if (currentDay != day)
        {
            day = currentDay;
        }
    }

    public string GetFormattedTime()
    {
        return $"Day {day + 1} {hour:D2}:00";
    }
}
