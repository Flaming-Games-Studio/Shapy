using UnityEngine;
using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using System;

public class GraphicsManager : MonoBehaviour
{
    //singleton instance
    public static GraphicsManager GM;

    //quality
    [SerializeField]
    private CustomDropdown qualityDropdown;
    private int qualityLevel;

    //this variables represent indexes of dropdown items created through inspector
    private readonly int dropdownIndexOfQualityLevel_LOW = 0;
    private readonly int dropdownIndexOfQualityLevel_HIGH = 1;
    private readonly int dropdownIndexOfQualityLevel_ULTRA = 2;

    //this variables represent quality by its name and its value index in unity
    //this is used this way because in client settings menu we have LOW,MEDIUM,HIGH and no need for all possible levels currently inside project
    private readonly int LOW = 1;
    private readonly int HIGH = 3;
    private readonly int ULTRA = 5;

    //fullscreen
    [SerializeField]
    private SwitchManager fullscreenSwitch;
    private int savedFullscreen;

    //resolution
    [SerializeField]
    private CustomDropdown resolutionDropdown;
    private List<Resolution> resolutions;
    private Resolution savedResolution;

    public void Awake()
    {
        if (GM == null)
        {
            GM = this;
            DontDestroyOnLoad(this);
        }

        else if (GM != this)
        {
            Destroy(GM.gameObject);
            GM = this;
        }
    }

    public void Start()
    {
        LoadSettings();
        InitializeResolutionDropdown();
        SetSelectedResolution(savedResolution);
    }

    #region InitializeResolutionDropdown

    private void InitializeResolutionDropdown()
    {
        resolutions = new List<Resolution>(Screen.resolutions);
        //resolutions.Sort((a, b) => b.width.CompareTo(a.width)); // Sort resolutions by width descending

        resolutions.Sort((a, b) =>
        {
            int widthComparison = b.width.CompareTo(a.width);
            if (widthComparison == 0)
            {
                return b.refreshRateRatio.CompareTo(a.refreshRateRatio);
            }
            return widthComparison;
        });

        resolutionDropdown.dropdownItems.Clear();

        for (int i = 0; i < resolutions.Count; i++)
        {
            Resolution res = resolutions[i];

            //if resolution width and height are lower then 800 and 600 dont create that option
            if (res.width < 800 && res.height < 600)
            {
                break;
            }
            // Create a dropdown item for each resolution
            CustomDropdown.Item newItem = new CustomDropdown.Item();
            double refreshRateRatio3decimals = Math.Round(res.refreshRateRatio.value, 3);
            newItem.itemName = $"{res.width} x {res.height} {refreshRateRatio3decimals} Hz";

            // Add the item to the dropdown list
            resolutionDropdown.dropdownItems.Add(newItem);
        }

        // Setup the dropdown with the new items
        resolutionDropdown.SetupDropdown();

        // Handle selection change event
        resolutionDropdown.dropdownEvent.AddListener(OnResolutionChanged);

        savedResolution = LoadResolution();     
    }

    #endregion

    #region SetSelectedResolution

    private void SetSelectedResolution(Resolution resolution)
    {
        if (savedFullscreen == 1)
        {
            // Check the platform
            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            {
                Screen.SetResolution(resolution.width, resolution.height, FullScreenMode.ExclusiveFullScreen, resolution.refreshRateRatio);
            }
            else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
            {
                Screen.SetResolution(resolution.width, resolution.height, FullScreenMode.FullScreenWindow, resolution.refreshRateRatio);
            }
            
        }
        else if (savedFullscreen == 0)
        {
            Screen.SetResolution(resolution.width, resolution.height, FullScreenMode.Windowed , resolution.refreshRateRatio);
        }
 
        for (int i = 0; i < resolutionDropdown.dropdownItems.Count; i++)
        {
            Resolution res = resolutions[i];         
            if (res.width == savedResolution.width && res.height == savedResolution.height && Math.Round(res.refreshRateRatio.value, 3) == Math.Round(savedResolution.refreshRateRatio.value, 3))
            {
                resolutionDropdown.ChangeDropdownInfo(i);
                break;
            }
        }
    }

