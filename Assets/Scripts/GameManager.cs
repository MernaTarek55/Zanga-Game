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
    public float defaultZoomSize = 5f;

    [Header("Environment Movement")]
    public Transform environmentParent;
    public List<Vector3> levelPositions;
    public Ease movementEase = Ease.InOutQuad;

    [Header("References")]
    public Camera mainCamera;
    public TextMeshProUGUI poemTextUI;
    private Vector3 originalCameraPosition;

    [Header("Last Level Settings")]
    public float lastLevelDuration = 60f;
    public float zoomInDuration = 2f;
    public float zoomedInSize = 3f; // More zoomed-in than default
    public TextMeshProUGUI timerText;

    private float lastLevelTimer;
    private bool isTimerActive;

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

    private void Update()
    {
        if (!isTimerActive) return;

        lastLevelTimer -= Time.deltaTime;
        UpdateTimerDisplay();

        if (lastLevelTimer <= 0f)
        {
            OnTimerFinished();
        }
    }

    public void InitializeLevel()
    {
        if (currentLevelIndex < levelPositions.Count)
        {
            environmentParent.position = levelPositions[currentLevelIndex];
        }

        ResetCameraZoom();
    }

    private void ResetCameraZoom()
    {
        mainCamera.orthographicSize = defaultZoomSize;
        mainCamera.transform.position = originalCameraPosition;
    }

    public void OnLevelCompleted()
    {
        StopTimer(); // Stop timer when level is completed

        if (IsLastLevel())
        {
            if (ClueManager.Instance.AreAllCluesSolved)
            {
                
                StopTimer();
                //Win State
                return;
            }
            StartCoroutine(CompleteLastLevel());
        }
        else
        {
            currentLevelIndex++;
            StartCoroutine(TransitionToNextLevel());
        }
    }

    private IEnumerator TransitionToNextLevel()
    {
        Vector3 targetPosition = levelPositions[currentLevelIndex];
        environmentParent.DOMove(targetPosition, levelTransitionDuration)
            .SetEase(movementEase);

        yield return new WaitForSeconds(levelTransitionDuration);

        if (IsLastLevel())
        {
            StartTimer();
            DisableHintsForFinalLevel();
        }

        ClueManager.Instance.ResetForNewLevel();
        ClueManager.Instance.ShowCurrentClue();
    }

    private IEnumerator CompleteLastLevel()
    {
        // Zoom in when completing last level
        yield return ZoomInCamera();

        // Wait a moment before resetting
        yield return new WaitForSeconds(1f);

        ReturnToFirstLevel();
    }

    private IEnumerator ZoomInCamera()
    {
        mainCamera.DOOrthoSize(zoomedInSize, zoomInDuration)
                 .SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(zoomInDuration);
    }

    private bool IsLastLevel()
    {
        return currentLevelIndex >= levels.Count - 1;
    }

    public void StartTimer()
    {
        lastLevelTimer = lastLevelDuration;
        isTimerActive = true;

        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
            timerText.color = Color.white;
        }
    }

    public void StopTimer()
    {
        isTimerActive = false;
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }
    }

    private void OnTimerFinished()
    {
        StopTimer();
        if (ClueManager.Instance.AreAllCluesSolved)
        {
            // If clues are solved but timer finished, just stop timer
            
            return;
        }

        // If timer finished and clues not solved, reset level
        StartCoroutine(OnTimeExpiredReset());
    }

    private IEnumerator OnTimeExpiredReset()
    {
        StartCoroutine(CompleteLastLevel());
        // Zoom in before resetting
        yield return ZoomInCamera();

        // Wait a moment before resetting
        yield return new WaitForSeconds(0.5f);

        ReturnToFirstLevel();
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(lastLevelTimer / 60f);
        int seconds = Mathf.FloorToInt(lastLevelTimer % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (lastLevelTimer < 10f)
        {
            timerText.color = Color.red;
        }
    }

    private void ReturnToFirstLevel()
    {
        StartCoroutine(ResetToFirstLevel());
    }

    private IEnumerator ResetToFirstLevel()
    {
        // Reset camera zoom if we zoomed in
        ResetCameraZoom();

        // Move environment back to first level
        environmentParent.DOMove(levelPositions[0], levelTransitionDuration);

        yield return new WaitForSeconds(levelTransitionDuration);

        // Reset game state
        currentLevelIndex = 0;
        ClueManager.Instance.ResetAllClues();
        HintManager.Instance.SetEnabled(true);
        HintManager.Instance.ResetIdleTimer();

        // Reinitialize first level
        InitializeLevel();
    }

    public void OnPlayerFell()
    {
        ReturnToFirstLevel();
    }

    public void ShowNextVerse(string verseText)
    {
        if (string.IsNullOrEmpty(verseText)) return;

        StopCoroutine("DisplayVerseCoroutine");
        StartCoroutine(DisplayVerseCoroutine(verseText));
    }

    private IEnumerator DisplayVerseCoroutine(string verse)
    {
        if (poemTextUI == null) yield break;

        poemTextUI.enabled = true;
        poemTextUI.text = ArabicFixer.Fix(verse);

        // Only fade in (no fade out)
        if (poemTextUI.alpha < 1f) // Optional: Check if not already fully visible
        {
            poemTextUI.DOFade(1f, 0.5f).SetEase(Ease.InQuad);
        }

        // Remove the fade-out part entirely
    }

    public LevelData GetCurrentLevel()
    {
        return (currentLevelIndex >= 0 && currentLevelIndex < levels.Count) ?
               levels[currentLevelIndex] : null;
    }

    private void DisableHintsForFinalLevel()
    {
        HintManager.Instance.SetEnabled(false);
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