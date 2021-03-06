using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIActionController : MonoBehaviour
{
    public uint numActions;
    [HideInInspector]
    public int numActionsSet { get; private set; }
    [HideInInspector]
    public bool areActionsBuilt;

    public string teamTag;
    public string ballTag;
    public Material defaultMaterial;
    public Material seletedMaterial;
    public Transform frontLeft;
    public Transform frontRight;
    public Transform backLeft;
    public Transform backRight;
    [HideInInspector]
    public ActionType selectedAction;
    private LinkedList<CharacterAction> actionsList;
    private bool canBuildActions;

    public ActionChangeButton buttonChanger;

    private SmoothMovement _mover;
    private SkinnedMeshRenderer _skinnedRenderer;
    private TextManager _txt;

    private Animator _animController;

    [SerializeField]
    private TeamController teamController;


    private static List<BallController> balls;
    private static List<ActionController> otherPlayers;
    private bool willBeInFront; // not back, not on ball
    private bool willHoldBall;
    public float throwChance;
    // public float throwArc;

    // Start is called before the first frame update
    void Start()
    {
        actionsList = new LinkedList<CharacterAction>();
        _mover = GetComponent<SmoothMovement>();
        _skinnedRenderer = GetComponent<SkinnedMeshRenderer>();
        _animController = GetComponent<Animator>();

        _txt = FindObjectOfType<TextManager>();

        canBuildActions = false;
        isBuilding = false;
        isActing = false;
        areActionsBuilt = false;
        willHoldBall = false;
        willBeInFront = false;
        // throwArc = Mathf.Abs(throwArc);

        selectedAction = ActionType.Move;
        numActionsSet = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (canBuildActions && !isBuilding) {
            if (building == null) {
                building = StartCoroutine(BuildingActions());
            } else {
                Debug.LogError("AI tried to build while already building!");
            }
        }
    }

    [HideInInspector]
    public bool isBuilding;
    Coroutine building;
    IEnumerator BuildingActions() {
        isBuilding = true;

        if (balls == null)
            balls = UpdateBallList();
        if (otherPlayers == null)
            otherPlayers = UpdateOtherPlayerList();

        for (numActionsSet = 0; numActionsSet < numActions;) {
            if (numActionsSet == 0 && !willHoldBall && balls.Count > 0) {
                AddGetBallAction();
                numActionsSet++;
            } else {
                BuildAction();
            }
            yield return new WaitForEndOfFrame();
        }
        isBuilding = false;
    }

    public void AddGetBallAction() {
        BallController chosenBall = balls[Random.Range(0, balls.Count)];
        actionsList.AddLast(new CharacterAction(ActionType.Move, _mover, chosenBall.transform.position));
        balls.Remove(chosenBall);
        willBeInFront = false;
        willHoldBall = true;
        numActionsSet++;
    }

    void BuildAction() {
        float roll = Random.value;
        if (willHoldBall) {
            if (roll < throwChance) {
                AddThrowAction();
            } else {
                AddMoveAction();
            }
        } else {
            // might need to add a "getBallChance," for now, always greedy
            if (balls.Count > 0) {
                AddGetBallAction();
            } else {
                AddMoveAction();
            }
        }
    }

    void AddThrowAction() {
        ActionController targetPlayer = otherPlayers[Random.Range(0, otherPlayers.Count)];

        Vector3 throwDest = Vector3.zero;

        // might need to pick the second move point idk
        foreach (CharacterAction act in targetPlayer.actionsList) {
            if (act.GetActionType() == ActionType.Move) {
                throwDest = act.destPoint;
            }
        }
        if (throwDest == Vector3.zero)
            throwDest = targetPlayer.transform.position;
        // want to add variance but since it's in CharacterAction i'm not gonna bother yet
        actionsList.AddLast(new CharacterAction(ActionType.Throw, _mover, throwDest));
        numActionsSet++;
        willHoldBall = false;
    }

    void AddMoveAction() {
        float newX, newZ;
        if (willBeInFront) {
            // move to back
            newX = Random.Range(backLeft.position.x, backRight.position.x);
            newZ = Random.Range(backLeft.position.z, backRight.position.z);
        } else {
            // move to front
            newX = Random.Range(frontLeft.position.x, frontRight.position.x);
            newZ = Random.Range(frontRight.position.z, frontRight.position.z);
        }
        Vector3 destPoint = new Vector3(newX, transform.position.y, newZ);
        actionsList.AddLast(new CharacterAction(ActionType.Move, _mover, destPoint));

        numActionsSet++;
        willBeInFront = !willBeInFront;
    }

    List<BallController> UpdateBallList() {
        List<BallController> droppedBalls = new List<BallController>();

        foreach (BallController ball in FindObjectsOfType<BallController>()) {
            if (ball.currentState == BallController.BallState.OnGround)
                droppedBalls.Add(ball);
        }

        return droppedBalls;
    }

    List<ActionController> UpdateOtherPlayerList() {
        List<ActionController> otherPlayers = new List<ActionController>();

        foreach (ActionController ac in FindObjectsOfType<ActionController>()) {
            if (!ac.CompareTag(tag))
                otherPlayers.Add(ac);
        }

        return otherPlayers;
    }

    public void ExecuteActions() {
        balls = null;
        otherPlayers = null;
        if (areActionsBuilt) {
            StartCoroutine(ExecutingActions());
        }
    }

    [HideInInspector]
    public bool isActing;

    IEnumerator ExecutingActions() {
        isActing = true;

        isActing = false;
    }

    public void SelectCharacter() {
        selectedAction = ActionType.Move;

        canBuildActions = true;
    }

    public void PlayerOut(Vector3 impactDir) {
        // game state blah blah blah stuff
        SFXManager.PlayNewSound(soundType.impact);
        _mover.GetKnockedOut(impactDir);
        BallController heldBall = GetComponentInChildren<BallController>();
        if (heldBall) {
            teamController.phaseController.balls.Remove(heldBall);
            Destroy(heldBall.gameObject);
        }
        teamController.members.Remove(this);
    }
}
