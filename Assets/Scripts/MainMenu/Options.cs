using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Options : MonoBehaviour
{
    [SerializeField] private NetworkCharacterControllerPrototype nCCP;
    [SerializeField] private GameObject optionsPanel, mainMenuPanel;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    public void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.options.Clear();
        for (int i = 0; i < resolutions.Length; i++)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData() { text = resolutions[i].ToString() });
        }
    }

    public void SetResolution(int choice)
    {
        Screen.SetResolution(resolutions[choice].width, resolutions[choice].height, FullScreenMode.Windowed, resolutions[choice].refreshRate);
    }

    public void SetLevel(float sliderValue)
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
    }

    public void optionsClick()
    {
        optionsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void returnClick()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void setSens(float input)
    {
        nCCP.rotationSpeed = 1000 * input;
    }

    public void setQuality(int level)
    {
        QualitySettings.SetQualityLevel(level, true);
    }
}
