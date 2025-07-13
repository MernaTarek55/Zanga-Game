using UnityEngine;

[System.Serializable]
public class ObjectState
{
    public GameObject obj;
    [HideInInspector] public Vector3 initialPosition;
    [HideInInspector] public Vector3 initialScale;
    [HideInInspector] public Quaternion initialRotation;
    [HideInInspector] public bool initialActiveState;
    public bool isblocked = false;
}