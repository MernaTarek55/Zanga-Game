using UnityEngine;
using System.Collections.Generic;

public class HintManager : MonoBehaviour
{
    public static HintManager Instance;

    public float idleThreshold = 10f;
    [SerializeField] float idleTimer = 0f;
    [SerializeField] bool hintTriggered = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private void Update()
    {
        if (ClueManager.Instance.AreAllCluesSolved())
        {
            enabled = false;
            return;
        }
        idleTimer += Time.deltaTime;

        if (idleTimer >= idleThreshold)
        {
            TriggerHint();
        }
    }

    public void ResetIdleTimer()
    {
        idleTimer = 0f;
        hintTriggered = false;
    }

    private void TriggerHint()
    {
        hintTriggered = true;
        Debug.Log("💡 Triggering hint...");

        var activeClues = ClueManager.Instance.GetCurrentActiveClueObjects();

        foreach (var clue in activeClues)
        {
            clue.PlayHintAnimation();
        }
        idleTimer = idleThreshold - 2;
    }

}
