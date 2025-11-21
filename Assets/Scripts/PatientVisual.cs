using UnityEngine;

public class PatientVisual : MonoBehaviour
{
    public Transform body;
    public SpriteRenderer eyes;
    public Sprite openEyes; // Assign in editor or load
    public Sprite closedEyes; // Assign in editor or load

    private bool isWalking = false;
    private float bounceTimer = 0f;
    private float blinkTimer = 0f;

    private void Update()
    {
        HandleBounce();
        HandleBlink();
    }

    public void SetWalking(bool walking)
    {
        isWalking = walking;
    }

    private void HandleBounce()
    {
        if (isWalking)
        {
            bounceTimer += Time.deltaTime * 10f;
            float yOffset = Mathf.Abs(Mathf.Sin(bounceTimer)) * 0.1f;
            body.localPosition = new Vector3(0, yOffset, 0);
        }
        else
        {
            body.localPosition = Vector3.Lerp(body.localPosition, Vector3.zero, Time.deltaTime * 5f);
        }
    }

    private void HandleBlink()
    {
        blinkTimer -= Time.deltaTime;
        if (blinkTimer <= 0)
        {
            // Blink
            if (eyes != null) eyes.color = Color.gray; // Simple blink effect if no sprite
            Invoke("ResetBlink", 0.1f);
            blinkTimer = Random.Range(2f, 5f);
        }
    }

    private void ResetBlink()
    {
        if (eyes != null) eyes.color = Color.black; // Restore
    }
}
