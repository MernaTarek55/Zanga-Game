using UnityEngine;

public class ClueInteractable : InteractableBase
{
    public int clueID;
    private bool hasBeenSolved = false;

    protected override void OnHold()
    {
        if (hasBeenSolved) return;

        int currentClueID = ClueManager.Instance.GetCurrentClueID();

        // Wrong clue – block interaction
        if (clueID != currentClueID)
        {
            Debug.Log($"❌ {name}: Not the current clue.");
            return;
        }

        // Correct clue interaction
        Debug.Log($"✅ {name}: Correct clue interacted!");
        ClueManager.Instance.OnClueSolved(clueID);
        hasBeenSolved = true;

        // Disable interaction so it can't be used again
        SetInteractable(false);
    }
}