    private void OnResolutionChanged(int index)
    {
        Resolution selectedResolution = resolutions[index];
        if (savedFullscreen == 1)
        {
            // Check the platform
            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            {
                Screen.SetResolution(selectedResolution.width, selectedResolution.height, FullScreenMode.ExclusiveFullScreen, selectedResolution.refreshRateRatio);
            }
            else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
            {
                Screen.SetResolution(selectedResolution.width, selectedResolution.height, FullScreenMode.FullScreenWindow, selectedResolution.refreshRateRatio);
            }
            
            SaveResolution(selectedResolution.width, selectedResolution.height , selectedResolution.refreshRateRatio.value);
            fullscreenSwitch.isOn = true;
            fullscreenSwitch.UpdateUI();
        }
        else if (savedFullscreen == 0)
        {
             //windowed mode same for Mac and Windows so no changes needed         
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, FullScreenMode.Windowed, selectedResolution.refreshRateRatio);
 
            SaveResolution(selectedResolution.width, selectedResolution.height, selectedResolution.refreshRateRatio.value);
            fullscreenSwitch.isOn = false;
            fullscreenSwitch.UpdateUI();
        }

    }

    #endregion

    #region SaveSettings

    public static void SaveResolution(int width, int height , double refreshRate)
    {
        PlayerPrefs.SetInt("ScreenWidth", width);
        PlayerPrefs.SetInt("ScreenHeight", height);
        PlayerPrefs.SetFloat("refreshRate", (float)refreshRate);
    }
    public void SaveQualitySettings(int currentQualityIndex)
    {
        PlayerPrefs.SetInt("QualityLevel", currentQualityIndex);
    }
    public void SaveFullscreenSettings(int isFullscreen)
    {
        PlayerPrefs.SetInt("Fullscreen", isFullscreen);
        savedFullscreen = isFullscreen;
    }

    #endregion

    #region LoadSettings

    private static RefreshRate ConvertFloatToRefreshRate(float refreshRateSaved)
    {
        //For simplicity, assume a common denominator for conversion(e.g., 1000)

        int numerator = Mathf.RoundToInt(refreshRateSaved * 1000);
        int denominator = 1000;

        RefreshRate refreshRate = new RefreshRate();

        refreshRate.numerator = (uint)numerator;
        refreshRate.denominator = (uint)denominator;

        return refreshRate;
    }
    public static Resolution LoadResolution()
    {
        int width = PlayerPrefs.GetInt("ScreenWidth", Screen.currentResolution.width);
        int height = PlayerPrefs.GetInt("ScreenHeight", Screen.currentResolution.height);
        float refreshRateSaved = PlayerPrefs.GetFloat("refreshRate", (float)Screen.currentResolution.refreshRateRatio.value);

        RefreshRate refreshRate = ConvertFloatToRefreshRate(refreshRateSaved);
        return new Resolution { width = width, height = height , refreshRateRatio = refreshRate};
    }
    private void LoadSettings()
    {
        //quality settings load
        qualityLevel = PlayerPrefs.GetInt("QualityLevel", ULTRA);
        QualitySettings.SetQualityLevel(qualityLevel);
        if (qualityLevel == LOW)
        {
            qualityDropdown.ChangeDropdownInfo(dropdownIndexOfQualityLevel_LOW);
        }
        else if (qualityLevel == HIGH)
        {
            qualityDropdown.ChangeDropdownInfo(dropdownIndexOfQualityLevel_HIGH);
        }
        else if (qualityLevel == ULTRA)
        {
            qualityDropdown.ChangeDropdownInfo(dropdownIndexOfQualityLevel_ULTRA);
        }

        //fullscreen settings load
        savedFullscreen = PlayerPrefs.GetInt("Fullscreen", 1);
        if (savedFullscreen == 1)
        {
            SetFullscreenON();
            fullscreenSwitch.isOn = true;
            fullscreenSwitch.UpdateUI();
        }
        else if (savedFullscreen == 0)
        {
            SetFullscreenOFF();
            fullscreenSwitch.isOn = false;
            fullscreenSwitch.UpdateUI();
        }
    }

    #endregion

    #region SetQuality

    public void SetQualityToLow()
    {
        QualitySettings.SetQualityLevel(LOW);
        SaveQualitySettings(LOW);
    }
    public void SetQualityToMedium()
    {
        QualitySettings.SetQualityLevel(HIGH);
        SaveQualitySettings(HIGH);
    }
    public void SetQualityToHigh()
    {
        QualitySettings.SetQualityLevel(ULTRA);
        SaveQualitySettings(ULTRA);
    }

    #endregion

    #region SetFullscreen

    public void SetFullscreenON()
    {
        // Set to fullscreen
        Screen.fullScreen = true;
        // Check the platform
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
       
        SaveFullscreenSettings(1);
    }
    public void SetFullscreenOFF()
    {
        Screen.fullScreen = false;
        // Check the platform
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
       
        SaveFullscreenSettings(0);
    }

    #endregion
}