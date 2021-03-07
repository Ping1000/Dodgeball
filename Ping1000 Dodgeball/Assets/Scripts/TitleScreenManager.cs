using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject singlePlayerPanel;
    public GameObject optionsPanel;
    public GameObject tutorialPanel;
    public AudioSource _src;

    public Slider musicSlider;
    public Slider sfxSlider;
    private static float sfxMultiplier = 1f;
    private static float musicMultiplier = 1f;

    private static float defaultThrowVolume = .75f;
    private static float defaultImpactVolume = .5f;
    private static float defaultButtonVolume = .5f;
    private static float defaultActionVolume = .5f;
    private static float defaultMoveVolume = 0.25f;
    private static float defaultMusicVolume = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        // volumeMultiplier = 1;
        //defaultThrowVolume = SFXManager.throwVolume;
        //defaultImpactVolume = SFXManager.impactVolume;
        //defaultButtonVolume = SFXManager.buttonVolume;
        //defaultActionVolume = SFXManager.actionVolume;
        //defaultMoveVolume = SFXManager.moveVolume;
        //defaultMusicVolume = MusicManager.maxVolume;
        _src.volume = defaultMusicVolume;
        musicSlider.value = musicMultiplier;
        sfxSlider.value = sfxMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectDifficulty(string diff) {
        switch (diff) {
            case "easy":
                AIActionController.throwChance = 0.25f;
                AIActionController.getFoodChance = 0.5f;
                AIActionController.inaccuracySize = 3f;
                break;
            case "medium":
                AIActionController.throwChance = 0.5f;
                AIActionController.getFoodChance = 0.5f;
                AIActionController.inaccuracySize = 1.5f;
                break;
            case "hard":
                AIActionController.throwChance = 0.8f;
                AIActionController.getFoodChance = 0.8f;
                AIActionController.inaccuracySize = 1f;
                break;
            case "impossible":
                AIActionController.throwChance = 1f;
                AIActionController.getFoodChance = 1f;
                AIActionController.inaccuracySize = 0f;
                break;
        }
        // LoadScene(2);
    }

    public void ToggleSinglePlayerMenu() {
        mainPanel.SetActive(!mainPanel.activeInHierarchy);
        singlePlayerPanel.SetActive(!singlePlayerPanel.activeInHierarchy);
    }

    public void TweenSubmenuOpen(RectTransform menu) {
        menu.gameObject.SetActive(true);
        menu.localScale = Vector3.zero;
        LeanTween.scale(menu, Vector3.one, 0.5f).setEaseOutBack();
    }

    public void TweenSubmenuClose(RectTransform menu) {
        LeanTween.scale(menu, Vector3.zero, 0.5f).setEaseInBack().
            setOnComplete(() => menu.gameObject.SetActive(false));
    }

    public void LoadScene(int buildIndex) {
        SceneManager.LoadScene(buildIndex);
    }

    public void ToggleTutorial(bool show) {
        tutorialPanel.SetActive(show);
    }

    public void ScaleSFXVolume(float value) {
        SFXManager.throwVolume = Mathf.Clamp(defaultThrowVolume * value, 0, 1);
        SFXManager.impactVolume = Mathf.Clamp(defaultImpactVolume * value, 0, 1);
        SFXManager.buttonVolume = Mathf.Clamp(defaultButtonVolume * value, 0, 1);
        SFXManager.actionVolume = Mathf.Clamp(defaultActionVolume * value, 0, 1);
        SFXManager.moveVolume = Mathf.Clamp(defaultMoveVolume * value, 0, 1);
        SFXManager.throwVolume = Mathf.Clamp(defaultThrowVolume * value, 0, 1);
        sfxMultiplier = value;
    }

    public void ScaleMusicVolume(float value) {
        MusicManager.maxVolume = Mathf.Clamp(defaultMusicVolume * value, 0, 1);
        _src.volume = Mathf.Clamp(defaultMusicVolume * value, 0, 1);
        musicMultiplier = value;
    }
}
