using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class MobileJoystickUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public static Vector2 Direction { get; private set; }

    [SerializeField] private RectTransform baseRect;
    [SerializeField] private RectTransform knobRect;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField, Min(24f)] private float radius = 72f;
    [SerializeField, Range(0f, 1f)] private float idleAlpha = 0.48f;
    [SerializeField, Range(0f, 1f)] private float activeAlpha = 0.82f;

    private RectTransform rootRect;
    private Camera eventCamera;
    private int activePointerId = int.MinValue;

    private void Awake()
    {
        if (baseRect == null || knobRect == null)
            BuildFallbackUI();

        rootRect = (RectTransform)transform;

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        Direction = Vector2.zero;
        EnsureEventSystem();
    }

    private void Update()
    {
        bool shouldShow = WaveManager.Instance != null
            && GameRoot.Instance != null
            && !GameRoot.Instance.Game.IsGameOver
            && !GameRoot.Instance.Game.IsStageCleared;

        canvasGroup.alpha = shouldShow ? (activePointerId == int.MinValue ? idleAlpha : activeAlpha) : 0f;
        canvasGroup.blocksRaycasts = shouldShow;
        canvasGroup.interactable = shouldShow;

        if (!shouldShow)
            ResetStick();
    }

    public static void EnsureExists()
    {
        if (FindAnyObjectByType<MobileJoystickUI>() != null)
            return;

        GameObject root = new GameObject("MobileJoystickUI", typeof(RectTransform));
        root.AddComponent<MobileJoystickUI>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (activePointerId != int.MinValue)
            return;

        activePointerId = eventData.pointerId;
        eventCamera = eventData.pressEventCamera;
        UpdateStick(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != activePointerId)
            return;

        UpdateStick(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != activePointerId)
            return;

        ResetStick();
    }

    private void UpdateStick(PointerEventData eventData)
    {
        if (baseRect == null || knobRect == null)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, eventData.position, eventCamera, out Vector2 localPoint);
        Vector2 clamped = Vector2.ClampMagnitude(localPoint, radius);
        Direction = clamped / radius;
        knobRect.anchoredPosition = clamped;
    }

    private void ResetStick()
    {
        activePointerId = int.MinValue;
        Direction = Vector2.zero;

        if (knobRect != null)
            knobRect.anchoredPosition = Vector2.zero;
    }

    private void BuildFallbackUI()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 30;
        gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        gameObject.AddComponent<GraphicRaycaster>();

        RectTransform root = (RectTransform)transform;
        root.anchorMin = Vector2.zero;
        root.anchorMax = Vector2.one;
        root.offsetMin = Vector2.zero;
        root.offsetMax = Vector2.zero;

        GameObject stickBase = new GameObject("JoystickBase");
        stickBase.transform.SetParent(transform, false);
        baseRect = stickBase.AddComponent<RectTransform>();
        baseRect.anchorMin = Vector2.zero;
        baseRect.anchorMax = Vector2.zero;
        baseRect.pivot = new Vector2(0.5f, 0.5f);
        baseRect.anchoredPosition = new Vector2(118f, 112f);
        baseRect.sizeDelta = new Vector2(radius * 2f, radius * 2f);

        Image baseImage = stickBase.AddComponent<Image>();
        baseImage.color = new Color(0.08f, 0.13f, 0.09f, 0.42f);
        baseImage.sprite = CreateCircleSprite(96, new Color(1f, 1f, 1f, 1f), new Color(1f, 1f, 1f, 0f));

        GameObject knob = new GameObject("JoystickKnob");
        knob.transform.SetParent(baseRect, false);
        knobRect = knob.AddComponent<RectTransform>();
        knobRect.anchorMin = new Vector2(0.5f, 0.5f);
        knobRect.anchorMax = new Vector2(0.5f, 0.5f);
        knobRect.pivot = new Vector2(0.5f, 0.5f);
        knobRect.anchoredPosition = Vector2.zero;
        knobRect.sizeDelta = new Vector2(76f, 76f);

        Image knobImage = knob.AddComponent<Image>();
        knobImage.color = new Color(0.74f, 0.95f, 0.63f, 0.86f);
        knobImage.sprite = CreateCircleSprite(64, new Color(1f, 1f, 1f, 1f), new Color(1f, 1f, 1f, 0f));
    }

    private static Sprite CreateCircleSprite(int size, Color centerColor, Color edgeColor)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;

        float center = (size - 1) * 0.5f;
        float maxDistance = center;
        Color[] pixels = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance01 = Vector2.Distance(new Vector2(x, y), new Vector2(center, center)) / maxDistance;
                float alpha = distance01 <= 1f ? Mathf.SmoothStep(1f, 0f, Mathf.Clamp01(distance01)) : 0f;
                pixels[y * size + x] = Color.Lerp(edgeColor, centerColor, alpha) * new Color(1f, 1f, 1f, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
    }

    private static void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null)
            return;

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
    }
}
