using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using ArabicSupport;
using UnityEngine.LowLevelPhysics;
using UnityEngine.UI;

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
    public Image poemImageUI;
    private Vector3 originalCameraPosition;

    [Header("First Level Settings")]
    [SerializeField] GameObject firstCage;
    [Header("Last Level Settings")]
    public float lastLevelDuration = 60f;
    public float zoomInDuration = 2f;
    public float zoomedInSize = 3f; 
    public Transform clockBranch; 
    private float lastLevelTimer;
    private bool isTimerActive;
    [SerializeField] GameObject Cage;
    [SerializeField] LastLevelHandle lastLevelHandle;
    private void Awake()
    {
        //AdjustCamera();
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
        //InitializeLevel();
    }

    
    void Update()
    {
        if (isTimerActive)
        {
            lastLevelTimer -= Time.deltaTime;

            float rotationProgress = 1 - (lastLevelTimer / lastLevelDuration);

            if (clockBranch != null)
            {
                clockBranch.rotation = Quaternion.Euler(0, 0, -360 * rotationProgress);
            }

            if (lastLevelTimer <= 0)
            {
                // Timer ended
                isTimerActive = false;
                OnTimerFinished();
            }
        }
    }

    public void InitializeLevel()
    {
        if (IsLastLevel())
        {
            StartTimer();
            DisableHintsForFinalLevel();
        }
        if (currentLevelIndex == 0)
        {
            firstCage.transform.DOMove(new Vector3(firstCage.transform.localPosition.x, firstCage.transform.localPosition.y -7f, 0), 1f);
        }
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
            lastLevelHandle.enabled = true;
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

    }

    public void StopTimer()
    {
        isTimerActive = false;
        
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
        OnTimeExpiredReset();
    }

    private void OnTimeExpiredReset()
    {
        Cage.GetComponent<Animator>().enabled = false;
        Cage.transform.DOMove(new Vector3(Cage.transform.localPosition.x, Cage.transform.localPosition.y -3.5f, 0), 1f);
        //StartCoroutine(CompleteLastLevel());
        //// Zoom in before resetting
        //yield return ZoomInCamera();

        //// Wait a moment before resetting
        //yield return new WaitForSeconds(0.5f);

        //ReturnToFirstLevel();
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

    public void ShowNextVerse(Sprite verseImage)
    {
        if (verseImage == null) return;

        DOTween.Kill(poemImageUI);

        poemImageUI.sprite = verseImage;
        poemImageUI.enabled = true;

        // Set initial transparency
        Color startColor = poemImageUI.color;
        startColor.a = 0f;
        poemImageUI.color = startColor;

        Sequence verseSequence = DOTween.Sequence();
        verseSequence.Append(poemImageUI.DOFade(1f,1f).SetEase(Ease.InQuad)); // Fade In
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

    void AdjustCamera()
    {
        int pixelsPerUnit = 100; // Match your sprite PPU
        Camera.main.orthographicSize = Screen.height / (2f * pixelsPerUnit);
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