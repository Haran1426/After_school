using UnityEngine;

[CreateAssetMenu(fileName = "WaveProfile", menuName = "After School/Wave Profile")]
public sealed class WaveProfile : ScriptableObject
{
    [SerializeField] private WavePhaseData[] phases;

    public WavePhaseData[] Phases => phases;
}
