using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PatternManager : MonoBehaviour
{
    public static PatternManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private ClueData clue;
    [SerializeField] private int[] patternSteps;
    [SerializeField] private GameObject[] petalsLight;
    [SerializeField] private GameObject moon;

    private int patternIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public int OnPetalPressed(int petalIndex)
    {
        if (petalIndex != patternSteps[patternIndex])
        {
            HandleIncorrectPattern();
            return 0;
        }

        patternIndex++;
        clue.clueSteps++;

        Debug.Log($"✅ Correct petal: {petalIndex}, step: {clue.clueSteps}/{clue.clueMaxSteps}");

        if (clue.clueSteps == clue.clueMaxSteps)
        {
            HandlePatternComplete();
        }

        return 1;
    }

    private void HandleIncorrectPattern()
    {
        Debug.Log($"❌ Incorrect petal. Expected: {patternSteps[patternIndex]}");
        patternIndex = 0;
        clue.clueSteps = 0;
        SetAllPetalsIntensity(0f);
    }

    private void HandlePatternComplete()
    {
        Debug.Log("🎉 Pattern completed!");
        patternIndex = 0;
        moon.SetActive(true);
        SetAllPetalsIntensity(1f);
        ClueManager.Instance.OnClueSolved(clue.clueID);
        
    }

    public static void SetAllPetalsIntensity(float intensity)
    {
        if (Instance == null || Instance.petalsLight == null)
        {
            Debug.LogError("PatternManager not initialized or petalsLight not set!");
            return;
        }

        foreach (var petal in Instance.petalsLight)
        {
            if (petal == null) continue;

            var light = petal.GetComponent<Light2D>();
            if (light != null)
            {
                light.intensity = intensity;
            }
        }
    }
}