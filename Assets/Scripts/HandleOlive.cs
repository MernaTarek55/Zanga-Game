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


            oliveSequence.Append(oliveTransform.DOLocalMove(new Vector3(oliveTransform.localPosition.x, oliveTransform.localPosition.y-3, 0), 1f));

            oliveSequence.Append(playerTransform.DOMove(new Vector3(playerTransform.position.x+10, playerTransform.position.y, 0f), 1.5f));

            oliveSequence.Append(oliveTransform.DOLocalMove(new Vector3(oliveTransform.localPosition.x-0.2f, oliveTransform.localPosition.y-1.5f, 0f), 1f)
                .SetEase(Ease.OutBack));

            oliveSequence.OnComplete(() => {
                if (CharacterHead != null)
                {
                    CharacterHead.transform.rotation = Quaternion.Euler(0f, 0f, CharacterHead.transform.rotation.z+16f);
                }
            });
        }
    }
}