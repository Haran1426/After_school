using UnityEngine;

[CreateAssetMenu(menuName = "LevelUp/Reward")]
public class RewardData : ScriptableObject
{
    public string rewardName;

    [TextArea]
    public string description;

    public Sprite icon;
    public Sprite background;

    public float value;
}