using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerVisualAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] idleFrames;
    [SerializeField] private Sprite[] walkFrames;
    [SerializeField, Min(1f)] private float idleFps = 6f;
    [SerializeField, Min(1f)] private float walkFps = 10f;
    [SerializeField] private bool flipWithHorizontalInput = true;

    private PlayerMovement movement;
    private bool wasWalking;
    private int frameIndex;
    private float frameTimer;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        movement = GetComponentInParent<PlayerMovement>();
    }

    private void Update()
    {
        Vector2 input = movement != null
            ? movement.LastMoveInput
            : new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool isWalking = input.sqrMagnitude > 0.01f;

        if (flipWithHorizontalInput && input.x != 0f)
            spriteRenderer.flipX = input.x < 0f;

        Sprite[] frames = isWalking && walkFrames.Length > 0 ? walkFrames : idleFrames;
        float fps = isWalking ? walkFps : idleFps;

        if (frames.Length == 0)
            return;

        if (wasWalking != isWalking)
        {
            frameIndex = 0;
            frameTimer = 0f;
            wasWalking = isWalking;
        }

        frameTimer += Time.deltaTime;
        float frameDuration = 1f / fps;
        while (frameTimer >= frameDuration)
        {
            frameTimer -= frameDuration;
            frameIndex = (frameIndex + 1) % frames.Length;
        }

        spriteRenderer.sprite = frames[frameIndex];
    }
}
