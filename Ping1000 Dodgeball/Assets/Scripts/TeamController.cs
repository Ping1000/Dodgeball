using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamController : MonoBehaviour
{
    public List<ActionController> members;
    [HideInInspector]
    public int finishedActing;
    [HideInInspector]
    public int membersBuilt;

    public PhaseController phaseController;

    void Start() {
        isPlanning = false;
        isActing = false;
        membersBuilt = 0;
        if (phaseController == null)
            phaseController = FindObjectOfType<PhaseController>();

        // Instatiate teams here
    }

    public void BeginPlanPhase() {
        StartCoroutine(PlanningPhase());
        membersBuilt = 0;
    }

    public void SelectNewMember() {
        foreach (ActionController member in members) {
            if (!member.areActionsBuilt) {
                member.SelectCharacter();
                break;
            }
        }
    }

    [HideInInspector]
    public bool isPlanning;
    IEnumerator PlanningPhase() {
        isPlanning = true;
        Debug.Log("Beginning planning phase...");
        members[0].SelectCharacter();
        while (membersBuilt < members.Count)
            yield return null;

        // TODO allow player to select characters individually 
        //HashSet<ActionController> membersBuilt = new HashSet<ActionController>();
        //int k = 0;
        //while (membersBuilt.Count < members.Count) {
        //    ActionController member = members[k];
        //    member.SelectCharacter();
        //    yield return new WaitForEndOfFrame();
        //    yield return new WaitUntil(() => !member.isBuilding);
        //    if (member.areActionsBuilt)
        //        membersBuilt.Add(member);
        //    k = (k + 1) % members.Count;
        //}
        //foreach (ActionController m in members) {
        //    if (m != null) {
        //        m.SelectCharacter();
        //        yield return new WaitUntil(() => m.areActionsBuilt);
        //    }
        //}

        foreach (ActionController m in members) {
            if (m != null) {
                LineController playerLine = m.GetComponent<LineController>();
                if (playerLine != null)
                    playerLine.ClearLines();
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
        membersBuilt = 0;
        isActing = false;
    }
}
