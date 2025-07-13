using UnityEngine;
using DG.Tweening;
using System;

[Flags]
public enum InteractionType
{
    None = 0,
    Tap = 1 << 0,
    Hold = 1 << 1,
    Drag = 1 << 2
}

public class InteractableBase : MonoBehaviour
{
    public int clueID;
    public bool isInteractable = true;
    public InteractionType interactions;

    private bool hasScaled = false;
    private Vector3 offset;
    private bool isDragging = false;
    [HideInInspector] public bool isInsideTargetArea = false;
    [HideInInspector] public DragTargetArea targetArea;
    public Transform targetSnapPosition;
    public System.Action<InteractableBase> OnSnapped;
    public bool hasBeenSolved = false;
    [SerializeField] protected GameObject tree;
    private void OnMouseDown()
    {
        HintManager.Instance.ResetIdleTimer();
        if (!isInteractable) return;

        if (interactions.HasFlag(InteractionType.Tap))
            OnTap();

        if (interactions.HasFlag(InteractionType.Drag))
        {
            offset = transform.position - GetMouseWorldPos();
            isDragging = true;
        }
        
    }

    private void OnMouseDrag()
    {
        if (!isInteractable || !interactions.HasFlag(InteractionType.Drag) || !isDragging) return;

        transform.position = GetMouseWorldPos() + offset;
    }

    private void OnMouseUp()
    {
        isDragging = false;
        if (interactions.HasFlag(InteractionType.Drag) && isInsideTargetArea && targetSnapPosition != null)
        {
            transform.DOMove(targetSnapPosition.position, 0.4f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => {
                    // Trigger the snap event
                    OnSnapped?.Invoke(this);
                    HandleSnapComplete();
                });
        }
        foreach (var clue in FindObjectsOfType<ClueInteractable>())
        {
                if (clue.IsBlocked)
                {
                clue.CheckIfBlocked();
                    if(!clue.IsBlocked)
                    {
                    transform.DOScale(0, 0.4f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => {
                // Trigger the snap event
                this.gameObject.SetActive(false);
            });
                }
                }
        }
    }
    protected virtual void HandleSnapComplete()
    {
        isInteractable = false;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Debug.Log("✅ Object snapped: " + gameObject.name);
        if (sr != null)
        {
            transform.DOScale(Vector3.one, 0.5f)
    .SetEase(Ease.OutBack);

            sr.DOColor(Color.white, 0.5f)
                .SetEase(Ease.Linear)
                .OnComplete(MakeObjectAppear);
        }

    }
    void MakeObjectAppear()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        if(tree == null) return;
        //if (tree.GetComponent<InteractableBase>() == null)
        //{

        //Vector3 pos = new Vector3(tree.transform.position.x , tree.transform.position.y+10 , tree.transform.position.z);
        //tree.transform.DOMove(pos  , 1.5f).SetEase(Ease.OutCubic);
        //}

    }
    private void OnMouseOver()
    {
        HintManager.Instance.ResetIdleTimer(); // Reset idle timer when mouse is over interactable
        if (!isInteractable || !interactions.HasFlag(InteractionType.Hold)) return;

        if (Input.GetMouseButton(0))
        {
            HandleHoldOnce();
            OnHold();
        }
    }
    public virtual void ResetClue()
    {
        SetInteractable(false);
    }
    private void HandleHoldOnce()
    {
        if (!hasScaled)
        {
            hasScaled = true;
            transform.DOPunchScale(Vector3.one * 0.1f, 0.3f);
        }
    }

    public virtual void SetInteractable(bool state)
    {
        isInteractable = state;
    }

    public void ResetInteraction()
    {
        hasScaled = false;
    }

    protected virtual void OnHold() { }

    public virtual void OnTap()
    {
        Debug.Log($"{gameObject.name} was tapped!");
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    public virtual void PlayHintAnimation() { }
}