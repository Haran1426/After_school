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
        gameObject.SetActive(true);
        Time.timeScale = 0f;

        // Awake 이후에 생성된 경우를 대비해 재탐색
        if (weaponManager == null) weaponManager = FindAnyObjectByType<PlayerWeaponManager>();
        if (player == null) player = FindAnyObjectByType<Player>();

        var selected = GetRandomRewards(3);

        for (int i = 0; i < slots.Count; i++)
            slots[i].Setup(selected[i], OnRewardSelected);
    }

    private void OnRewardSelected(RewardData data)
    {
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
        List<RewardData> copy = new List<RewardData>(rewardPool);
        List<RewardData> result = new List<RewardData>();

        count = Mathf.Min(count, copy.Count);

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, copy.Count);
            result.Add(copy[index]);
            copy.RemoveAt(index);
        }

        return result;
    }
}