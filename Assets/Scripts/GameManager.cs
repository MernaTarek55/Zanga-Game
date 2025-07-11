using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using ArabicSupport;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level Management")]
    public List<LevelData> levels;
    public int currentLevelIndex = 0;
    public float levelTransitionDuration = 2f;
    public float zoomOutDuration = 3f;
    public float zoomInDuration = 2f;
    public float zoomedOutSize = 10f;
    public float defaultZoomSize = 5f;

    [Header("Environment Movement")]
    public Transform environmentParent;
    public List<Vector3> levelPositions; // Positions for each level
    public Ease movementEase = Ease.InOutQuad;

    [Header("References")]
    public Camera mainCamera;
    public TextMeshProUGUI poemTextUI;
    private Vector3 originalCameraPosition;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        originalCameraPosition = mainCamera.transform.position;
    }
    
    private void Start()
    {
        InitializeLevel();
    }

    public void InitializeLevel()
    {
        // Position environment for current level
        if (currentLevelIndex < levelPositions.Count)
        {
            environmentParent.position = levelPositions[currentLevelIndex];
        }
        // Reset camera zoom
        mainCamera.orthographicSize = defaultZoomSize;
        mainCamera.transform.position = originalCameraPosition;
    }

    public void OnLevelCompleted()
    {
        currentLevelIndex++;
        
        if (currentLevelIndex < levels.Count)
        {
            // Move to next level
            StartCoroutine(TransitionToNextLevel());
        }
        else
        {
            // All levels completed - zoom out to show full view
            StartCoroutine(ZoomOutForFinalView());
        }
    }

    private IEnumerator TransitionToNextLevel()
    {

        // Check if we have another level to transition to
        if (currentLevelIndex < levelPositions.Count && currentLevelIndex < levels.Count)
        {
            // Move environment to next level position
            Vector3 targetPosition = levelPositions[currentLevelIndex];
            environmentParent.DOMove(targetPosition, levelTransitionDuration)
                .SetEase(movementEase);

            yield return new WaitForSeconds(levelTransitionDuration);
            HintManager.Instance.SetEnabled(true);
            HintManager.Instance.ResetIdleTimer();
            // Initialize next level
            ClueManager.Instance.ResetForNewLevel();
            ClueManager.Instance.ShowCurrentClue();
        }
        else
        {
            // No more levels - zoom out to show final view
            StartCoroutine(ZoomOutForFinalView());
        }
    }

    private IEnumerator ZoomOutForFinalView()
    {
        // Zoom camera out
        mainCamera.DOOrthoSize(zoomedOutSize, zoomOutDuration);

        yield return new WaitForSeconds(zoomOutDuration);

        // Start timer for final view
        StartCoroutine(FinalViewCountdown());
    }

    private IEnumerator FinalViewCountdown()
    {
        float timer = 10f; // 10 seconds to view completed puzzle

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            // You could display this timer in UI if desired
            yield return null;
        }

        // Return to first level
        ReturnToFirstLevel();
    }

    public void OnPlayerFell()
    {
        // Player fell - reset to first level
        ReturnToFirstLevel();
    }

    private void ReturnToFirstLevel()
    {
        StartCoroutine(ZoomInAndReset());
    }

    private IEnumerator ZoomInAndReset()
    {
        // Zoom camera back in
        mainCamera.DOOrthoSize(defaultZoomSize, zoomInDuration);

        // Move environment back to first level position
        environmentParent.DOMove(levelPositions[0], zoomInDuration)
            .SetEase(movementEase);

        yield return new WaitForSeconds(zoomInDuration);

        // Reset level progress
        currentLevelIndex = 0;
        ClueManager.Instance.ResetAllClues();
        InitializeLevel();
    }

    public void ShowNextVerse(string verseText)
    {
        if (string.IsNullOrEmpty(verseText))
        {
            Debug.LogWarning("Tried to show empty verse text");
            return;
        }

        // Stop any ongoing verse animation
        StopCoroutine("DisplayVerseCoroutine");

        // Start new display routine
        StartCoroutine(DisplayVerseCoroutine(verseText));
    }

    private IEnumerator DisplayVerseCoroutine(string verse)
    {
        if (poemTextUI == null)
        {
            Debug.LogError("PoemTextUI reference is missing!");
            yield break;
        }

        // Ensure the text component is enabled
        poemTextUI.enabled = true;

        // Process Arabic text if needed
        string processedText = ArabicFixer.Fix(verse);
        poemTextUI.text = processedText;

        // Fade in
        poemTextUI.alpha = 0f;
        poemTextUI.DOFade(1f, 0.5f).SetEase(Ease.InQuad);

        // Wait for fade in + display time
        yield return new WaitForSeconds(0.5f + 3f); // 0.5s fade + 3s display

        // Fade out
        poemTextUI.DOFade(0f, 0.5f).SetEase(Ease.OutQuad);

        // Optional: Disable after fade out
        yield return new WaitForSeconds(0.5f);
        // poemTextUI.enabled = false;
    }
    public LevelData GetCurrentLevel()
    {
        if (currentLevelIndex >= 0 && currentLevelIndex < levels.Count)
            return levels[currentLevelIndex];
        return null;
    }
}
[System.Serializable]
public class LevelData
{
    public string levelName;
    public List<ClueData> levelClues = new List<ClueData>();
    public Vector3 environmentPosition;

    public bool IsValid()
    {
        return levelClues != null && levelClues.Count > 0;
    }
}