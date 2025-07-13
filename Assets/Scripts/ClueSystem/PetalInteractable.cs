using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PetalInteractable : InteractableBase
{
    public int supClueID;
    //public bool hasBeenSolved = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    public bool IsBlocked = false;

    public Action<int> onPetalPressed;


    protected virtual void Start()
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

        SetInteractable(!IsBlocked);
    }



    public override void OnTap()
    {
        if (isInteractable == false) return;
        Light2D light = GetComponentInChildren<Light2D>();
        if (light != null)
        {
            light.intensity = 1f;
        }
        int returnVal = PatternManager.Instance.OnPetalPressed(supClueID);
        if (returnVal == 0)
        {
            light.intensity = 0f;
            AudioManager.Instance.PlaySound(SoundType.WrongInteraction);
        }
        else { AudioManager.Instance.PlaySound(SoundType.correctInteraction); }
            transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
        return;
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