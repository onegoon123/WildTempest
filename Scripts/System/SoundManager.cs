using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSourcePrefab;
    [SerializeField] private int sfxSourcePoolSize = 10;

    private List<AudioSource> sfxSources;

    [Header("Audio Clips")]
    public AudioClip[] bgmClips;
    public AudioClip[] sfxClips;

    private Dictionary<string, AudioClip> sfxDict = new();
    // 루프 SFX용 사운드 소스 (clip 이름 기준으로 관리)
    private Dictionary<string, AudioSource> loopSfxSources = new();

    [SerializeField]
    private float bgmVolume = .2f;
    private float bgmPercent = .5f;
    public float BGMPercent
    {
        get { return bgmPercent; } 
        set
        {
            bgmSource.volume = bgmVolume * value;
            bgmPercent = value;
        }
    }
    [SerializeField]
    public float sfxVolume = .2f;
    public float sfxPercent = .5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // AudioClip 이름 → Clip 매핑
        foreach (var clip in sfxClips)
            sfxDict[clip.name] = clip;

        // SFX 풀 생성
        sfxSources = new List<AudioSource>();
        for (int i = 0; i < sfxSourcePoolSize; i++)
        {
            var source = Instantiate(sfxSourcePrefab, transform);
            source.gameObject.SetActive(false);
            sfxSources.Add(source);
        }

        bgmPercent = PlayerPrefs.GetFloat("BGM", 0.5f);
        sfxPercent = PlayerPrefs.GetFloat("SFX", 0.5f);
    }

    bool isPausedByTimeScale = false;
    void Update()
    {
        if (Time.timeScale == 0f && !isPausedByTimeScale)
        {
            PauseAllLoopSFX();
            isPausedByTimeScale = true;
        }
        else if (Time.timeScale > 0f && isPausedByTimeScale)
        {
            ResumeAllLoopSFX();
            isPausedByTimeScale = false;
        }
    }

    private void PauseAllLoopSFX()
    {
        foreach (var pair in loopSfxSources)
        {
            pair.Value.Pause();
        }
    }

    private void ResumeAllLoopSFX()
    {
        foreach (var pair in loopSfxSources)
        {
            pair.Value.UnPause();
        }
    }

    // 효과음 재생
    public static void PlaySFX(string clipName)
    {
        if (!Instance.sfxDict.TryGetValue(clipName, out var clip))
        {
            Debug.LogWarning($"SFX '{clipName}' not found.");
            return;
        }

        Instance.PlaySFX(clip);
    }

    // 효과음 재생
    public void PlaySFX(AudioClip clip)
    {
        var source = GetAvailableSFXSource();
        if (source == null)
        {
            Debug.Log("사운드 재생에 실패했습니다");
            return;
        }
        source.clip = clip;
        source.volume = sfxVolume * sfxPercent;
        source.gameObject.SetActive(true);
        source.Play();
    }

    public static void PlayLoopSFX(string clipName, float time = -1, bool unTimeScale = false)
    {
        if (Instance.loopSfxSources.ContainsKey(clipName))
        {
            // 이미 재생 중이면 무시
            return;
        }

        if (!Instance.sfxDict.TryGetValue(clipName, out var clip))
        {
            Debug.LogWarning($"Loop SFX '{clipName}' not found.");
            return;
        }

        var source = Instantiate(Instance.sfxSourcePrefab, Instance.transform);
        source.clip = clip;
        source.loop = true;
        source.spatialBlend = 0f;
        source.volume = Instance.sfxVolume;
        source.Play();

        Instance.loopSfxSources[clipName] = source;

        if (time > 0)
        {
            Instance.StartCoroutine(Instance.DeactivateAfterTime(source, time, unTimeScale));
        }
    }

    public static void StopLoopSFX(string clipName)
    {
        if (!Instance.loopSfxSources.TryGetValue(clipName, out var source))
            return;

        source.Stop();
        Destroy(source.gameObject); // 메모리 정리
        Instance.loopSfxSources.Remove(clipName);
    }

    public static void StopAllLoopSFX()
    {
        foreach (var source in Instance.loopSfxSources)
        {
            source.Value.Stop();
            Destroy(source.Value.gameObject); // 메모리 정리
        }
        Instance.loopSfxSources.Clear();
    }

    // BGM 재생
    public static void PlayBGM(int index, bool loop = true)
    {
        if (index < 0 || index >= Instance.bgmClips.Length) return;

        Instance.bgmSource.clip = Instance.bgmClips[index];
        Instance.bgmSource.loop = loop;
        Instance.bgmSource.volume = Instance.bgmVolume * Instance.BGMPercent;
        Instance.bgmSource.DOKill();
        Instance.bgmSource.Play();
    }

    public static void StopBGM()
    {
        Instance.bgmSource.Stop();
    }

    public static void FadeBGM(float targetVolume, float duration)
    {
        Instance.bgmSource.DOFade(targetVolume * Instance.bgmVolume, duration).SetUpdate(true);
    }

    public static void PauseBGM()
    {
        Instance.bgmSource.Pause();
    }

    public static void UnPauseBGM()
    {
        Instance.bgmSource.UnPause();
    }

    public static void SetVolume(float sfxVolume, float bgmVolume)
    {
        foreach (var sfx in Instance.sfxSources)
            sfx.volume = sfxVolume;
        Instance.bgmSource.volume = bgmVolume;
    }

    // AudioSource 풀에서 반환
    private AudioSource GetAvailableSFXSource()
    {
        foreach (var source in sfxSources)
        {
            if (!source.isPlaying)
                return source;
        }

        // 전부 사용 중이면: 가장 오래 재생 중인 소스를 선택해서 덮어쓰기
        AudioSource oldestSource = null;
        float oldestTime = float.MinValue;

        foreach (var source in sfxSources)
        {
            if (source.time > oldestTime)
            {
                oldestTime = source.time;
                oldestSource = source;
            }
        }

        if (oldestSource != null)
        {
            Debug.Log($"모든 SFX AudioSource가 사용 중입니다. 가장 오래된 소스를 강제로 재사용합니다: {oldestSource.clip?.name}");
            return oldestSource;
        }

        return null;
    }

    private System.Collections.IEnumerator DeactivateAfterTime(AudioSource source, float time, bool unTimeScale)
    {
        if (unTimeScale)
            yield return new WaitForSecondsRealtime(time);
        else
            yield return new WaitForSeconds(time);
        

        if (source != null)
        {
            source.Stop();
            Destroy(source.gameObject); // 메모리 정리
            loopSfxSources.Remove(source.clip.name);
        }
    }
}
