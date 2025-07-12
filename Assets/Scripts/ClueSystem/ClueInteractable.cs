
using DG.Tweening;
using UnityEngine;

public class ClueInteractable : InteractableBase
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    public bool IsBlocked = false;
    [SerializeField] private GameObject rock;
    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        ClueManager.Instance.RegisterClueObject(this);
        CheckIfBlocked();
    }

    protected override void HandleSnapComplete()
    {
        base.HandleSnapComplete(); // Do the base visual feedback

        // Clue-specific handling
        if (ClueManager.Instance.GetCurrentClueID() == clueID)
        {
            GameManager.Instance.levels[GameManager.Instance.currentLevelIndex]
                .levelClues[ClueManager.Instance.currentClueIndex].clueSteps++;

            if (GameManager.Instance.levels[GameManager.Instance.currentLevelIndex]
                .levelClues[ClueManager.Instance.currentClueIndex].clueSteps >=
                GameManager.Instance.levels[GameManager.Instance.currentLevelIndex]
                .levelClues[ClueManager.Instance.currentClueIndex].clueMaxSteps)
            {
                if (tree != null) tree.SetActive(true);
                else
                {
                    transform.DOMove(new Vector3(6f, 1f, 0f), 2f)
                        .SetEase(Ease.OutBack)
                        .OnComplete(() =>
                        {
                            transform.rotation = originalRotation;
                            SetInteractable(false);
                        });
                }
                    ClueManager.Instance.OnClueSolved(clueID);
            }
        }
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
        GameManager.Instance.levels[GameManager.Instance.currentLevelIndex]
            .levelClues[ClueManager.Instance.currentClueIndex].clueSteps++;
        if (GameManager.Instance.levels[GameManager.Instance.currentLevelIndex]
            .levelClues[ClueManager.Instance.currentClueIndex].clueMaxSteps == GameManager.Instance.levels[GameManager.Instance.currentLevelIndex]
            .levelClues[ClueManager.Instance.currentClueIndex].clueSteps)
        {
            if (clueID == 11)
            {
                gameObject.SetActive(false);
                rock.SetActive(true);
                ClueManager.Instance.OnClueSolved(clueID);
            }
        }

        // Visual feedback
        transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
    }

    public virtual void ResetClue()
    {
        hasBeenSolved = false;
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        SetInteractable(false);
    }
    public override void PlayHintAnimation()
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