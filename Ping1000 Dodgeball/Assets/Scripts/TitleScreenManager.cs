using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public GameObject tutorialPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadSinglePlayer() {
        SceneManager.LoadScene(1);
    }

    public void ToggleTutorial(bool show) {
        tutorialPanel.SetActive(show);
    }
}
