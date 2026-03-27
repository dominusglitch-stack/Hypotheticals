using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsScript : MonoBehaviour
{
    static SettingsScript _instance = null;
    static public SettingsScript instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<SettingsScript>();

            return _instance;
        }
    }

    [SerializeField] private TMP_Dropdown resDropDown;
    [SerializeField] private TMP_Dropdown displayDropDown;
    //[SerializeField] private TMP_Dropdown refreshDropDown;

    Resolution[] AllResolutions;
    List<Resolution> filteredResolutions = new List<Resolution>();
    int SelectedResolution;
    List<string> resolutionStringList = new List<string>();
    //List<string> refreshRateStringList = new List<string>();
    FullScreenMode[] AllDisplayModes = new FullScreenMode[] { FullScreenMode.ExclusiveFullScreen, FullScreenMode.FullScreenWindow, FullScreenMode.Windowed };
    int SelectedDisplay;
    //Resolution savedSelectedResolution;
    RefreshRate selectedRefreshRate;

    [SerializeField] private AudioMixer gameMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private TextMeshProUGUI masterSliderPercent;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TextMeshProUGUI musicSliderPercent;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxSliderPercent;
    [SerializeField] private TMP_InputField caseSeedInput;
    [SerializeField] private TextMeshProUGUI caseSeed;
    [SerializeField] private GameObject replayTutorialButton;

    [SerializeField] private Toggle hardToggle;
    [SerializeField] private Toggle noirToggle;

    [SerializeField] public bool hardMode;
    [SerializeField] public bool noirMode;
    [SerializeField] public bool tutorialMode;

    private void Start()
    {
        AllResolutions = Screen.resolutions;

        selectedRefreshRate = Screen.currentResolution.refreshRateRatio;

        foreach (Resolution res in AllResolutions)
        {
            if(res.refreshRateRatio.ToString() == selectedRefreshRate.ToString())
            {
                resolutionStringList.Add(res.ToString());
                filteredResolutions.Add(res);
            }
            
            //if(!refreshRateStringList.Contains(res.refreshRateRatio.ToString()))
            //{
            //    refreshRateStringList.Add(res.refreshRateRatio.ToString());
            //}
        }

        resDropDown.AddOptions(resolutionStringList);

        for (int i = 0; i < resolutionStringList.Count; i++)
        {
            if (resolutionStringList[i] == Screen.currentResolution.ToString())
            {
                resDropDown.value = i;
            }
        }

        displayDropDown.value = 1;

        //refreshDropDown.AddOptions(refreshRateStringList);

        Tutorial();

        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
            LoadBools();
            LoadResolution();
            LoadSeed();
        }
        else
        {
            SetMusicVolume();
            SetSFXVolume();
            SetMasterVolume();
            SetDifficulty();
            SetNoirMode();
        }
    }

    public void ChangeResolution()
    {
        SelectedResolution = resDropDown.value;
        SelectedDisplay = displayDropDown.value;
        Screen.SetResolution(filteredResolutions[SelectedResolution].width, filteredResolutions[SelectedResolution].height, AllDisplayModes[SelectedDisplay]);
        PlayerPrefs.SetInt("savedResIndex", resDropDown.value);
    }

    public void ChangeFullscreen()
    {
        SelectedDisplay = displayDropDown.value;
        Screen.fullScreenMode = AllDisplayModes[SelectedDisplay];
        PlayerPrefs.SetInt("savedDisplayIndex", displayDropDown.value);
    }

    /*
    public void ChangeRefresh()
    {

    }
    */

    public void OpenPanel(int whoPressed)
    {
        if (whoPressed == 0)
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    
    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        gameMixer.SetFloat("music", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("musicVolume", volume);

        int percentage = Mathf.FloorToInt(volume * 100);

        musicSliderPercent.text = percentage.ToString() + "%";
    }

    public void SetMasterVolume()
    {
        float volume = masterSlider.value;
        gameMixer.SetFloat("master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("masterVolume", volume);

        int percentage = Mathf.FloorToInt(volume * 100);

        masterSliderPercent.text = percentage.ToString() + "%";
    }

    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        gameMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);

        int percentage = Mathf.FloorToInt(volume * 100);

        sfxSliderPercent.text = percentage.ToString() + "%";
    }

    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume");

        SetMusicVolume();
        SetSFXVolume();
        SetMasterVolume();
    }

    private void LoadBools()
    {
        hardMode = (PlayerPrefs.GetInt("hardMode") != 0);
        noirMode = (PlayerPrefs.GetInt("noirMode") != 0);
        hardToggle.isOn = hardMode;
        noirToggle.isOn = noirMode;
    }

    public void SetDifficulty()
    {
        hardMode = hardToggle.isOn;
        PlayerPrefs.SetInt("hardMode", (hardMode ? 1 : 0));
    }

    public void SetNoirMode()
    {
        noirMode = noirToggle.isOn;
        PlayerPrefs.SetInt("noirMode", (noirMode ? 1 : 0));
        //GameSceneManager.instance.NoirMode(noirMode);
    }

    private void LoadResolution()
    {
        //savedSelectedResolution = new Resolution();
        //savedSelectedResolution.width = PlayerPrefs.GetInt("savedResWidth", Screen.currentResolution.width);
        //savedSelectedResolution.height = PlayerPrefs.GetInt("savedResHeight", Screen.currentResolution.height);

        resDropDown.value = PlayerPrefs.GetInt("savedResIndex", resDropDown.options.Count);
        displayDropDown.value = PlayerPrefs.GetInt("savedDisplayIndex", 1);

        ChangeResolution();
        ChangeFullscreen();

        //float defaultRef = (float)Screen.currentResolution.refreshRateRatio.value;
        //savedSelectedResolution.refreshRateRatio = PlayerPrefs.GetFloat("savedRefRate", defaultRef);
    }

    public void SetSeed()
    {
        int tempCaseSeed = (int)Mathf.Clamp(long.Parse(caseSeedInput.text), int.MinValue, int.MaxValue);

        if (long.Parse(caseSeedInput.text) > int.MaxValue)
        {
            tempCaseSeed = int.MaxValue;
        }

        //Debug.Log(tempCaseSeed);

        if (tempCaseSeed != PlayerPrefs.GetInt("caseSeed"))
        {
            PlayerPrefs.SetInt("hasBeatenThisSeed", 0);
        }

        PlayerPrefs.SetInt("caseSeed", tempCaseSeed);

        caseSeedInput.text = tempCaseSeed.ToString();
    }

    public void LoadSeed()
    {
        if(caseSeedInput != null)
        {
            if (PlayerPrefs.GetInt("caseSeed") == 357764661)
            {
                caseSeedInput.text = "Tutorial";
            }
            else
            {
                caseSeedInput.text = PlayerPrefs.GetInt("caseSeed").ToString();
            }
        }
        else if (caseSeed != null)
        {
            if (PlayerPrefs.GetInt("caseSeed") == 357764661)
            {
                caseSeed.text = "Tutorial";
            }
            else
            {
                caseSeed.text = PlayerPrefs.GetInt("caseSeed").ToString();
            }
        }
    }

    public void Tutorial()
    {
        tutorialMode = PlayerPrefs.GetInt("tutorial", 1) != 0;

        if (tutorialMode)
        {
            if(caseSeedInput != null)
            {
                caseSeedInput.gameObject.SetActive(false);
                replayTutorialButton.gameObject.SetActive(false);
            }
        }
    }

    public void ReplayTutorial()
    {
        PlayerPrefs.SetInt("tutorial", 1);

        tutorialMode = true;

        Tutorial();
    }
}
