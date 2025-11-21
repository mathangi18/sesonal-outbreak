using UnityEngine;

public class MOManager : MonoBehaviour
{
    public static MOManager Instance;

    public bool awarenessCampaignActive = false;
    public bool mobileUnitActive = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void ToggleAwarenessCampaign(bool active)
    {
        awarenessCampaignActive = active;
        Debug.Log($"Awareness Campaign: {active}");
        // Logic to influence patient choice would go here
    }

    public void ToggleMobileUnit(bool active)
    {
        mobileUnitActive = active;
        Debug.Log($"Mobile Unit: {active}");
        // Logic to enable/disable mobile unit facility
        Facility mobile = FindObjectOfType<Facility>(); // Need better way to find specific facility
        // For now, just log
    }

    public void AddStaffToPHC()
    {
        // Find PHC and add staff
        Debug.Log("Staff added to PHC");
    }

    public void RemoveStaffFromPHC()
    {
        // Find PHC and remove staff
        Debug.Log("Staff removed from PHC");
    }
}
