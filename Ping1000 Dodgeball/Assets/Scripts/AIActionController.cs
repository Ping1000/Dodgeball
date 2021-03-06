using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SmoothMovement))]
public class AIActionController : ActionController
{
    public Transform frontLeft;
    public Transform frontRight;
    public Transform backLeft;
    public Transform backRight;

    private static List<BallController> balls;
    private static List<ActionController> otherPlayers;
    private bool willBeInFront; // not back, not on ball
    private bool willHoldBall;
    [Range(0,1)]
    public float throwChance;
    [Range(0,10)]
    public float inaccuracySize;
    // public float throwArc;

    // Start is called before the first frame update
    void Start()
    {
        actionsList = new LinkedList<CharacterAction>();
        _mover = GetComponent<SmoothMovement>();
        _skinnedRenderer = GetComponent<SkinnedMeshRenderer>();
        _animController = GetComponent<Animator>();

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

    Coroutine building;
    IEnumerator BuildingActions() {
        isBuilding = true;
        canBuildActions = false;

        if (balls == null)
            balls = UpdateBallList();
        if (otherPlayers == null)
            otherPlayers = UpdateOtherPlayerList();

        for (numActionsSet = 0; numActionsSet < numActions;) {
            if (numActionsSet == 0 && !willHoldBall && balls.Count > 0) {
                AddGetBallAction();
            } else {
                BuildAction();
            }
            yield return new WaitForEndOfFrame();
        }

        building = null;
        areActionsBuilt = true;
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
        // naive variance
        float offsetX = Random.Range(-1 * inaccuracySize, inaccuracySize);
        float offsetZ = Random.Range(-1 * inaccuracySize, inaccuracySize);
        throwDest.x += offsetX;
        throwDest.z += offsetZ;
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

    public override void ExecuteActions() {
        balls = null;
        otherPlayers = null;
        if (areActionsBuilt) {
            StartCoroutine(ExecutingActions());
        }
    }

    IEnumerator ExecutingActions() {
        isActing = true;

        while (actionsList.Count > 0) {
            CharacterAction action = actionsList.First.Value;
            if (action.GetActionType() == ActionType.Move) {
                _animController.SetBool("isRunning", true);
            } else if (action.GetActionType() == ActionType.Throw) {
                _animController.SetBool("isThrowing", true);
            }
            action.DoAction();
            yield return new WaitUntil(() => !(action.isActing));
            if (action.GetActionType() == ActionType.Move) {
                _animController.SetBool("isRunning", false);
            } else if (action.GetActionType() == ActionType.Throw) {
                _animController.SetBool("isThrowing", false);
            }
            actionsList.RemoveFirst();
        }

        actionsList = new LinkedList<CharacterAction>();
        canBuildActions = false;
        isBuilding = false;
        isActing = false;
        areActionsBuilt = false;

        teamController.finishedActing++;
    }

    public override void SelectCharacter() {
        canBuildActions = true;
    }
}
