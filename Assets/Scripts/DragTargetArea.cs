using UnityEngine;

public class DragTargetArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var interactable = other.GetComponent<InteractableBase>();
        if (interactable != null)
        {
            interactable.isInsideTargetArea = true;
            interactable.targetArea = this;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var interactable = other.GetComponent<InteractableBase>();
        if (interactable != null)
        {
            interactable.isInsideTargetArea = false;
        }
    }
}
