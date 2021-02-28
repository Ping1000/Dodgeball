using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// TODO: add "break" feature to allow people to cancel their actions
// TODO: integrate with UI stuff to show people what they're planned action is
// BUG:  If there's an issue with the path, a destination might be skipped.
//       fixed for now by lowering the margins on the navmesh border, not sure
//       if it happens with overlapping paths so i'll have to test more later

[RequireComponent(typeof(SmoothMovement))]
public class ActionController : MonoBehaviour {
    public uint numActions; // Number of actions each player can take total
    [HideInInspector]
    public int numActionsSet { get; private set; } // Number of actions that have been set // public for debugging, should probably privatize
    [HideInInspector]
    public bool areActionsBuilt;

    public LayerMask clickableMask; //  Most likely this will be an | of multiple masks
    public string floorTag;
    public string teamTag;
    public string ballTag;
    public string moveButtonTag;
    public string throwButtonTag;
    public Material defaultMaterial;
    public Material selectedMaterial;
    [HideInInspector]
    public ActionType selectedAction;
    private LinkedList<CharacterAction> actionsList; 
    private bool canBuildActions;

    private SmoothMovement _mover;
    private MeshRenderer _renderer;
    private TextManager _txt;
    private LineController _lines;

    // public GameObject debugSpherePrefab;
    // private List<GameObject> debugSpheres;

    [SerializeField]
    private TeamController teamController;

    // Start is called before the first frame update
    void Start() // May need to change this to Awake()
    {
        actionsList = new LinkedList<CharacterAction>();
        _mover = GetComponent<SmoothMovement>();
        _renderer = GetComponent<MeshRenderer>();
        _lines = GetComponent<LineController>();
        _txt = FindObjectOfType<TextManager>();

        canBuildActions = false;
        isBuilding = false;
        isWaiting = false;
        isActing = false;
        areActionsBuilt = false;

        selectedAction = ActionType.Move; // TODO un-hardcode this
        numActionsSet = 0;

        // TESTING
        // debugSpheres = new List<GameObject>();
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

    void OnCollisionEnter(Collision collision)
    {
        
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

        for (numActionsSet = 0; numActionsSet < numActions; ) { // i++ removed on purpose, want to allow multiple movement waypoints to be considered one action
            if (waiting == null) {
                waiting = StartCoroutine(WaitingForInput(numActionsSet));
                yield return new WaitUntil(() => !isWaiting);
            } else {
                Debug.LogError("Tried to wait for input while already waiting!");
                break;
            }
        }

        _lines.ClearLines(); // move to after executing? after doing all teams? idk
        isBuilding = false;
        building = null;
        areActionsBuilt = true;
        _renderer.material = defaultMaterial;
        Debug.Log("Actions built.");
    }

    [HideInInspector]
    public bool isWaiting;
    [HideInInspector]
    public Coroutine waiting;
    /// <summary>
    /// Coroutine for waiting for input for the action
    /// </summary>
    /// <param name="currentAction">Index of the action in actions to set</param>
    /// <returns></returns>
    IEnumerator WaitingForInput(int currentAction) {
        isWaiting = true;

        if (selectedAction == ActionType.Move) {
            _lines.StartTrackingMove();
        } else if (selectedAction == ActionType.Throw) {
            _lines.StartTrackingThrow();
        }

        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Backspace));

