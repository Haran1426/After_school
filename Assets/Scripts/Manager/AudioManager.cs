using UnityEngine;

public enum AudioCue
{
    KnifeThrow,
    EnemyHit,
    EnemyDie,
    PlayerHit,
    ExpPickup,
    LevelUp,
    RewardSelect,
    GameOver
}

public class AudioManager : ManagerBase
{
    AudioSource bgmSource;
    AudioSource sfxSource;

    AudioClip bgmClip;
    AudioClip knifeThrowClip;
    AudioClip enemyHitClip;
    AudioClip enemyDieClip;
    AudioClip playerHitClip;
    AudioClip expPickupClip;
    AudioClip levelUpClip;
    AudioClip rewardSelectClip;
    AudioClip gameOverClip;

    float bgmVolume = 0.35f;
    float sfxVolume = 0.8f;

    protected override void OnInitialize()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        sfxSource.playOnAwake = false;

        CreateDefaultClips();

        ApplyVolume();
        PlayBgm();
    }

    void ApplyVolume()
    {
        bgmSource.volume = bgmVolume;
        sfxSource.volume = sfxVolume;
    }

    public void SetBgmVolume(float value)
    {
        bgmVolume = Mathf.Clamp01(value);
        bgmSource.volume = bgmVolume;
    }

    public void SetSfxVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        sfxSource.volume = sfxVolume;
    }

    public float GetBgmVolume()
    {
        return bgmVolume;
    }

    public float GetSfxVolume()
    {
        return sfxVolume;
    }

    public void PlayBgm(AudioClip clip = null)
    {
        if (bgmSource == null)
            return;

        bgmSource.clip = clip != null ? clip : bgmClip;
        if (bgmSource.clip != null && !bgmSource.isPlaying)
            bgmSource.Play();
    }

    public void StopBgm()
    {
        if (bgmSource != null)
            bgmSource.Stop();
    }

    public void PlaySfx(AudioCue cue)
    {
        PlaySfx(GetClip(cue));
    }

    public void PlaySfx(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    private AudioClip GetClip(AudioCue cue)
    {
        switch (cue)
        {
            case AudioCue.KnifeThrow:
                return knifeThrowClip;
            case AudioCue.EnemyHit:
                return enemyHitClip;
            case AudioCue.EnemyDie:
                return enemyDieClip;
            case AudioCue.PlayerHit:
                return playerHitClip;
            case AudioCue.ExpPickup:
                return expPickupClip;
            case AudioCue.LevelUp:
                return levelUpClip;
            case AudioCue.RewardSelect:
                return rewardSelectClip;
            case AudioCue.GameOver:
                return gameOverClip;
            default:
                return null;
        }
    }

    private void CreateDefaultClips()
    {
        bgmClip = CreateBgmClip();
        knifeThrowClip = CreateToneClip("KnifeThrow", 0.08f, 740f, 440f, 0.45f, WaveType.Triangle);
        enemyHitClip = CreateToneClip("EnemyHit", 0.06f, 220f, 110f, 0.55f, WaveType.Noise);
        enemyDieClip = CreateToneClip("EnemyDie", 0.16f, 180f, 70f, 0.7f, WaveType.Saw);
        playerHitClip = CreateToneClip("PlayerHit", 0.12f, 120f, 70f, 0.75f, WaveType.Square);
        expPickupClip = CreateToneClip("ExpPickup", 0.09f, 660f, 990f, 0.45f, WaveType.Sine);
        levelUpClip = CreateArpeggioClip("LevelUp", new[] { 523.25f, 659.25f, 783.99f, 1046.5f }, 0.08f, 0.55f);
        rewardSelectClip = CreateToneClip("RewardSelect", 0.08f, 880f, 1174.66f, 0.45f, WaveType.Sine);
        gameOverClip = CreateArpeggioClip("GameOver", new[] { 392f, 329.63f, 261.63f, 196f }, 0.16f, 0.7f);
    }

    private AudioClip CreateBgmClip()
    {
        const int sampleRate = 44100;
        const float seconds = 8f;
        int samples = Mathf.CeilToInt(sampleRate * seconds);
        float[] data = new float[samples];
        float[] notes = { 196f, 246.94f, 293.66f, 329.63f, 293.66f, 246.94f, 220f, 246.94f };
        float noteLength = seconds / notes.Length;

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            int noteIndex = Mathf.FloorToInt(t / noteLength) % notes.Length;
            float freq = notes[noteIndex];
            float phase = t * freq * Mathf.PI * 2f;
            float melody = Mathf.Sin(phase) * 0.12f;
            float harmony = Mathf.Sin(phase * 0.5f) * 0.06f;
            float beat = Mathf.Sin(t * 2f * Mathf.PI) > 0.82f ? 0.04f : 0f;

            data[i] = melody + harmony + beat;
        }

        AudioClip clip = AudioClip.Create("DefaultBgm", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreateArpeggioClip(string clipName, float[] notes, float noteSeconds, float volume)
    {
        const int sampleRate = 44100;
        int samples = Mathf.CeilToInt(sampleRate * noteSeconds * notes.Length);
        float[] data = new float[samples];
        int samplesPerNote = Mathf.CeilToInt(sampleRate * noteSeconds);

        for (int i = 0; i < samples; i++)
        {
            int noteIndex = Mathf.Min(i / samplesPerNote, notes.Length - 1);
            float localT = (float)(i % samplesPerNote) / samplesPerNote;
            float t = (float)i / sampleRate;
            float envelope = Mathf.Sin(localT * Mathf.PI);
            data[i] = Mathf.Sin(t * notes[noteIndex] * Mathf.PI * 2f) * envelope * volume;
        }

        AudioClip clip = AudioClip.Create(clipName, samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreateToneClip(string clipName, float seconds, float startFrequency, float endFrequency, float volume, WaveType waveType)
    {
        const int sampleRate = 44100;
        int samples = Mathf.CeilToInt(sampleRate * seconds);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float progress = (float)i / samples;
            float freq = Mathf.Lerp(startFrequency, endFrequency, progress);
            float t = (float)i / sampleRate;
            float envelope = 1f - progress;
            float wave = EvaluateWave(t, freq, waveType);
            data[i] = wave * envelope * volume;
        }

        AudioClip clip = AudioClip.Create(clipName, samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private float EvaluateWave(float time, float frequency, WaveType waveType)
    {
        float phase = time * frequency;

        switch (waveType)
        {
            case WaveType.Square:
                return Mathf.Sign(Mathf.Sin(phase * Mathf.PI * 2f));
            case WaveType.Triangle:
                return Mathf.PingPong(phase * 2f, 2f) - 1f;
            case WaveType.Saw:
                return Mathf.Repeat(phase, 1f) * 2f - 1f;
            case WaveType.Noise:
                return Random.Range(-1f, 1f);
            default:
                return Mathf.Sin(phase * Mathf.PI * 2f);
        }
    }

    private enum WaveType
    {
        Sine,
        Square,
        Triangle,
        Saw,
        Noise
    }
}
