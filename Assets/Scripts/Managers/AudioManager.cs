using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Manager: Handles game audio, effects, voicelines, music
/// </summary>

public enum eMixers { music, effects }
public enum eEffects { farmFire, combatFire, farmEarth, combatEarth, farmWater, combatWater, farmAir, combatAir, harvest, walk, jump, till, castHook, bubblePop}
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [NamedArray(typeof(eMixers))] public AudioMixerGroup[] mixers;
    [NamedArray(typeof(eMixers))] public float[] volume = { 1f, 1f };
    [NamedArray(typeof(eMixers))] private string[] strMixers = { "MusicVol", "EffectsVol" };

    [NamedArray(typeof(eEffects))] public AudioClip[] effectsSounds;

    [SerializeField] private AudioSource BGM;
    [SerializeField] private AudioSource Effects;

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
}
