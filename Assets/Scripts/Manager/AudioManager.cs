using UnityEngine;

public class AudioManager : ManagerBase<AudioManager>
{
    AudioSource bgmSource;
    AudioSource sfxSource;

    float bgmVolume = 1f;
    float sfxVolume = 1f;

    protected override void Awake()
    {
        base.Awake();

        bgmSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        bgmSource.loop = true;

        ApplyVolume();
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
}
