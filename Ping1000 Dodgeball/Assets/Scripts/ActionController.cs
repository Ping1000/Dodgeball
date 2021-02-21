using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// TODO: add "break" feature to allow people to cancel their actions
// TODO: integrate with UI stuff to show people what they're planned action is
// BUG:  If there's an issue with the path, a destination might be skipped.
//       fixed for now by lowering the margins on the navmesh border, not sure
//       if it happens with overlapping paths so i'll have to test more later

public class ActionController : MonoBehaviour
{
    public uint numActions;
    [HideInInspector]
    public bool areActionsBuilt;

    private CharacterAction[] actions;
    private bool canBuildActions;

    private NavMeshAgent _agent;
    
    // Start is called before the first frame update
    void Start()
    {
        actions = new CharacterAction[numActions];
        _agent = GetComponent<NavMeshAgent>();

        canBuildActions = false;
        isBuilding = false;
        isWaiting = false;
        isActing = false;
        areActionsBuilt = false;

        // TESTING
        // SelectCharacter();
    }

    // Update is called once per frame
    void Update()
    {
        if (canBuildActions && !isBuilding) {
            if (building == null)
                building = StartCoroutine(BuildingActions());
            else
                Debug.LogError("Tried to build while already building!");
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            // break out of building actions by calling stopcorouting on building
            // and deleting existing actions (maybe just add a new function to
            // reset the vars and also add to executing actions
        }
    }

    private bool isBuilding;
    Coroutine building;
    /// <summary>
    /// Coroutine for building this character's actions
    /// </summary>
    /// <returns></returns>
    IEnumerator BuildingActions() {
        isBuilding = true;
        canBuildActions = false;

        for (int i = 0; i < numActions; i++) {
            if (waiting == null) {
                waiting = StartCoroutine(WaitingForInput(i));
                yield return new WaitUntil(() => !isWaiting);
            } else
                Debug.LogError("Tried to wait for input while already waiting!");
        }

        isBuilding = false;
        building = null;
        areActionsBuilt = true;
        Debug.Log("Actions built.");
    }

    [HideInInspector]
    public bool isWaiting;
    Coroutine waiting;
    /// <summary>
    /// Coroutine for waiting for input for the action
    /// </summary>
    /// <param name="currentAction">Index of the action in actions to set</param>
    /// <returns></returns>
    IEnumerator WaitingForInput(int currentAction) {
        isWaiting = true;

        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0));

        RaycastHit hit;
        NavMeshHit meshHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
            // WILL NEED TO CASE ON WHAT YOU CLICK ON, FOR NOW JUST MOVE
            //if (NavMesh.SamplePosition(hit.point, out meshHit, 5, 0b0)) {
            //    actions[currentAction] = new CharacterAction(ActionType.Move, this, _agent, meshHit.position);
            //}
            actions[currentAction] = new CharacterAction(ActionType.Move, this, _agent, hit.point);
        } else {
            Debug.Log("Missed a valid raycast target");
        }

        isWaiting = false;
        waiting = null;
    }

    /// <summary>
    /// Execute the built up actions and reset variables when done.
    /// </summary>
    public void ExecuteActions() {
        if (areActionsBuilt)
            StartCoroutine(ExecutingActions());
    }

    [HideInInspector]
    public bool isActing;
    /// <summary>
    /// Coroutine to handle executing the actions sequentially
    /// </summary>
    /// <returns></returns>
    IEnumerator ExecutingActions() {
        isActing = true;
        foreach (CharacterAction a in actions) {
            a.DoAction();
            yield return new WaitUntil(() => !(a.isActing));
        }

        // reset variables when done
        actions = new CharacterAction[numActions]; // should be garbage collected, right?
        canBuildActions = false;
        isBuilding = false;
        isWaiting = false;
        isActing = false;
        areActionsBuilt = false;

    }

    /// <summary>
    /// Enables the character to start building actions
    /// </summary>
    public void SelectCharacter() {
        // do something visually here to indicate which character is active
        Debug.Log("Active character: " + gameObject.name);
        canBuildActions = true;
    }
}
