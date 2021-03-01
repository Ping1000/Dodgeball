﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionChangeButton : MonoBehaviour {
    private ActionController[] ac_list;
    [SerializeField]

    public Image moveImage;
    public Image throwImage;
    public Color selectedColor;
    public Color deselectedColor;

    // Start is called before the first frame update
    void Start() {

    }

    public void SetColors(ActionType act) {
        switch (act) {
            case ActionType.Move:
                moveImage.color = selectedColor;
                throwImage.color = deselectedColor;
                break;
            case ActionType.Throw:
                moveImage.color = deselectedColor;
                throwImage.color = selectedColor;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
