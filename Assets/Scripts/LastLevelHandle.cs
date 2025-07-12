using DG.Tweening;
using UnityEngine;

public class LastLevelHandle : MonoBehaviour
{
    [SerializeField] private GameObject Character;
    [SerializeField] private GameObject CharacterHead;
    private void Start()
    {
        Character.transform.DOMove(new Vector3(Character.transform.localPosition.x + 3.3f, Character.transform.localPosition.y, 0f), 1.5f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("hourHand"))
        {
            this.GetComponent<Collider2D>().enabled = false;
            Transform oliveTransform = this.transform;
            Transform playerTransform = Character.transform;

            Sequence oliveSequence = DOTween.Sequence();


            oliveSequence.Append(oliveTransform.DOLocalMove(new Vector3(oliveTransform.localPosition.x, oliveTransform.localPosition.y - 4, 0), 1f));

            oliveSequence.Append(playerTransform.DOMove(new Vector3(playerTransform.position.x + 7, playerTransform.position.y, 0f), 1.5f));

            
        }
    }
}
