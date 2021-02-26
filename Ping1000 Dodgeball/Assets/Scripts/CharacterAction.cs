using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enum type dictating the type of action for this object
/// </summary>
public enum ActionType {
    Move,
    Throw,
    Catch,
    Pass
}

public class CharacterAction
{
    private SmoothMovement _mover;
    private ActionType _type;
    private MonoBehaviour _mono; // need a MonoBehaviour to run async coroutines
    private Vector3 destPoint; // destination point for movement, throwing, etc
    public float throwForce = 500f;

    public CharacterAction(ActionType type, MonoBehaviour mono, SmoothMovement mover, Vector3 destPoint) {
        _type = type;
        _mover = mover;
        _mono = mono;
        this.destPoint = destPoint;
        // do we need additional params / do we need to case on type?

        isActing = false;
    }

    // invoke coroutine and have a bool for isActing
    /// <summary>
    /// Invoke the appropriate coroutine according to the action type.
    /// Set isActing so other classes can wait for this action to finish
    /// </summary>
    public void DoAction() {
        switch (_type) {
            case ActionType.Move:
                if (isActing) {
                    Debug.LogError("Called DoMove while already acting!");
                } else {
                    _mono.StartCoroutine(DoMove());
                }
                break;
            case ActionType.Throw:
                if (isActing) {
                    Debug.LogError("Called DoThrow while already acting!");
                } else {
                    _mono.StartCoroutine(DoThrow());
                }
                break;
            case ActionType.Catch:
                if (isActing) {
                    Debug.LogError("Called DoCatch while already acting!");
                } else {
                    _mono.StartCoroutine(DoCatch());
                }
                break;
        }
    }

    public bool isActing;
    /// <summary>
    /// Handle the "move" action.
    /// </summary>
    /// <returns></returns>
    IEnumerator DoMove() {
        isActing = true;
        _mover.MoveTo(destPoint);
        // TODO fix this, players ignoring middle waypoint for some reason
        // or don't bc it might resolve itself with other code implemented
        yield return new WaitUntil(() => !(_mover.isMoving));
        // yield return new WaitUntil(() => Vector3.Distance(_agent.transform.position, _agent.destination) <= _agent.stoppingDistance);
        // Debug.Log("Moving complete!");
        isActing = false;
    }

    IEnumerator DoThrow() {
        Debug.Log("Do Throw!");
        isActing = true;

        _mover.RotateTowards(destPoint);
        yield return new WaitUntil(() => !(_mover.isRotating));

        BallController ball = _mono.gameObject.GetComponentInChildren<BallController>();
        if (ball != null)
        {
            Vector3 curpos = _mono.transform.position;
            Vector3 throwPoint = new Vector3(destPoint.x - curpos.x, 0, destPoint.z - curpos.z);
            Vector3 throwVec = throwPoint.normalized;
            throwVec.y = .5f;
            // ball.ThrowBall(_mono.gameObject, destPoint.normalized, throwForce);
            ball.ThrowBall(_mono.gameObject, throwVec, throwForce);
        }
        else
        {
            Debug.Log("Wasted Throw");
        }
        isActing = false;
        yield return null;
    }

    IEnumerator DoCatch() {
        yield return null;
    }
}
