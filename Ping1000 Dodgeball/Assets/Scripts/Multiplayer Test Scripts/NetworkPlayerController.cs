using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; // this necessary?
using Mirror;

[RequireComponent(typeof(SmoothMovement))]
[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransform))]
public class NetworkPlayerController : NetworkBehaviour
{
    public uint numActions; // Number of actions each player can take total
    [HideInInspector]
    public int numActionsSet;// Number of actions that have been set // public for debugging, should probably privatize
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
    private ActionType selectedAction;
    private Queue<NetworkCharacterAction> actionsQueue;
    private bool canBuildActions;

    private SmoothMovement _mover;
    private MeshRenderer _renderer;
    private TextManager _txt;

    public GameObject debugSpherePrefab;
    private List<GameObject> debugSpheres;

    [SerializeField]
    private NetworkTeamController teamController;

    // Start is called before the first frame update
    void Start() // May need to change this to Awake()
    {
        actionsQueue = new Queue<NetworkCharacterAction>();
        _mover = GetComponent<SmoothMovement>();
        _renderer = GetComponent<MeshRenderer>();
        _txt = FindObjectOfType<TextManager>();

        canBuildActions = false;
        isBuilding = false;
        isWaiting = false;
        isActing = false;
        areActionsBuilt = false;

        selectedAction = ActionType.Move; // TODO un-hardcode this
        numActionsSet = 0;

        // TESTING
        debugSpheres = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.isLocalPlayer) { return; }

        if (canBuildActions && !isBuilding)
        {
            if (building == null)
                building = StartCoroutine(BuildingActions());
            else
                Debug.LogError("Tried to build while already building!");
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // break out of building actions by calling stopcorouting on building
            // and deleting existing actions (maybe just add a new function to
            // reset the vars and also add to executing actions
            //this.StopAllCoroutines();
            //ResetVars();
        }
    }

    private bool isBuilding;
    Coroutine building;
    /// <summary>
    /// Coroutine for building this character's actions
    /// </summary>
    /// <returns></returns>
    IEnumerator BuildingActions()
    {
        isBuilding = true;
        canBuildActions = false;

        for (numActionsSet = 0; numActionsSet < numActions;)
        { // i++ removed on purpose, want to allow multiple movement waypoints to be considered one action
            if (waiting == null)
            {
                waiting = StartCoroutine(WaitingForInput());
                yield return new WaitUntil(() => !isWaiting);
            }
            else
                Debug.LogError("Tried to wait for input while already waiting!");
        }

        isBuilding = false;
        building = null;
        areActionsBuilt = true;
        _renderer.material = defaultMaterial;
        Debug.Log("Actions built.");
    }

    [HideInInspector]
    public bool isWaiting;
    Coroutine waiting;
    /// <summary>
    /// Coroutine for waiting for input for the action
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitingForInput()
    {
        isWaiting = true;
        Debug.Log("Waiting on " + this.name);
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0));

        RaycastHit hit;
        //NavMeshHit meshHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 20f, Color.blue, 2f);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        { // TODO append ", clickableMask" so it only checks for collisions on things we can actually click

            // Debugging
            Debug.DrawRay(ray.origin, ray.direction * 20f, Color.red, 2f);
            Debug.Log("Hit " + hit.collider.gameObject.name);
            //

            if (hit.collider.CompareTag(moveButtonTag)) 
            {
                selectedAction = ActionType.Move;
                // Debug.Log("Move Button Selected!");
                _txt.actionText.text = "Selected Action: Move";
            }
            else if (hit.collider.CompareTag(throwButtonTag))
            {
                selectedAction = ActionType.Throw;
                // Debug.Log("Throw Button Selected!");
                _txt.actionText.text = "Selected Action: Throw";
            }
            /*else if (hit.collider.CompareTag(teamTag)) // TODO create Deselect() and get this working
            {
                hit.collider.gameObject.GetComponent<NetworkPlayerController>().SelectCharacter();
                Deselect();
            }*/
            else
            {
                switch (selectedAction)
                {
                    case ActionType.Move:
                        if (hit.collider.CompareTag(floorTag))
                        {
                            debugSpheres.Add(Instantiate(debugSpherePrefab, hit.point, Quaternion.identity));
                            // TODO change this to avoid redundant info
                            actionsQueue.Enqueue(new NetworkCharacterAction(selectedAction, _mover, hit.point));

                            // TODO check distance, if above threshold, set waypoint in the direction of point up to that distance
                            // then increment num
                            numActionsSet++;
                        }
                        break;
                    case ActionType.Throw:
                        // if (hit.collider.CompareTag(floorTag) || hit.collider.CompareTag(teamTag)) // doesn't matter what we click on basically
                        { //don't care if an enemy is clicked bc clickable mask should've taken care of it
                            debugSpheres.Add(Instantiate(debugSpherePrefab, hit.point, Quaternion.identity));
                            actionsQueue.Enqueue(new NetworkCharacterAction(selectedAction, _mover, hit.point));
                            numActionsSet++;
                            Debug.Log("Throw!");
                        }
                        break;
                    default:
                        Debug.LogError("Selected Action " + selectedAction + " Not Found");
                        break;
                }
            }

        }
        else
        {
            Debug.Log("Missed a valid raycast target");
        }

        isWaiting = false;
        waiting = null;
    }

    /// <summary>
    /// Execute the built up actions and reset variables when done.
    /// </summary>
    public void ExecuteActions()
    {
        if (areActionsBuilt)
            StartCoroutine(ExecutingActions());
    }

    [HideInInspector]
    public bool isActing;
    /// <summary>
    /// Coroutine to handle executing the actions sequentially
    /// </summary>
    /// <returns></returns>
    IEnumerator ExecutingActions()
    {
        isActing = true;

        while (actionsQueue.Count > 0)
        {
            NetworkCharacterAction action = actionsQueue.Dequeue();
            action.DoAction();
            yield return new WaitUntil(() => !(action.isActing));
        }

        // reset variables when done
        actionsQueue = new Queue<NetworkCharacterAction>(); // should be garbage collected, right? // God I hope so
        canBuildActions = false;
        isBuilding = false;
        isWaiting = false;
        isActing = false;
        areActionsBuilt = false;

        foreach (GameObject obj in debugSpheres)
        {
            Destroy(obj);
        }
        teamController.finishedActing++;
    }

    /// <summary>
    /// Enables the character to start building actions
    /// </summary>
    public void SelectCharacter()
    {
        // do something visually here to indicate which character is active
        // Debug.Log("Active character: " + gameObject.name);
        _renderer.material = selectedMaterial;
        switch (selectedAction)
        {
            case ActionType.Move:
                //_txt.actionText.text = "Selected Action: Move";
                break;
            case ActionType.Throw:
                //_txt.actionText.text = "Selected Action: Throw";
                break;
            default:
                //_txt.actionText.text = "Seleted Action: N/A";
                break;
        }
        canBuildActions = true;
    }

    //TODO
    public void PlayerOut(Vector3 impactDir)
    {
        // game state blah blah blah stuff
        _mover.GetKnockedOut(impactDir);
        //teamController.members.Remove(this); //TODO readd
    }
}