        if (Input.GetKeyDown(KeyCode.Backspace)) {
            UndoMove();
        } else {

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) { // TODO append ", clickableMask" so it only checks for collisions on things we can actually click

                // Debugging
                Debug.DrawRay(ray.origin, ray.direction * 20f, Color.red, 2f);
                Debug.Log("Hit " + hit.collider.gameObject.name);
                //

                // TODO figure out how to select different actions
                // With current implementation, it may be easier to do a control panel first...

                // Switch may be unneeded with this:
                // actionsQueue.Enqueue(new CharacterAction(selectedAction, this, _agent, hit.point));
                //if (selectedAction == ActionType.Move)
                //{
                //    // TODO check distance, if above threshold, set waypoint in the direction of point up to that 
                //}
                //numActionsSet++;

                // Maybe able to switch actions using ActionType.Select? 
                // There's a better way to do this but currently rushed and tired
                if (hit.collider.CompareTag(moveButtonTag)) {
                    selectedAction = ActionType.Move;
                    // Debug.Log("Move Button Selected!");
                    // _txt.actionText.text = "Selected Action: Move";
                    SFXManager.PlayNewSound(soundType.button);
                } else if (hit.collider.CompareTag(throwButtonTag)) {
                    selectedAction = ActionType.Throw;
                    // Debug.Log("Throw Button Selected!");
                    //_txt.actionText.text = "Selected Action: Throw";
                    SFXManager.PlayNewSound(soundType.button);
                } else {
                    switch (selectedAction) {
                        case ActionType.Move:
                            if (hit.collider.CompareTag(floorTag)) {
                                actionsList.AddLast(new CharacterAction(selectedAction, _mover, hit.point));

                                SFXManager.PlayNewSound(soundType.action);
                                numActionsSet++;
                            }
                            break;
                        case ActionType.Throw:
                            if (hit.collider.gameObject.layer == 8) {
                                actionsList.AddLast(new CharacterAction(selectedAction, _mover, hit.point));
                                SFXManager.PlayNewSound(soundType.action);
                                numActionsSet++;
                            }
                        break;
                        case ActionType.Catch:
                            if (hit.collider.CompareTag(floorTag) || hit.collider.CompareTag(teamTag)) {
                                actionsList.AddLast(new CharacterAction(selectedAction, _mover, hit.point));
                                numActionsSet++;
                            }
                            break;
                        case ActionType.Pass:
                            if (hit.collider.CompareTag(teamTag)) {
                                actionsList.AddLast(new CharacterAction(selectedAction, _mover, hit.point));
                                numActionsSet++;
                            }
                            break;
                        default:
                            Debug.LogError("Selected Action " + selectedAction + " Not Found");
                            break;
                    }
                }
            } else {
                Debug.Log("Missed a valid raycast target");
            }
        }
        isWaiting = false;
        waiting = null;
    }

    void UndoMove() {
        if (actionsList.Count > 0) {
            actionsList.RemoveLast();
            numActionsSet--;
            _lines.ClearRecentLine();
        }
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

        while (actionsList.Count > 0)
        {
            CharacterAction action = actionsList.First.Value;
            action.DoAction();
            yield return new WaitUntil(() => !(action.isActing));
            actionsList.RemoveFirst();
        }

        // reset variables when done
        actionsList = new LinkedList<CharacterAction>(); // should be garbage collected, right? // God I hope so
        canBuildActions = false;
        isBuilding = false;
        isWaiting = false;
        isActing = false;
        areActionsBuilt = false;

        //foreach (GameObject obj in debugSpheres)
        //{
        //    Destroy(obj);
        //}
        teamController.finishedActing++;
    }

    /// <summary>
    /// Enables the character to start building actions
    /// </summary>
    public void SelectCharacter() {
        // do something visually here to indicate which character is active
        // Debug.Log("Active character: " + gameObject.name);
        _renderer.material = selectedMaterial;
        switch (selectedAction) {
            case ActionType.Move:
                // _txt.actionText.text = "Selected Action: Move";
                break;
            case ActionType.Throw:
                // _txt.actionText.text = "Selected Action: Throw";
                break;
            default:
                // _txt.actionText.text = "Seleted Action: N/A";
                break;
        }
        canBuildActions = true;
    }

    //TODO
    public void PlayerOut(Vector3 impactDir) {
        // game state blah blah blah stuff
        SFXManager.PlayNewSound(soundType.impact);
        _mover.GetKnockedOut(impactDir);
        teamController.members.Remove(this);
    }
}
