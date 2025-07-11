using DG.Tweening;
using UnityEngine;

public class GeneralInteractable : InteractableBase
{
    protected override void OnHold()
    {
        if (isInteractable)
        {
            Debug.Log("✅ General object interacted!");
            // You can add animations, sounds, effects, etc.
            transform.DOPunchScale(Vector3.one * 0.1f, 0.3f);
        }
    }
}
