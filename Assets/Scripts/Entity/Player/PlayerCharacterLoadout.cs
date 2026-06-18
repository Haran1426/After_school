using UnityEngine;

[System.Serializable]
public sealed class PlayerCharacterLoadoutEntry
{
    public PlayerCharacterType character;
    public GameObject visualPrefab;
    public WeaponBase initialWeaponPrefab;
    [Min(0.1f)] public float moveSpeed = 5f;
    [Min(1f)] public float maxHp = 10f;
    [Min(0.1f)] public float power = 1f;
}

public sealed class PlayerCharacterLoadout : MonoBehaviour
{
    private const string SelectedVisualName = "SelectedCharacterVisual";

    [SerializeField] private Transform visualRoot;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private Player player;
    [SerializeField] private PlayerWeaponManager weaponManager;
    [SerializeField] private PlayerCharacterLoadoutEntry[] loadouts;

    private GameObject spawnedVisual;

    private void Awake()
    {
        Apply(CharacterSelection.SelectedCharacter);
    }

    public void Apply(PlayerCharacterType character)
    {
        PlayerCharacterLoadoutEntry loadout = FindLoadout(character);
        if (loadout == null)
            return;

        ApplyVisual(loadout.visualPrefab);
        ApplyStats(loadout);
        ApplyInitialWeapon(loadout.initialWeaponPrefab);
    }

    private PlayerCharacterLoadoutEntry FindLoadout(PlayerCharacterType character)
    {
        if (loadouts == null)
            return null;

        for (int i = 0; i < loadouts.Length; i++)
        {
            PlayerCharacterLoadoutEntry loadout = loadouts[i];
            if (loadout != null && loadout.character == character)
                return loadout;
        }

        return null;
    }

    private void ApplyVisual(GameObject visualPrefab)
    {
        if (visualPrefab == null)
            return;

        if (visualRoot != null)
            visualRoot.gameObject.SetActive(false);

        if (spawnedVisual != null)
            Destroy(spawnedVisual);

        GameObject visual = Instantiate(visualPrefab, transform);
        visual.name = SelectedVisualName;
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.identity;
        spawnedVisual = visual;
    }

    private void ApplyStats(PlayerCharacterLoadoutEntry loadout)
    {
        if (movement == null)
            movement = GetComponent<PlayerMovement>();

        if (movement != null)
            movement.ConfigureMoveSpeed(loadout.moveSpeed);

        if (player == null)
            player = GetComponent<Player>();

        if (player != null)
        {
            player.maxHp = loadout.maxHp;
            player.currentHp = loadout.maxHp;
            player.power = loadout.power;
            player.ConfigureCharacterPassive(loadout.character);
        }
    }

    private void ApplyInitialWeapon(WeaponBase weaponPrefab)
    {
        if (weaponPrefab == null)
            return;

        if (weaponManager == null)
            weaponManager = GetComponentInChildren<PlayerWeaponManager>();

        if (weaponManager != null)
            weaponManager.AddOrUpgradeWeapon(weaponPrefab);
    }
}
