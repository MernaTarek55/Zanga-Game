using UnityEngine;
using DG.Tweening; // Make sure you have this using directive

public class RotateClock : MonoBehaviour
{
    [SerializeField] GameObject clock;
    [SerializeField] int sec;

    private void Start()
    {
        // Rotate the clock 360 degrees on the Z axis every 1 second, loop infinitely
        clock.transform.DORotate(new Vector3(0, 0, -360), sec, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1);
    }
}
