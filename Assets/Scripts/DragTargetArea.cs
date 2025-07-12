
using UnityEngine;
using DG.Tweening;

public class DragTargetArea : MonoBehaviour
{
    public System.Action<InteractableBase> OnObjectSnapped;
    [SerializeField] LayerMask layer;
    private void OnTriggerEnter2D(Collider2D other)
    {
        var interactable = other.GetComponent<InteractableBase>();
        if (interactable != null && IsInTargetLayer(interactable.gameObject))
        {
            interactable.isInsideTargetArea = true;
            interactable.targetArea = this;

            // Register for the snap completion event
            interactable.OnSnapped += HandleObjectSnapped;
        }
    }
    private bool IsInTargetLayer(GameObject obj)
    {
        return ((1 << obj.layer) & layer.value) != 0;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        var interactable = other.GetComponent<InteractableBase>();
        if (interactable != null && IsInTargetLayer(interactable.gameObject))
        {
            interactable.isInsideTargetArea = false;

            // Unregister the event when object leaves
            interactable.OnSnapped -= HandleObjectSnapped;
        }
    }

    private void HandleObjectSnapped(InteractableBase interactable)
    {
        // This will be called via event instead of Update
        OnObjectSnapped?.Invoke(interactable);
    }
}

