using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamController : MonoBehaviour
{
    public List<ActionController> members;
    [HideInInspector]
    public int finishedActing;

    public PhaseController phaseController;

    void Start() {
        isPlanning = false;
        isActing = false;
        if (phaseController == null)
            phaseController = FindObjectOfType<PhaseController>();

        // Instatiate teams here
    }

    void Update() {
        // TESTING
        //if (Input.GetKeyDown(KeyCode.Space)) {
        //    BeginPlanPhase();
        //} else if (Input.GetKeyDown(KeyCode.Return)) {
        //    BeginActionPhase();
        //}
    }

    public void BeginPlanPhase() {
        StartCoroutine(PlanningPhase());
    }

    [HideInInspector]
    public bool isPlanning;
    IEnumerator PlanningPhase() {
        isPlanning = true;
        Debug.Log("Beginning planning phase...");

        // TODO allow player to select characters individually 
        foreach (ActionController m in members) {
            if (m != null) {
                m.SelectCharacter();
                yield return new WaitUntil(() => m.areActionsBuilt);
            }
        }


        Debug.Log("Planning phase complete.");
        isPlanning = false;
    }
    public void BeginActionPhase() {
        StartCoroutine(ActionPhase());
    }

    [HideInInspector]
    public bool isActing;
    IEnumerator ActionPhase() {
        isActing = true;
        Debug.Log("Beginning action phase...");
        foreach (ActionController m in members) {
            m.ExecuteActions();
        }
        finishedActing = 0;
        yield return new WaitForEndOfFrame();
        // surely there's a better way to manage this
        //foreach (ActionController m in members) {
        //    yield return new WaitUntil(() => !(m.isActing));
        //}
        yield return new WaitUntil(() => finishedActing >= members.Count);
        Debug.Log("Action phase complete.");
        isActing = false;
    }
}
