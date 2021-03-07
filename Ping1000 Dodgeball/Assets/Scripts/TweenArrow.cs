using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenArrow : MonoBehaviour
{
    public float turnTime;
    public float moveHeight;
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.rotateAround(gameObject, Vector3.up, 360, turnTime).setLoopType(LeanTweenType.linear);
        LeanTween.moveLocalY(gameObject, moveHeight, turnTime / 2).setEaseOutQuad().setLoopPingPong();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
