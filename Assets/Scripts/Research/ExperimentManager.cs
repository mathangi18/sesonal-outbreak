using UnityEngine;

public class ExperimentManager : MonoBehaviour
{
    public static ExperimentManager Instance;

    [Header("Config")]
    public int seed = 42;
    public string arm = "A";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        Random.InitState(seed);
    }

    public void LoadConfig(string json)
    {
        // Parse JSON and set config
    }
}
