using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Manager: Handles game audio, effects, voicelines, music
/// </summary>

public enum eMixers { music, effects }
public enum eEffects { farmFire, combatFire, farmEarth, combatEarth, farmWater, combatWater, farmAir, combatAir, harvest, footstep, jump, till, castHook, bubblePop, playerHurt, flying, cantFly, potion, upgrade,}

public enum eMusic { mainIsland, fireIsland, waterIsland, waterIslandBossBattle, flightMusic, caveMusic }
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [NamedArray(typeof(eMixers))] public AudioMixerGroup[] mixers;
    [NamedArray(typeof(eMixers))] public float[] volume = { 1f, 1f };
    [NamedArray(typeof(eMixers))] private string[] strMixers = { "MusicVol", "EffectsVol" };

    [NamedArray(typeof(eEffects))] public AudioClip[] effectsSounds;
    [NamedArray(typeof(eMusic))] public AudioClip[] bgmTracks;

    [SerializeField] private AudioSource BGM;
    [SerializeField] private AudioSource Effects;

    [Header("Transition Settings")]
    [SerializeField] private float fadeDuration = 1.5f;
    private Coroutine activeFadeRoutine;
    private float originalBgmVolume;

    // Tracks the history of zones the player has entered
    private List<eMusic> activeZoneHistory = new List<eMusic>();
    [SerializeField] private eMusic absoluteDefaultMusic = eMusic.mainIsland;

    private void Start()
    {
        // 1. Save the default Inspector volume of your BGM source
        originalBgmVolume = BGM.volume;

        // 2. Play the absolute default music immediately on game start
        ExecuteTrackChange(absoluteDefaultMusic);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Destroy New AudioManager");
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void SetMixerLevel(eMixers _mixer, float _soundlevel)
    {
        mixers[(int)_mixer].audioMixer.SetFloat(strMixers[(int)_mixer], Mathf.Log10(_soundlevel) * 20f);
        volume[(int)_mixer] = _soundlevel;
    }

    public static void PlayOneShot(eEffects _effect, Transform sourceTransform, float volume)
    {
        Instance.Effects.PlayOneShot(Instance.effectsSounds[(int)_effect]);
    }
    public static void ChangeTrack(eMusic _music)
    {
        AudioClip targetTrack = Instance.bgmTracks[(int)_music];

        if (Instance.BGM.clip == targetTrack && Instance.BGM.isPlaying)
            return;

        // Stop any fade currently in progress to prevent conflicts
        if (Instance.activeFadeRoutine != null)
        {
            Instance.StopCoroutine(Instance.activeFadeRoutine);
        }

        // Start the fade transition
        Instance.activeFadeRoutine = Instance.StartCoroutine(Instance.FadeTrackRoutine(targetTrack));
    }

    private IEnumerator FadeTrackRoutine(AudioClip newClip)
    {
        // 1. Fade Out
        float startVolume = BGM.volume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            BGM.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }
        BGM.volume = 0f;

        // 2. Swap the Track
        BGM.Stop();
        BGM.clip = newClip;

        if (newClip != null)
        {
            BGM.Play();

            // 3. Fade In
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                BGM.volume = Mathf.Lerp(0f, originalBgmVolume, t / fadeDuration);
                yield return null;
            }
            BGM.volume = originalBgmVolume;
        }
    }
    public static void UpdateZoneMusic(eMusic zoneMusic, bool isEntering)
    {
        if (isEntering)
        {
            // If entering a new zone, add it to the front of our history list
            if (!Instance.activeZoneHistory.Contains(zoneMusic))
            {
                Instance.activeZoneHistory.Insert(0, zoneMusic);
            }
        }
        else
        {
            // If leaving a zone, remove it from our history list
            Instance.activeZoneHistory.Remove(zoneMusic);
        }

        // Determine what track should be playing right now
        eMusic trackToPlay = Instance.absoluteDefaultMusic;
        if (Instance.activeZoneHistory.Count > 0)
        {
            trackToPlay = Instance.activeZoneHistory[0]; // Play the most recent active zone
        }

        // Trigger the actual track swap/fade logic
        Instance.ExecuteTrackChange(trackToPlay);
    }

    private void ExecuteTrackChange(eMusic _music)
    {
        AudioClip targetTrack = bgmTracks[(int)_music];

        if (BGM.clip == targetTrack && BGM.isPlaying)
            return;

        if (activeFadeRoutine != null)
        {
            StopCoroutine(activeFadeRoutine);
        }

        activeFadeRoutine = StartCoroutine(FadeTrackRoutine(targetTrack));
    }

    public static void SetFlightState(bool inFlightMode)
    {
        // Change this enum value to match your exact flight music enum name
        eMusic flightTrack = eMusic.flightMusic;

        if (inFlightMode)
        {
            // If flying, force flight music to the very front of the history stack
            if (!Instance.activeZoneHistory.Contains(flightTrack))
            {
                Instance.activeZoneHistory.Insert(0, flightTrack);
            }
            else
            {
                // If it's already in the list, move it to the front
                Instance.activeZoneHistory.Remove(flightTrack);
                Instance.activeZoneHistory.Insert(0, flightTrack);
            }
        }
        else
        {
            // If landed, remove flight music from the history stack completely
            Instance.activeZoneHistory.Remove(flightTrack);
        }

        // Determine what track should play now based on the remaining history stack
        eMusic trackToPlay = Instance.absoluteDefaultMusic;
        if (Instance.activeZoneHistory.Count > 0)
        {
            trackToPlay = Instance.activeZoneHistory[0];
        }

        Instance.ExecuteTrackChange(trackToPlay);
    }
}
