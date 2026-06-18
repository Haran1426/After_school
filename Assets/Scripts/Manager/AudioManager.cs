using UnityEngine;

public enum AudioCue
{
    KnifeThrow,
    ClawSwipe,
    LeafBurst,
    AcornShot,
    EnemyHit,
    EnemyDie,
    PlayerHit,
    ExpPickup,
    LevelUp,
    RewardSelect,
    GameOver,
    LeafWhirlwind,
    BossShockwave
}

public class AudioManager : ManagerBase
{
    AudioSource bgmSource;
    AudioSource sfxSource;

    AudioClip bgmClip;
    AudioClip knifeThrowClip;
    AudioClip clawSwipeClip;
    AudioClip leafBurstClip;
    AudioClip acornShotClip;
    AudioClip enemyHitClip;
    AudioClip enemyDieClip;
    AudioClip playerHitClip;
    AudioClip expPickupClip;
    AudioClip levelUpClip;
    AudioClip rewardSelectClip;
    AudioClip gameOverClip;
    AudioClip leafWhirlwindClip;
    AudioClip bossShockwaveClip;

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
            case AudioCue.ClawSwipe:
                return clawSwipeClip;
            case AudioCue.LeafBurst:
                return leafBurstClip;
            case AudioCue.AcornShot:
                return acornShotClip;
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
            case AudioCue.LeafWhirlwind:
                return leafWhirlwindClip;
            case AudioCue.BossShockwave:
                return bossShockwaveClip;
            default:
                return null;
        }
    }

    private void CreateDefaultClips()
    {
        bgmClip = LoadClip("Audio/BGM/bgm_forest_survival_loop", CreateBgmClip);

        clawSwipeClip = LoadClip("Audio/SFX/sfx_claw_swipe",
            () => CreateToneClip("ClawSwipe", 0.08f, 740f, 440f, 0.45f, WaveType.Triangle));
        knifeThrowClip = clawSwipeClip;
        leafBurstClip = LoadClip("Audio/SFX/sfx_leaf_burst",
            () => CreateToneClip("LeafBurst", 0.18f, 520f, 240f, 0.45f, WaveType.Noise));
        acornShotClip = LoadClip("Audio/SFX/sfx_acorn_shot",
            () => CreateToneClip("AcornShot", 0.08f, 660f, 360f, 0.45f, WaveType.Triangle));
        enemyHitClip = LoadClip("Audio/SFX/sfx_enemy_hit",
            () => CreateToneClip("EnemyHit", 0.06f, 220f, 110f, 0.55f, WaveType.Noise));
        enemyDieClip = LoadClip("Audio/SFX/sfx_enemy_die",
            () => CreateToneClip("EnemyDie", 0.16f, 180f, 70f, 0.7f, WaveType.Saw));
        playerHitClip = LoadClip("Audio/SFX/sfx_player_hit",
            () => CreateToneClip("PlayerHit", 0.12f, 120f, 70f, 0.75f, WaveType.Square));
        expPickupClip = LoadClip("Audio/SFX/sfx_exp_pickup",
            () => CreateToneClip("ExpPickup", 0.09f, 660f, 990f, 0.45f, WaveType.Sine));
        levelUpClip = LoadClip("Audio/SFX/sfx_level_up",
            () => CreateArpeggioClip("LevelUp", new[] { 523.25f, 659.25f, 783.99f, 1046.5f }, 0.08f, 0.55f));
        rewardSelectClip = LoadClip("Audio/SFX/sfx_reward_select",
            () => CreateToneClip("RewardSelect", 0.08f, 880f, 1174.66f, 0.45f, WaveType.Sine));
        gameOverClip = LoadClip("Audio/SFX/sfx_game_over",
            () => CreateArpeggioClip("GameOver", new[] { 392f, 329.63f, 261.63f, 196f }, 0.16f, 0.7f));
        leafWhirlwindClip = LoadClip("Audio/SFX/sfx_leaf_whirlwind",
            () => CreateLayeredToneClip("LeafWhirlwind", 0.22f, 760f, 420f, 0.38f, WaveType.Sine, WaveType.Noise));
        bossShockwaveClip = LoadClip("Audio/SFX/sfx_boss_shockwave",
            () => CreateLayeredToneClip("BossShockwave", 0.32f, 120f, 52f, 0.75f, WaveType.Saw, WaveType.Noise));
    }

    private AudioClip LoadClip(string resourcesPath, System.Func<AudioClip> fallbackFactory)
    {
        AudioClip clip = Resources.Load<AudioClip>(resourcesPath);
        return clip != null ? clip : fallbackFactory();
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

    private AudioClip CreateLayeredToneClip(string clipName, float seconds, float startFrequency, float endFrequency, float volume, WaveType mainWave, WaveType textureWave)
    {
        const int sampleRate = 44100;
        int samples = Mathf.CeilToInt(sampleRate * seconds);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float progress = (float)i / samples;
            float freq = Mathf.Lerp(startFrequency, endFrequency, progress);
            float t = (float)i / sampleRate;
            float attack = Mathf.Clamp01(progress / 0.08f);
            float release = 1f - Mathf.SmoothStep(0.25f, 1f, progress);
            float envelope = attack * release;
            float main = EvaluateWave(t, freq, mainWave);
            float texture = EvaluateWave(t, freq * 1.7f, textureWave) * 0.22f;
            data[i] = Mathf.Clamp((main + texture) * envelope * volume, -1f, 1f);
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
