using UnityEngine;
using UnityEngine.Events;

public class HintManager : MonoBehaviour
{
    public static HintManager Instance;

    public float idleThreshold = 10f; // seconds
    private float idleTimer = 0f;

    public UnityEvent onHintTriggered;

    private bool hintShown = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        idleTimer += Time.deltaTime;

        if (idleTimer >= idleThreshold && !hintShown)
        {
            ShowHint();
        }
    }

    public void ResetIdleTimer()
    {
        idleTimer = 0f;
        hintShown = false;
    }

    private void ShowHint()
    {
        hintShown = true;
        Debug.Log("💡 Hint triggered!");
        onHintTriggered?.Invoke(); // You can hook this to UI or voice hint, etc.
    }

}
