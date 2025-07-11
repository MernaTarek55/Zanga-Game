using DG.Tweening;
using UnityEngine;

public class ClueInteractable : InteractableBase
{
    public int clueID;
    private bool hasBeenSolved = false;
    private void Start()
    {
        ClueManager.Instance.RegisterClueObject(this);
    }
    public void PlayHintAnimation()
    {
        transform.DOKill(); // Stop old animations

        // Wiggle / punch scale
        transform.DOPunchScale(Vector3.one * 0.2f, 0.4f, 5, 1)
            .SetLoops(2, LoopType.Yoyo);
    }
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
