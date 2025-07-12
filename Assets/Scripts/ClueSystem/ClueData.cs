using UnityEngine;

[CreateAssetMenu(menuName = "Clue Data")]
public class ClueData : ScriptableObject
{
    public int clueID;
    [TextArea(2, 4)] public string clueText;
    public int[] validObjectIDs;
    public int clueSteps = 0;
    public int clueMaxSteps;
}
