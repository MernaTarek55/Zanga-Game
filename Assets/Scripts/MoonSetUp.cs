using UnityEngine;

public class MoonSetUp : MonoBehaviour
{
    void OnEnable()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.gray;
        }

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
    

}
