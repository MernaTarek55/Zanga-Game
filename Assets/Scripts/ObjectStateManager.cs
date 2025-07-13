using System.Collections.Generic;
using UnityEngine;

public class ObjectStateManager : MonoBehaviour
{
    public List<ObjectState> objectStates = new List<ObjectState>();
    public static ObjectStateManager Instance { get; private set; }
    private void Awake()
    {
        //AdjustCamera();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    private void Start()
    {
        if (objectStates.Count > 0)
        {
            foreach (var state in objectStates)
            {
                if (state.obj != null)
                {
                    state.initialPosition = state.obj.transform.localPosition;
                    state.initialScale = state.obj.transform.localScale;
                    state.initialRotation = state.obj.transform.localRotation;
                    state.initialActiveState = state.obj.activeSelf;
                }
            }
        }
    }


    public void ResetAllObjects()
    {
        PatternManager.SetAllPetalsIntensity(0f);
        foreach (var state in objectStates)
        {
            if (state.obj != null)
            {
                state.obj.transform.localPosition = state.initialPosition;
                state.obj.transform.localScale = state.initialScale;
                state.obj.transform.localRotation = state.initialRotation;
                state.obj.SetActive(state.initialActiveState);

                // Reset any additional components
                var rb = state.obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

            }
            if (state.obj.GetComponent<ClueInteractable>() != null)
            {
                var interactable = state.obj.GetComponent<ClueInteractable>();
                interactable.IsBlocked = state.isblocked;
            }
        }
    }
}