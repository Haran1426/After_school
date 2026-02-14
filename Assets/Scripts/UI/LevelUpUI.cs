using UnityEngine;
using System.Collections.Generic;

public class LevelUpUI : MonoBehaviour
{
    [SerializeField] private List<RewardSlotUI> slots;
    [SerializeField] private List<RewardData> rewardPool;

    public void Show()
    {
        gameObject.SetActive(true);

        List<RewardData> selected = GetRandomRewards(3);

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].Setup(selected[i], OnRewardSelected);
        }

        Time.timeScale = 0f;
    }

    private void OnRewardSelected(RewardData data)
    {
        Debug.Log(data.rewardName);


        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    private List<RewardData> GetRandomRewards(int count)
    {
        List<RewardData> copy = new List<RewardData>(rewardPool);
        List<RewardData> result = new List<RewardData>();

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, copy.Count);
            result.Add(copy[index]);
            copy.RemoveAt(index);
        }

        return result;
    }
}