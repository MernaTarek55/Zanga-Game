using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ClueManager : MonoBehaviour
{
    public static ClueManager Instance;

    public List<ClueInteractable> allClueObjects = new List<ClueInteractable>();
    public int currentClueIndex = 0;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        ShowCurrentClue();
    }

    public void RegisterClueObject(ClueInteractable clue)
    {
        if (!allClueObjects.Contains(clue))
        {
            allClueObjects.Add(clue);
        }
    }

    public void OnClueSolved(int solvedID)
    {
        Debug.Log($"✅ Clue {solvedID} solved!");
        currentClueIndex++;

        // Get current level's clues
        var currentLevel = GameManager.Instance.GetCurrentLevel();
        if (currentLevel == null) return;

        // Check if solved all clues for current level
        if (currentClueIndex >= currentLevel.levelClues.Count)
        {
            Debug.Log("🎉 Level completed!");
            HintManager.Instance.SetEnabled(false);
            GameManager.Instance.OnLevelCompleted();
        }
        else
        {
            ShowCurrentClue();
        }
    }

    public void ShowCurrentClue()
    {
        var currentLevel = GameManager.Instance.GetCurrentLevel();
        if (currentLevel == null || currentClueIndex >= currentLevel.levelClues.Count)
        {
            Debug.LogWarning("No valid clue to show");
            return;
        }

        // Show current clue text
        GameManager.Instance.ShowNextVerse(currentLevel.levelClues[currentClueIndex].clueText);

        // Enable valid objects for this clue
        EnableValidObjects(currentLevel.levelClues[currentClueIndex].validObjectIDs);
    }

    private void EnableValidObjects(int[] validIDs)
    {
        foreach (var clue in allClueObjects)
        {
            bool shouldBeActive = validIDs.Contains(clue.clueID) &&
                                IsClueInCurrentLevel(clue.clueID);
            clue.SetInteractable(shouldBeActive);
        }
    }

    private bool IsClueInCurrentLevel(int clueID)
    {
        var currentLevel = GameManager.Instance.GetCurrentLevel();
        if (currentLevel == null) return false;

        return currentLevel.levelClues.Any(c => c.clueID == clueID);
    }

    public void ResetForNewLevel()
    {
        currentClueIndex = 0;
        ShowCurrentClue();
    }

    public void ResetAllClues()
    {
        currentClueIndex = 0;
        foreach (var clue in allClueObjects)
        {
            clue.ResetClue();
        }
        ShowCurrentClue();
    }

    public int GetCurrentClueID()
    {
        var currentLevel = GameManager.Instance.GetCurrentLevel();
        if (currentLevel == null || currentClueIndex >= currentLevel.levelClues.Count)
            return -1;

        return currentLevel.levelClues[currentClueIndex].clueID;
    }
}