using UnityEngine;
using System.Collections.Generic;

public class LevelUpUI : MonoBehaviour
{
    [SerializeField] private List<RewardSlotUI> slots;
    [SerializeField] private List<RewardData> rewardPool;

    private PlayerWeaponManager weaponManager;
    private Player player;

    private void Awake()
    {
        weaponManager = FindAnyObjectByType<PlayerWeaponManager>();
        player = FindAnyObjectByType<Player>();
    }

    public void Show()
    {
        // Awake 이후에 생성된 경우를 대비해 재탐색
        if (weaponManager == null) weaponManager = FindAnyObjectByType<PlayerWeaponManager>();
        if (player == null) player = FindAnyObjectByType<Player>();

        var selected = GetRandomRewards(slots.Count);
        if (selected.Count == 0)
            return;

        gameObject.SetActive(true);
        Time.timeScale = 0f;

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < selected.Count)
                slots[i].Setup(selected[i], OnRewardSelected);
            else
                slots[i].Hide();
        }
    }

    private void OnRewardSelected(RewardData data)
    {
        if (data == null) return;

        if (data.rewardType == RewardType.Weapon)
        {
            weaponManager.AddOrUpgradeWeapon(data.weaponPrefab);
        }
        else if (data.rewardType == RewardType.Stat)
        {
            ApplyStatReward(data);
        }

        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    private void ApplyStatReward(RewardData data)
    {
        if (player == null) return;

        switch (data.statRewardType)
        {
            case StatRewardType.AddMaxHp:
                player.maxHp += data.value;
                player.currentHp += data.value;
                if (player.currentHp > player.maxHp) player.currentHp = player.maxHp;
                break;

            case StatRewardType.Heal:
                player.currentHp += data.value;
                if (player.currentHp > player.maxHp) player.currentHp = player.maxHp;
                break;

            case StatRewardType.Power:
                player.power += data.value;
                break;
        }
    }

    private List<RewardData> GetRandomRewards(int count)
    {
        List<RewardData> copy = new List<RewardData>();
        List<RewardData> result = new List<RewardData>();

        foreach (var reward in rewardPool)
        {
            if (IsValidReward(reward))
                copy.Add(reward);
        }

        count = Mathf.Min(count, copy.Count);

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, copy.Count);
            result.Add(copy[index]);
            copy.RemoveAt(index);
        }

        return result;
    }

    private bool IsValidReward(RewardData reward)
    {
        return reward != null
            && !string.IsNullOrWhiteSpace(reward.rewardName)
            && !string.IsNullOrWhiteSpace(reward.description)
            && (reward.rewardType != RewardType.Weapon || reward.weaponPrefab != null);
    }
}
