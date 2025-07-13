using DG.Tweening;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.GetChild(0).DOLocalMoveY(0.85f, 1).SetLoops(-1, LoopType.Yoyo);
        transform.GetChild(1).DOLocalMoveY(-0.05f, 0.8f).SetLoops(-1, LoopType.Yoyo);
    }

   
}
