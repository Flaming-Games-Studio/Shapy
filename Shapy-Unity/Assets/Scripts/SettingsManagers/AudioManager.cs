using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager AM;

    [SerializeField]
    private AudioMixer am;
    [SerializeField]
    private string masterVol = "MasterVol";
    [SerializeField]
    private string musicVol = "MusicVol";
    [SerializeField]
    private string sfxVol = "SfxVol";
    [SerializeField]
    private string ambientVol = "AmbientVol";
    [SerializeField]
    private Slider[] volumeSliders;

    private float sliderVolumeValue;
    private readonly float multiplier = 30f;

    void Start()
    {
        if (AM == null)
        {
            AM = this;
            DontDestroyOnLoad(this);
        }  
        else if (AM != this)
        {
            Destroy(AM.gameObject);
            AM = this;
        } 
        SavedVolumeSettings();
    }

    public void SavedVolumeSettings()
    {
        volumeSliders[0].value = PlayerPrefs.GetFloat(masterVol, 1);
        sliderVolumeValue = Mathf.Log10(volumeSliders[0].value) * multiplier;
        am.SetFloat(masterVol, sliderVolumeValue);

        volumeSliders[1].value = PlayerPrefs.GetFloat(musicVol, 1);
        sliderVolumeValue = Mathf.Log10(volumeSliders[1].value) * multiplier;
        am.SetFloat(musicVol, sliderVolumeValue);

        volumeSliders[2].value = PlayerPrefs.GetFloat(sfxVol, 1);
        sliderVolumeValue = Mathf.Log10(volumeSliders[2].value) * multiplier;
        am.SetFloat(sfxVol, sliderVolumeValue);

        volumeSliders[3].value = PlayerPrefs.GetFloat(ambientVol, 1);
        sliderVolumeValue = Mathf.Log10(volumeSliders[3].value) * multiplier;
        am.SetFloat(ambientVol, sliderVolumeValue);
    }

    #region MasterVolume

    public void MasterVolumeSettings()
    {
        sliderVolumeValue = Mathf.Log10(volumeSliders[0].value) * multiplier;
        am.SetFloat("MasterVol", sliderVolumeValue);
        PlayerPrefs.SetFloat(masterVol, sliderVolumeValue);
    }

    public void SetMasterVolume(Slider slider)
    {
        float value = Mathf.Log10(slider.value) * multiplier;
        am.SetFloat(masterVol, value);
        PlayerPrefs.SetFloat(masterVol, slider.value);
    }

    #endregion

    #region MusicVolume

    public void MusicVolumeSettings()
    {
        sliderVolumeValue = Mathf.Log10(volumeSliders[1].value) * multiplier;
        am.SetFloat("MusicVol", sliderVolumeValue);
        PlayerPrefs.SetFloat(musicVol, sliderVolumeValue);
    }

    public void SetMusicVolume(Slider slider)
    {
        float value = Mathf.Log10(slider.value) * multiplier;
        am.SetFloat(musicVol, value);
        PlayerPrefs.SetFloat(musicVol, slider.value);
    }

    #endregion

    #region SFXVolume

    public void SFXVolumeSettings()
    {
        sliderVolumeValue = Mathf.Log10(volumeSliders[2].value) * multiplier;
        am.SetFloat("SfxVol", sliderVolumeValue);
        PlayerPrefs.SetFloat(sfxVol, sliderVolumeValue);
    }

    public void SetSfxVolume(Slider slider)
    {
        float value = Mathf.Log10(slider.value) * multiplier;
        am.SetFloat(sfxVol, value);
        PlayerPrefs.SetFloat(sfxVol, slider.value);
    }

    #endregion

    #region AmbientVolume

    public void AmbientVolumeSettings()
    {
        sliderVolumeValue = Mathf.Log10(volumeSliders[3].value) * multiplier;
        am.SetFloat("AmbientVol", sliderVolumeValue);
        PlayerPrefs.SetFloat(ambientVol, sliderVolumeValue);
    }

    public void SetAmbienceVolume(Slider slider)
    {
        float value = Mathf.Log10(slider.value) * multiplier;
        am.SetFloat(ambientVol, value);
        PlayerPrefs.SetFloat(ambientVol, slider.value);
    }

    #endregion
}

