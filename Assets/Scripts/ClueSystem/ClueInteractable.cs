using DG.Tweening;
using UnityEngine;

public class ClueInteractable : InteractableBase
{
    public int clueID;
    public bool hasBeenSolved = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    public bool IsBlocked = false;
    private void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        ClueManager.Instance.RegisterClueObject(this);
        CheckIfBlocked();
    }
    public void CheckIfBlocked()
    {
        IsBlocked = false;

        Vector3 rayOrigin = transform.position;
        Vector3 direction = Vector3.back; // -Z axis

        float rayDistance = 10f; // Long enough to reach any blockers behind

        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, direction, rayDistance);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null) continue;
            if (hit.collider.gameObject == gameObject) continue;

            InteractableBase blocker = hit.collider.GetComponent<InteractableBase>();
            if (blocker != null && blocker.isInteractable)
            {
                IsBlocked = true;
                Debug.Log($"🔴 {name} is blocked by: {blocker.name}");
                break;
            }
        }
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = !IsBlocked;

        SetInteractable(!IsBlocked );
    }



    protected override void OnHold()
    {
        CheckIfBlocked();
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
        if (IsBlocked)
        {
            Vector3 directionToCamera = (Camera.main.transform.position - transform.position).normalized;
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directionToCamera);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider == null) continue;
                if (hit.collider.gameObject == gameObject) continue; // 👈 skip self

                InteractableBase blocker = hit.collider.GetComponent<InteractableBase>();
                if (blocker != null && blocker.isInteractable)
                {
                    Debug.Log("👀 Hint targeting blocker: " + blocker.name);
                    blocker.transform.DOKill();
                    blocker.transform.DOPunchScale(Vector3.one * 0.2f, 0.4f, 5, 1).SetLoops(2, LoopType.Yoyo);
                    break;
                }
            }


            return;
        }

        // Animate this object
        transform.DOKill(); // Stop old animations
        transform.DOPunchScale(Vector3.one * 0.2f, 0.4f, 5, 1)
            .SetLoops(2, LoopType.Yoyo);
    }

}