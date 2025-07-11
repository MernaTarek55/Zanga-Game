using DG.Tweening;
using UnityEngine;

public class ClueInteractable : InteractableBase
{
    public int clueID;
    public bool hasBeenSolved = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        ClueManager.Instance.RegisterClueObject(this);
    }

    protected override void OnHold()
    {
        if (hasBeenSolved || !isInteractable) return;

        if (ClueManager.Instance.GetCurrentClueID() != clueID)
        {
            // Wrong clue - give feedback
            transform.DOShakePosition(0.5f, strength: 0.1f);
            return;
        }

        // Correct clue
        hasBeenSolved = true;
        Debug.Log($"✅ {name}: Correct clue interacted!");
        ClueManager.Instance.OnClueSolved(clueID);

        // Visual feedback
        transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
    }

    public void ResetClue()
    {
        hasBeenSolved = false;
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        SetInteractable(false);
    }
    public void PlayHintAnimation()
    {
        transform.DOKill(); // Stop old animations

        // Wiggle / punch scale
        transform.DOPunchScale(Vector3.one * 0.2f, 0.4f, 5, 1)
            .SetLoops(2, LoopType.Yoyo);
    }
}