using UnityEngine;
using UnityEngine.UI;

public class ParetoCalculator : MonoBehaviour
{
    public RectTransform graphContainer;
    public GameObject dotPrefab;

    public void AddDataPoint(float att, float acpe)
    {
        GameObject dot = Instantiate(dotPrefab, graphContainer);
        // Position dot based on ATT (x) and ACPE (y)
        // Normalize to graph size
        // This is a placeholder implementation
        Rect rect = graphContainer.rect;
        float x = (att / 24f) * rect.width; // Assuming max 24h ATT
        float y = (acpe / 5000f) * rect.height; // Assuming max $5000 ACPE
        
        dot.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
    }
}
