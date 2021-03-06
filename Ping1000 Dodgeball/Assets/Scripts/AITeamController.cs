using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITeamController : MonoBehaviour
{
    public List<ActionController> members;

    public PhaseController phaseController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void BeginPlanPhase() {
        StartCoroutine(PlanningPhase());
    }

    [HideInInspector]
    public bool isPlanning;
    IEnumerator PlanningPhase() {
        yield return null;
    }

    public void BeginActionPhase() {
        StartCoroutine(ActionPhase());
    }

    public bool isActing;
    IEnumerator ActionPhase() {
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
