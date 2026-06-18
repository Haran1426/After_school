public enum PlayerCharacterType
{
    Cat,
    Bunny,
    Squirrel
}

public static class CharacterSelection
{
    public static PlayerCharacterType SelectedCharacter { get; private set; } = PlayerCharacterType.Cat;

    public static void Select(PlayerCharacterType character)
    {
        SelectedCharacter = character;
    }
}
