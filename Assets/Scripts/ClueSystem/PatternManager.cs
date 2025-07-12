using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PatternManager : MonoBehaviour
{
    public static PatternManager Instance;
    int patternIndex = 0;
    ClueData clue;
    [SerializeField] int[] patternSteps;
    [SerializeField] GameObject[] PetalsLight;
    [SerializeField] GameObject moon;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        clue = GameManager.Instance.levels[GameManager.Instance.currentLevelIndex].levelClues[ClueManager.Instance.currentClueIndex];
        clue.clueSteps = 0; // Initialize clue steps
    }


    public int OnPetalPressed(int petalIndex)
    {
        // if not correct: reset everything
        if (petalIndex != patternSteps[patternIndex])
        {
            Debug.Log($"❌ Incorrect petal pressed: {petalIndex}, expected: {patternIndex}");
            patternIndex = 0; // Reset pattern index
            clue.clueSteps = 0; // Reset clue steps
            foreach(var petalLight in PetalsLight)
            {
                Light2D light = petalLight.GetComponent<Light2D>();
                if (light != null)
                {
                    light.intensity = 0f;
                }
            }
            return 0;
        }
        else
        {
            patternIndex++;
            clue.clueSteps++;
            Debug.LogWarning($"✅ Correct petal pressed: {petalIndex}, current step: {clue.clueSteps}/{clue.clueMaxSteps}");
            if (clue.clueSteps == clue.clueMaxSteps)
            {
                Debug.Log($"✅ Pattern completed successfully!");
                moon.SetActive(true);
                foreach (var petalLight in PetalsLight)
                {
                    Light2D light = petalLight.GetComponent<Light2D>();
                    if (light != null)
                    {
                        light.intensity = 1f;
                    }
                }
                ClueManager.Instance.OnClueSolved(clue.clueID);
            }
            return 1;
        }
    }
}
