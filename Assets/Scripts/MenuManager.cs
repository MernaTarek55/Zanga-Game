using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject startMenuPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartGame()
    {
        GameManager.Instance.InitializeLevel();
        ClueManager.Instance.ShowCurrentClue();
        startMenuPanel.SetActive(false);
    }
}
