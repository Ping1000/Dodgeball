using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// TODO: add "break" feature to allow people to cancel their actions
// TODO: integrate with UI stuff to show people what they're planned action is
// BUG:  sometimes (only saw when testing action list of size 5), actions would
//       get skipped.

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
        SelectCharacter();
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
        } else if (Input.GetKeyDown(KeyCode.Space)) {
            // execute actions on space for now
            ExecuteActions();
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
            // WILL NEED TO CASE ON WHAT YOU CLICK ON, FOR NOW JUST MOVE
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

        // reset variables when done
        actions = new CharacterAction[numActions]; // should be garbage collected, right?
        canBuildActions = false;
        isBuilding = false;
        isWaiting = false;
        isActing = false;
        areActionsBuilt = false;
    }

    [HideInInspector]
    public bool isActing;
    /// <summary>
    /// Coroutine to handle executing the actions sequentially
    /// </summary>
    /// <returns></returns>
    IEnumerator ExecutingActions() {
        foreach (CharacterAction a in actions) {
            a.DoAction();
            yield return new WaitUntil(() => !(a.isActing));
        }

    }

    /// <summary>
    /// Enables the character to start building actions
    /// </summary>
    public void SelectCharacter() {
        canBuildActions = true;
    }
}
