using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkTeamController : NetworkBehaviour
{
    public List<NetworkPlayerController> members;
    [HideInInspector]
    public int finishedActing;

    void Start()
    {
        isPlanning = false;
        isActing = false;
    }

    void Update()
    {
        // TESTING
        //if (Input.GetKeyDown(KeyCode.Space)) {
        //    BeginPlanPhase();
        //} else if (Input.GetKeyDown(KeyCode.Return)) {
        //    BeginActionPhase();
        //}
    }
    
    public void BeginPlanPhase()
    {
        if (!this.isLocalPlayer) { return; }
        StartCoroutine(PlanningPhase());
    }

    [SyncVar]
    [HideInInspector]
    public bool isPlanning;
    IEnumerator PlanningPhase()
    {
        isPlanning = true;
        Debug.Log("Beginning planning phase..." + this.name);

        // TODO allow player to select characters individually 
        foreach (NetworkPlayerController m in members)
        {
            if (m != null)
            {
                m.SelectCharacter();
                yield return new WaitUntil(() => m.areActionsBuilt);
            }
        }


        Debug.Log("Planning phase complete." + this.name);
        isPlanning = false;
    }
    public void BeginActionPhase()
    {
        if (!this.isLocalPlayer) { return; }
        StartCoroutine(ActionPhase());
    }

    [SyncVar]
    [HideInInspector]
    public bool isActing;
    IEnumerator ActionPhase()
    {
        isActing = true;
        Debug.Log("Beginning action phase..." + this.name);
        foreach (NetworkPlayerController m in members)
        {
            m.ExecuteActions();
        }
        finishedActing = 0;
        yield return new WaitForEndOfFrame();
        // surely there's a better way to manage this
        //foreach (ActionController m in members) {
        //    yield return new WaitUntil(() => !(m.isActing));
        //}
        yield return new WaitUntil(() => finishedActing >= members.Count);
        Debug.Log("Action phase complete." + this.name);
        isActing = false;
    }
}
