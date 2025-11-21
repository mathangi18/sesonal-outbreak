using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI patientsText;
    public TextMeshProUGUI attText;
    public TextMeshProUGUI acpeText;

    private void Update()
    {
        if (GameClock.Instance != null)
        {
            timeText.text = GameClock.Instance.GetFormattedTime();
        }

        if (HybridGameManager.Instance != null)
        {
            patientsText.text = $"Patients: {HybridGameManager.Instance.patientsProcessed}";
            // Placeholder for ATT/ACPE calculation
            attText.text = "ATT: 0h";
            acpeText.text = "ACPE: $0";
        }
    }

    // Button callbacks
    public void OnToggleAwareness(bool val)
    {
        MOManager.Instance.ToggleAwarenessCampaign(val);
    }

    public void OnToggleMobile(bool val)
    {
        MOManager.Instance.ToggleMobileUnit(val);
    }
}
