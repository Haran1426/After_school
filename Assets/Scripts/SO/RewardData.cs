using UnityEngine;




public enum RewardType
{
    Stat,
    Weapon
}

public enum StatRewardType
{
    AddMaxHp,
    Heal,
    Power
}

[CreateAssetMenu(menuName = "LevelUp/Reward")]
public class RewardData : ScriptableObject
{
    public string rewardName;

    [TextArea] public string description;

    public Sprite icon;
    public Sprite background;

    public RewardType rewardType;

    public float value;

    public StatRewardType statRewardType;   // ½ºÅÝ ¶§ »ç¿ë
    public WeaponBase weaponPrefab;
}