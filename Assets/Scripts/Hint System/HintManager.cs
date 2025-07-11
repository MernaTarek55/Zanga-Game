using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HintManager : MonoBehaviour
{
    public static HintManager Instance;

    [Header("Settings")]
    public float idleThreshold = 10f; // Time in seconds before hint triggers
    public float hintCooldown = 2f; // Minimum time between hints

    [Header("Debug")]
    [SerializeField] private float idleTimer = 0f;
    [SerializeField] private bool hintTriggered = false;
    [SerializeField] private bool isEnabled = true;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (!isEnabled) return;

        // Check if current level is complete
        var currentLevel = GameManager.Instance.GetCurrentLevel();
        if (currentLevel == null ||
            ClueManager.Instance.currentClueIndex >= currentLevel.levelClues.Count)
        {
            isEnabled = false;
            return;
        }

        idleTimer += Time.deltaTime;

        if (!hintTriggered && idleTimer >= idleThreshold)
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

        var activeClues = GetValidActiveClues();

        if (activeClues.Count == 0)
        {
            Debug.Log("No active clues available for hint");
            return;
        }

        foreach (var clue in activeClues)
        {
            clue.PlayHintAnimation();
        }

        // Set cooldown before next hint can trigger
        idleTimer = idleThreshold - hintCooldown;
    }

    private List<ClueInteractable> GetValidActiveClues()
    {
        List<ClueInteractable> validClues = new List<ClueInteractable>();

        var currentLevel = GameManager.Instance.GetCurrentLevel();
        if (currentLevel == null) return validClues;

        // Get current clue data
        if (ClueManager.Instance.currentClueIndex >= currentLevel.levelClues.Count)
            return validClues;

        int[] validIDs = currentLevel.levelClues[ClueManager.Instance.currentClueIndex].validObjectIDs;

        // Filter clues that:
        // 1. Are in the current level
        // 2. Are valid for current clue step
        // 3. Are interactable
        // 4. Haven't been solved yet
        foreach (var clue in ClueManager.Instance.allClueObjects)
        {
            if (validIDs.Contains(clue.clueID) &&
                clue.isInteractable &&
                !clue.hasBeenSolved)
            {
                validClues.Add(clue);
            }
        }

        return validClues;
    }

    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
        if (!enabled)
        {
            idleTimer = 0f;
            hintTriggered = false;
        }
    }
}