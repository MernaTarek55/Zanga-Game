using DG.Tweening;
using UnityEngine;

public class HandleOlive : MonoBehaviour
{
    [SerializeField] private GameObject Character;
    [SerializeField] private GameObject CharacterHead;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("hole"))
        {
            this.GetComponent<Collider2D>().enabled = false;
            Transform oliveTransform = this.transform;
            Transform playerTransform = Character.transform;

            Sequence oliveSequence = DOTween.Sequence();


            oliveSequence.Append(oliveTransform.DOLocalMoveY(oliveTransform.localPosition.y - 2, 2));



            oliveSequence.Append(playerTransform.DOMoveX(playerTransform.position.x + (oliveTransform.position.x - playerTransform.position.x) - 1, 3f));



            oliveSequence.Append(CharacterHead.transform.DORotate(new Vector3(0, 0, 16f), 0.5f, RotateMode.Fast));


        }
    }


    public void PlayLvl2EndSeq()
    {
         Transform oliveTransform = this.transform;
            Transform playerTransform = Character.transform;

            Sequence oliveSequence = DOTween.Sequence();

            oliveSequence.AppendInterval(3);
            oliveSequence.Append(oliveTransform.DOLocalMoveY(oliveTransform.localPosition.y - 2, 2));



            oliveSequence.Append(playerTransform.DOMoveX(playerTransform.position.x + (oliveTransform.position.x - playerTransform.position.x) - 1, 3f));



            oliveSequence.Append(CharacterHead.transform.DORotate(new Vector3(0, 0, 16f), 0.5f, RotateMode.Fast));
    }
}