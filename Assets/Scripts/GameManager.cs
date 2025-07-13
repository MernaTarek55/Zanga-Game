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
    public float tickInterval = 1f;
    private float lastTickTime;
    private float lastLevelTimer;
    private bool isTimerActive;
    [SerializeField] GameObject Cage;
    [SerializeField] LastLevelHandle lastLevelHandle;

    [Header("Win Panel")]
    [SerializeField] GameObject winpanel;
    [SerializeField] Image winpanelImage;
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

            // Tick every `tickInterval` seconds
            if (Time.time - lastTickTime >= tickInterval)
            {
                lastTickTime = Time.time;
                TickTock();
            }

            if (lastLevelTimer <= 0)
            {
                isTimerActive = false;
                OnTimerFinished();
            }
        }
    }
    void TickTock()
    {
        if (clockBranch == null) return;

        float totalTicks = lastLevelDuration / tickInterval;
        float angleStep = 360f / totalTicks;

        clockBranch.Rotate(0, 0, -angleStep);

        // Play tick sound (optional)
        // AudioManager.Instance.Play("TickSound");
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
        StopTimer();

        if (IsLastLevel() && ClueManager.Instance.AreAllCluesSolved)
        {
            StopTimer();
            ShowWinPanel();
            return;
        }

        DOVirtual.DelayedCall(3f, () =>
        {
            if (IsLastLevel())
            {
                CompleteLastLevel();
            }
            else
            {
                currentLevelIndex++;
                TransitionToNextLevel();
            }
        });
    }
    void ShowWinPanel()
    {
        winpanel.SetActive(true);

        // Set initial transparent color
        Color newColor = winpanelImage.color;
        newColor.a = 0f;
        winpanelImage.color = newColor;

        // Fade in the image
        winpanelImage.DOFade(1f, 1f).SetEase(Ease.InQuad);
    }
    private void TransitionToNextLevel()
    {
        Sequence transition = DOTween.Sequence();

        // 1. Move environment
        transition.Append(
            environmentParent.DOMove(levelPositions[currentLevelIndex], levelTransitionDuration)
            .SetEase(movementEase)
        );

        // 2. Show clue at 50% of transition (FIXED VERSION)
        transition.Insert(levelTransitionDuration * 0.5f,
            DOTween.To(
                () => 0f,  // Proper getter that returns a value
                x => { },   // Empty setter
                1f,        // Unused but required
                0f         // Immediate
            ).OnStart(() => {
                ClueManager.Instance.ResetForNewLevel();
                ClueManager.Instance.ShowCurrentClue();
            })
        );

        // 3. Final setup
        transition.OnComplete(() => {
            if (IsLastLevel())
            {
                lastLevelHandle.enabled = true;
                StartTimer();
                DisableHintsForFinalLevel();
            }
        });
    }


    //private IEnumerator CompleteLastLevel()
    //{
    //    // Zoom in when completing last level
    //    yield return ZoomInCamera();

    //    // Wait a moment before resetting
    //    yield return new WaitForSeconds(1f);

    //    ReturnToFirstLevel();
    //}
    private void CompleteLastLevel()
    {
        // Similar structure but for last level completion
        Sequence lastLevelSequence = DOTween.Sequence();

        // Add any animations/waits needed for last level
        lastLevelSequence.AppendInterval(1f); // Example delay

        lastLevelSequence.OnComplete(() =>
        {
            // Your last level completion logic
        });
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