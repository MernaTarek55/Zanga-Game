using DG.Tweening;
using UnityEngine;

public class LastLevelHandle : MonoBehaviour
{
    [SerializeField] private GameObject Character;
    [SerializeField] private GameObject CharacterHead;
    [SerializeField] private Transform hingedBranch;
    private void Start()
    {
        Character.transform.DOMove(new Vector3(Character.transform.localPosition.x + 3.3f, Character.transform.localPosition.y, 0f), 1.5f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("hourHand"))
        {
            this.GetComponent<Collider2D>().enabled = false;
            Transform playerTransform = Character.transform;

            Transform branch = hingedBranch;

            float bigSwingAngle = 75f;        // Initial "snap" swing
            float bigSwingDuration = 0.4f;

             float smallSwingAngle = 8f;        // Small swing arc
            float smallSwingDuration = 1.2f;
            int swingLoops = 3;               // Number of yoyo swings (3 full swings)

            Sequence branchSequence = DOTween.Sequence();
            AudioManager.Instance.PlaySound(SoundType.BranchBreaking);
            // Step 1: Big initial swing (as if branch snapped)
            branchSequence.Append(
                branch.DOLocalRotate(new Vector3(0, 0, bigSwingAngle), bigSwingDuration)
                    .SetEase(Ease.OutBack)
            );

            branchSequence.Append(
            branch.DOLocalRotate(new Vector3(0, 0, bigSwingAngle + smallSwingAngle), smallSwingDuration)
                  .SetEase(Ease.InOutSine)
                  .SetLoops(swingLoops, LoopType.Yoyo)
        );

            // Step 3: Return to resting position
            branchSequence.Append(
                branch.DOLocalRotate(Vector3.zero, 1f)
                    .SetEase(Ease.OutQuad)
            );

            branchSequence.Append(branch.DOLocalRotate(new Vector3(0, 0, 55), 2f)
                    .SetEase(Ease.OutQuad)
            );
            branchSequence.Join(branch.DOLocalMoveY(-1.5f, 1));

            branchSequence.Append(playerTransform.DOLocalMoveX(-3,2));


        }
    }
    
   
}
