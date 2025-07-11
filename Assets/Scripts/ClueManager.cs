using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using ArabicSupport;

public class ClueManager : MonoBehaviour
{
    public static ClueManager Instance;
    public TextMeshProUGUI clueTextUI;

    public ClueData[] allClues; // Your clue scriptable objects
    public int currentClueIndex = 0;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        ShowCurrentClue();
    }
    public int GetCurrentClueID()
    {
        return allClues[currentClueIndex].clueID;
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
            // Trigger level complete here
        }
    }

    // ✅ THIS FIXES YOUR ERROR
    public void ShowCurrentClue()
    {
        ClueData currentClue = allClues[currentClueIndex];

        clueTextUI.DOFade(0, 0f);
        clueTextUI.text = ArabicFixer.Fix(currentClue.clueText);
        clueTextUI.DOFade(1, 0.5f);

        EnableValidObjects(currentClue.validObjectIDs);
    }


    // This finds all ClueInteractables and enables only the correct ones
    public void EnableValidObjects(int[] validIDs)
    {
        ClueInteractable[] all = FindObjectsOfType<ClueInteractable>();

        foreach (var obj in all)
        {
            if (validIDs.Contains(obj.clueID))
                obj.SetInteractable(true);
            else
                obj.SetInteractable(false);
        }
    }
}
