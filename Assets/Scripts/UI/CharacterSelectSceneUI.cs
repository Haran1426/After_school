using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class CharacterSelectSceneUI : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private TextMeshProUGUI selectedText;
    [SerializeField] private Sprite catPreview;
    [SerializeField] private Sprite bunnyPreview;
    [SerializeField] private Sprite squirrelPreview;

    private void Start()
    {
        BuildMenu();
        Select(CharacterSelection.SelectedCharacter);
    }

    public void SelectCat()
    {
        Select(PlayerCharacterType.Cat, "고양이");
    }

    public void SelectBunny()
    {
        Select(PlayerCharacterType.Bunny, "토끼");
    }

    public void SelectSquirrel()
    {
        Select(PlayerCharacterType.Squirrel, "다람쥐");
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void BackToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    private void Select(PlayerCharacterType character, string displayName)
    {
        CharacterSelection.Select(character);

        if (selectedText != null)
            selectedText.text = "선택됨: " + displayName;
    }

    private void Select(PlayerCharacterType character)
    {
        switch (character)
        {
            case PlayerCharacterType.Bunny:
                SelectBunny();
                break;
            case PlayerCharacterType.Squirrel:
                SelectSquirrel();
                break;
            default:
                SelectCat();
                break;
        }
    }

    private void BuildMenu()
    {
        if (transform.Find("CharacterSelectPanel") != null)
            return;

        GameObject panel = CreateUiObject("CharacterSelectPanel", transform);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(1040f, 580f);

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.08f, 0.14f, 0.1f, 0.92f);

        VerticalLayoutGroup vertical = panel.AddComponent<VerticalLayoutGroup>();
        vertical.padding = new RectOffset(34, 34, 28, 28);
        vertical.spacing = 18f;
        vertical.childAlignment = TextAnchor.UpperCenter;
        vertical.childControlWidth = true;
        vertical.childControlHeight = false;
        vertical.childForceExpandWidth = true;
        vertical.childForceExpandHeight = false;

        CreateLabel(panel.transform, "캐릭터 선택", 42f, FontStyles.Bold, new Color(0.95f, 1f, 0.9f, 1f));
        selectedText = CreateLabel(panel.transform, "선택됨: 고양이", 24f, FontStyles.Normal, new Color(0.8f, 1f, 0.75f, 1f));

        GameObject row = CreateUiObject("CharacterRow", panel.transform);
        RectTransform rowRect = row.AddComponent<RectTransform>();
        rowRect.sizeDelta = new Vector2(0f, 300f);

        HorizontalLayoutGroup horizontal = row.AddComponent<HorizontalLayoutGroup>();
        horizontal.spacing = 26f;
        horizontal.childAlignment = TextAnchor.MiddleCenter;
        horizontal.childControlWidth = false;
        horizontal.childControlHeight = false;
        horizontal.childForceExpandWidth = false;
        horizontal.childForceExpandHeight = false;

        CreateCharacterCard(row.transform, "고양이", "균형형 / 속도 5.5 / 체력 10\n발톱 베기 / 10% 회피", catPreview, new Color(0.95f, 0.72f, 0.42f, 1f), SelectCat);
        CreateCharacterCard(row.transform, "토끼", "회피형 / 속도 6.2 / 체력 8\n점프 충격파 / 22% 회피", bunnyPreview, new Color(0.92f, 0.82f, 1f, 1f), SelectBunny);
        CreateCharacterCard(row.transform, "다람쥐", "기술형 / 공격 1.1 / 체력 11\n도토리 탄환 / 구슬 흡수 +0.9", squirrelPreview, new Color(0.74f, 0.48f, 0.25f, 1f), SelectSquirrel);

        GameObject bottomRow = CreateUiObject("BottomRow", panel.transform);
        RectTransform bottomRect = bottomRow.AddComponent<RectTransform>();
        bottomRect.sizeDelta = new Vector2(0f, 68f);

        HorizontalLayoutGroup bottom = bottomRow.AddComponent<HorizontalLayoutGroup>();
        bottom.spacing = 18f;
        bottom.childAlignment = TextAnchor.MiddleCenter;
        bottom.childControlWidth = false;
        bottom.childControlHeight = false;
        bottom.childForceExpandWidth = false;
        bottom.childForceExpandHeight = false;

        CreateButton(bottomRow.transform, "뒤로", new Color(0.35f, 0.35f, 0.35f, 1f), BackToTitle);
        CreateButton(bottomRow.transform, "게임 시작", new Color(0.25f, 0.62f, 0.28f, 1f), StartGame);
    }

    private void CreateCharacterCard(Transform parent, string title, string description, Sprite preview, Color color, UnityEngine.Events.UnityAction onClick)
    {
        GameObject card = CreateUiObject(title + "Card", parent);
        RectTransform rect = card.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(270f, 280f);

        Image image = card.AddComponent<Image>();
        image.color = color;

        Button button = card.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);

        VerticalLayoutGroup layout = card.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(16, 16, 18, 18);
        layout.spacing = 10f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        CreateLabel(card.transform, title, 32f, FontStyles.Bold, Color.white);

        GameObject previewObject = CreateUiObject(title + "Preview", card.transform);
        Image previewImage = previewObject.AddComponent<Image>();
        previewImage.sprite = preview;
        previewImage.preserveAspect = true;
        previewImage.raycastTarget = false;

        LayoutElement previewLayout = previewObject.AddComponent<LayoutElement>();
        previewLayout.preferredHeight = 100f;

        CreateLabel(card.transform, description, 19f, FontStyles.Normal, new Color(0.12f, 0.12f, 0.12f, 1f));
    }

    private Button CreateButton(Transform parent, string text, Color color, UnityEngine.Events.UnityAction onClick)
    {
        GameObject go = CreateUiObject(text + "Button", parent);
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(240f, 62f);

        Image image = go.AddComponent<Image>();
        image.color = color;

        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);

        TextMeshProUGUI label = CreateLabel(go.transform, text, 26f, FontStyles.Bold, Color.white);
        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        return button;
    }

    private TextMeshProUGUI CreateLabel(Transform parent, string text, float fontSize, FontStyles style, Color color)
    {
        GameObject go = CreateUiObject(text + "Text", parent);
        TextMeshProUGUI label = go.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = fontSize;
        label.fontStyle = style;
        label.color = color;
        label.alignment = TextAlignmentOptions.Center;
        label.textWrappingMode = TextWrappingModes.Normal;

        RectTransform rect = label.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0f, Mathf.Max(42f, fontSize * 2.2f));

        return label;
    }

    private GameObject CreateUiObject(string objectName, Transform parent)
    {
        GameObject go = new GameObject(objectName);
        go.layer = gameObject.layer;
        go.transform.SetParent(parent, false);
        return go;
    }
}
