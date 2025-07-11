using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using ArabicSupport;

public class ClueManager : MonoBehaviour
{
    public static ClueManager Instance;
    [SerializeField]TextMeshProUGUI clueTextUI;
    public ClueData[] allClues;
    public int currentClueIndex = 0;

    private List<ClueInteractable> allClueObjects = new List<ClueInteractable>();

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

    public void ShowCurrentClue()
    {
        int[] validIDs = allClues[currentClueIndex].validObjectIDs;
        clueTextUI.text = ArabicFixer.Fix(allClues[currentClueIndex].clueText);
        EnableValidObjects(validIDs);
    }

    public int GetCurrentClueID()
    {
        return allClues[currentClueIndex].clueID;
    }

    public void EnableValidObjects(int[] validIDs)
    {
        foreach (var clue in allClueObjects)
        {
            if (validIDs.Contains(clue.clueID))
                clue.SetInteractable(true);
            else
                clue.SetInteractable(false);
        }
    }

    public void OnClueSolved(int solvedID)
    {
        Debug.Log($"✅ Clue {solvedID} solved!");
        currentClueIndex++;

        if (currentClueIndex < allClues.Length)
        {
            ShowCurrentClue();
        }
        else
        {
            Debug.Log("🎉 All clues solved!");
        }
    }

    public List<ClueInteractable> GetCurrentActiveClueObjects()
    {
        int currentID = GetCurrentClueID();
        return allClueObjects
            .Where(c => c.clueID == currentID && c.isInteractable)
            .ToList();
    }
    public bool AreAllCluesSolved()
    {
        return currentClueIndex >= allClues.Length;
    }

}
