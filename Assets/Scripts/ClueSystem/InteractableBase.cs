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

[RequireComponent(typeof(Collider2D))]
public class InteractableBase : MonoBehaviour
{
    public bool isInteractable = true;
    public InteractionType interactions; // 👈 هتختاري منها من Inspector

    private bool hasScaled = false;
    private Vector3 offset;
    private bool isDragging = false;

    private void OnMouseDown()
    {
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
    }

    private void OnMouseOver()
    {
        if (!isInteractable || !interactions.HasFlag(InteractionType.Hold)) return;

        if (Input.GetMouseButton(0))
        {
            HandleHoldOnce();
            OnHold();
        }
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
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = state ? Color.white : Color.gray;
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
}