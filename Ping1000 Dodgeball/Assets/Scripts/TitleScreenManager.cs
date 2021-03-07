using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject singlePlayerPanel;
    public GameObject optionsPanel;
    public GameObject tutorialPanel;
    public AudioSource _src;
    public static float srcVolume = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        _src.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleSinglePlayerMenu() {
        mainPanel.SetActive(!mainPanel.activeInHierarchy);
        singlePlayerPanel.SetActive(!singlePlayerPanel.activeInHierarchy);
    }

    public void LoadScene(int buildIndex) {
        SceneManager.LoadScene(buildIndex);
    }

    public void ToggleTutorial(bool show) {
        tutorialPanel.SetActive(show);
    }

    public void SetVolume(float value) {

    }
}
