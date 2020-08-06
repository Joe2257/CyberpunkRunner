using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class MainMenu : MonoBehaviour
{
    [Header("MainMenu")]
    public GameObject settings;
    public GameObject buttons;

    [Header("Settings")]
    public Toggle     mobileToggle;
    public Slider     masterSlider;
    public Slider     fxSlider;
    public Slider     musicSlider;
    public AudioMixer mainMixer;


    void Start()
    {
        InitializePlayerPref();
    }

    //Set the correct options if they have been modified in the settings menu.
    private void InitializePlayerPref()
    {

        if (PlayerPrefs.HasKey("MobileToggle"))
        {
            if (PlayerPrefs.GetInt("MobileToggle") == 1)
                mobileToggle.isOn = true;
            else
                mobileToggle.isOn = false;
        }       

        if (PlayerPrefs.HasKey("Volume"))
        { mainMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("Volume")); }

        if (PlayerPrefs.HasKey("FxVolume"))
        { mainMixer.SetFloat("FXVolume", PlayerPrefs.GetFloat("FxVolume")); }

        if (PlayerPrefs.HasKey("MusicVolume"))
        { mainMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume")); }
    }

    //------------Updates-------------\\

    void Update()
    {
        SetVoulume();
    }

    //------------MainMenu_Buttons-------------\\

    public void OnStartButtonClick()
    {
        SceneManager.LoadScene("City");
    }

    public void OnSettingsButtonClick()
    {
        settings.SetActive(true);
        buttons.SetActive(false);
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }

    //------------Settings_Buttons-------------\\

    public void OnSaveButtonClick()
    {
        PlayerPrefs.SetFloat("Volume", masterSlider.value);
        PlayerPrefs.SetFloat("FxVolume", fxSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);

        settings.SetActive(false);
        buttons.SetActive(true);
    }

    public void OnMobileButtonClick()
    {
       if (mobileToggle.isOn)
           PlayerPrefs.SetInt("Mobile", 1);
       else
           PlayerPrefs.SetInt("Mobile", 0);
    }

    public void SetVoulume()
    {
        mainMixer.SetFloat("MasterVolume", masterSlider.value);
        mainMixer.SetFloat("FXVolume", fxSlider.value);
        mainMixer.SetFloat("MusicVolume", musicSlider.value);
    }

}
