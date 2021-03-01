using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    public static WinScreen instance;

    public Text winText;
    public GameObject winCanvas;

    void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Show(string msg) {
        instance.winText.text = msg;
        instance.winCanvas.SetActive(true);
    }

    public void GotoMainMenu() {
        SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
