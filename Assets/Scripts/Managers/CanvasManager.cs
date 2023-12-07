using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button startButon;
    [SerializeField] Button settingsButton;
    [SerializeField] Button backButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button continueButton;
    [SerializeField] Button mainMenuButton;
    [SerializeField] Button resumeButton;

    [Header("Menus")]
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject gameOverMenu;
    [SerializeField] GameObject normalHUD;

    [Header("Text")]
    [SerializeField] Text masterVolSliderText;
    [SerializeField] Text musicVolSliderText;
    [SerializeField] Text sfxVolSliderText;

    [Header("Counters")]
    [SerializeField] GameObject HPCounter;
    [SerializeField] GameObject ScoreCounter;
    [SerializeField] GameObject LivesCounter;
    Animator[] HPCounterArr;
    Animator[] ScoreCounterArr;
    Animator[] LivesCounterArr;


    [Header("Slider")]
    [SerializeField] Slider masterVolSlider;
    [SerializeField] Slider musicVolSlider;
    [SerializeField] Slider sfxVolSlider;

    [Header("Toggle")]
    [SerializeField] Toggle muteToggle;

    // Start is called before the first frame update
    void Start()
    {
        if (startButon)
            startButon.onClick.AddListener(() => GameManager.Instance.LoadLevel(++GameManager.Instance.currentSceneIndex));
        if (settingsButton)
            settingsButton.onClick.AddListener(ShowSettingsMenu);
        if (backButton)
            backButton.onClick.AddListener(GoBack);
        if (quitButton)
            quitButton.onClick.AddListener(Quit);
        if (continueButton)
            continueButton.onClick.AddListener(RestartLevel);
        if (mainMenuButton)
            mainMenuButton.onClick.AddListener(goToMainMenu);
        if (resumeButton)
            resumeButton.onClick.AddListener(ResumePlay);

        // Volume
        if (masterVolSlider)
        {
            masterVolSlider.onValueChanged.AddListener((value) => UpdateMasterVolume(value));
            if (masterVolSliderText)
                masterVolSliderText.text = masterVolSlider.value.ToString();
        }
        if (musicVolSlider)
        {
            musicVolSlider.onValueChanged.AddListener((value) => UpdateMusicVolume(value));
            if (masterVolSliderText)
                musicVolSliderText.text = musicVolSlider.value.ToString();
        }
        if (sfxVolSlider)
        {
            sfxVolSlider.onValueChanged.AddListener((value) => UpdateSFXVolume(value));
            if (sfxVolSliderText)
                sfxVolSliderText.text = sfxVolSlider.value.ToString();
        }
        if (muteToggle)
        {
            muteToggle.onValueChanged.AddListener((value) => MuteVolume(value));
        }

        // HUD
        if (HPCounter)
        {
            HPCounterArr = GetAnimators(HPCounter);
            GameManager.Instance.OnHPValueChanged.AddListener((value) => UpdateHP(value));
        }
        if (ScoreCounter)
        {
            ScoreCounterArr = GetAnimators(ScoreCounter);
            GameManager.Instance.OnScoreValueChanged.AddListener((value) => UpdateScore(value));
        }
        if (LivesCounter)
        {
            LivesCounterArr = GetAnimators(LivesCounter);
            GameManager.Instance.OnLivesValueChanged.AddListener((value) => UpdateLives(value));
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!pauseMenu) return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            pauseMenu.SetActive(true);
            normalHUD.SetActive(false);
            Time.timeScale = 0;
            //other stuff
        }
        else
        {
            //stuff
        }
    }

    private void MuteVolume(bool value)
    {
        muteToggle.GetComponentInChildren<Text>().text = value ? "Muted" : "Not Muted";
    }

    private void UpdateMasterVolume(float value)
    {
        masterVolSliderText.text = value.ToString();
    }

    private void UpdateMusicVolume(float value)
    {
        musicVolSliderText.text = value.ToString();
    }

    private void UpdateSFXVolume(float value)
    {
        sfxVolSliderText.text = value.ToString();
    }



    private void UpdateHP(int value)
    {
        for (int i = 0; i < HPCounterArr.Length; i++)
        {
            if (i < value)
                HPCounterArr[i].SetTrigger("HPgained");
            else
                HPCounterArr[i].SetTrigger("HPlost");
        }
    }

    private void UpdateScore(int value)
    {
        string strVal = value.ToString().PadLeft(7, '0');
        for (int i = 0; i < strVal.Length; ++i)
        {
            UpdateDigit(int.Parse(strVal[i].ToString()), ScoreCounterArr[i]);
        }
    }

    private void UpdateLives(int value)
    {
        string strVal = value.ToString().PadLeft(2, '0');
        for (int i = 0; i < strVal.Length; ++i)
        {
            UpdateDigit(int.Parse(strVal[i].ToString()), LivesCounterArr[i]);
        }
    }

    private void ShowSettingsMenu()
    {
        settingsMenu.SetActive(true);
        if (mainMenu)
            mainMenu.SetActive(false);
        else if (pauseMenu)
            pauseMenu.SetActive(false);
    }
    private void GoBack()
    {
        settingsMenu.SetActive(false);
        if (mainMenu)
            mainMenu.SetActive(true);
        else if (pauseMenu)
        {
            pauseMenu.SetActive(true);
        }
    }

    private void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    private void goToMainMenu()
    {
        Time.timeScale = 1;
        GameManager.Instance.currentSceneIndex = 1;
        GameManager.Instance.LoadLevel(GameManager.Instance.currentSceneIndex);
    }

    private void ResumePlay()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        normalHUD.SetActive(true);
        UpdateScore(GameManager.Instance.currentScore);
        UpdateLives(GameManager.Instance.currentLives);
        UpdateHP(GameManager.Instance.currentHP);
        // sum pther stuff
    }

    private void RestartLevel()
    {
        GameManager.Instance.currentSceneIndex = GameManager.Instance.contSceneIdex;
        GameManager.Instance.LoadLevel(GameManager.Instance.currentSceneIndex);
    }

    private Animator[] GetAnimators(GameObject go)
    {
        return go.GetComponentsInChildren<Animator>();
    }

    private void UpdateDigit(int num, Animator anim)
    {
        switch (num)
        {
            case 0:
                anim.SetTrigger("Show0");
                break;
            case 1:
                anim.SetTrigger("Show1");
                break;
            case 2:
                anim.SetTrigger("Show2");
                break;
            case 3:
                anim.SetTrigger("Show3");
                break;
            case 4:
                anim.SetTrigger("Show4");
                break;
            case 5:
                anim.SetTrigger("Show5");
                break;
            case 6:
                anim.SetTrigger("Show6");
                break;
            case 7:
                anim.SetTrigger("Show7");
                break;
            case 8:
                anim.SetTrigger("Show8");
                break;
            case 9:
                anim.SetTrigger("Show9");
                break;
        }
    }
}
