using DG.Tweening;
using UnityEngine;

public class HandleOlive : MonoBehaviour
{
    [SerializeField] private GameObject Character;
    [SerializeField] private GameObject CharacterHead;
    [SerializeField] private Transform oliveTransform;


    public void PlayLvl2EndSeq()
    {
        
        Transform playerTransform = Character.transform;

        Sequence oliveSequence = DOTween.Sequence();

        oliveSequence.AppendInterval(1.5f);
        oliveSequence.Append(oliveTransform.DOLocalMoveY(oliveTransform.localPosition.y - 2, 1.5f));



        oliveSequence.Append(playerTransform.DOMoveX(playerTransform.position.x + (oliveTransform.position.x - playerTransform.position.x) - 1, 2f));



        oliveSequence.Append(CharacterHead.transform.DORotate(new Vector3(0, 0, 16f), 0.5f, RotateMode.Fast));
    }
}