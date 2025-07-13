using UnityEngine;

[CreateAssetMenu(menuName = "Clue Data")]
public class ClueData : ScriptableObject
{
    public int clueID;
    public Sprite clueImage;
    public int[] validObjectIDs;
    public int clueSteps = 0;
    public int clueMaxSteps;
}
