using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Audio Sliders")]
    public Slider mainSlider;
    public Slider musicSlider;
    public Slider fxSlider;

    [Header("Audio Mixers")]
    public AudioMixer audioMixer;

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("QUIT GAME PRESSED");
    }

    public void MainVolume()
    {
        AudioListener.volume = mainSlider.value;
        Debug.Log(mainSlider.value);
    }

    public void MusicVolume()
    {
        audioMixer.SetFloat("musicVol", musicSlider.value);
    }

    public void FXVolume()
    {
        audioMixer.SetFloat("fxVol", fxSlider.value);
    }
}
