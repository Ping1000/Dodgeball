using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public GameObject tutorialPanel;
    public AudioSource _src;
    public static float srcVolume = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        _src.Play();
    }

    public void LoadSinglePlayer() {
        SceneManager.LoadScene(1);
    }

    public void ToggleTutorial(bool show) {
        tutorialPanel.SetActive(show);
    }
}
