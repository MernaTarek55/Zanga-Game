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
            Debug.Log("Hiiiiiii");
            Transform oliveTransform = this.transform;
            Transform playerTransform = Character.transform;

            Sequence oliveSequence = DOTween.Sequence();


            oliveSequence.Append(oliveTransform.DOLocalMove(new Vector3(-1.26f, -2.25999999f, 0), 1f));

            oliveSequence.Append(playerTransform.DOMove(new Vector3(5f, -3f, 0f), 1.5f));

            oliveSequence.Append(oliveTransform.DOLocalMove(new Vector3(-1.5f, -1.8f, 0f), 1f)
                .SetEase(Ease.OutBack));

            oliveSequence.OnComplete(() => {
                if (CharacterHead != null)
                {
                    CharacterHead.transform.rotation = Quaternion.Euler(0f, 0f, 16.083f);
                }
            });
        }
    }
}