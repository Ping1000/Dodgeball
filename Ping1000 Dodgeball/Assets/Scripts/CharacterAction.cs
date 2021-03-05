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
    private Vector3 destPoint; // destination point for movement, throwing, etc

    public CharacterAction(ActionType type, SmoothMovement mover, Vector3 destPoint) {
        _type = type;
        _mover = mover;
        this.destPoint = destPoint;
        // do we need additional params / do we need to case on type?

        isActing = false;
    }

    public ActionType GetActionType()
    {
        return _type;
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
                    _mover.StartCoroutine(DoMove());
                }
                break;
            case ActionType.Throw:
                if (isActing) {
                    Debug.LogError("Called DoThrow while already acting!");
                } else {
                    SFXManager.PlayNewSound(soundType.ballThrow);
                    _mover.StartCoroutine(DoThrow());
                }
                break;
            case ActionType.Catch:
                if (isActing) {
                    Debug.LogError("Called DoCatch while already acting!");
                } else {
                    _mover.StartCoroutine(DoCatch());
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

        SFXManager.PlayNewSound(soundType.move);
        _mover.RotateTowards(destPoint);
        yield return new WaitUntil(() => !(_mover.isRotating));
        _mover.MoveTo(destPoint);
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

        BallController ball = _mover.gameObject.GetComponentInChildren<BallController>();
        if (ball != null)
        {
            Vector3 curpos = _mover.transform.position;
            Vector3 throwVec = new Vector3(destPoint.x - curpos.x, 0, destPoint.z - curpos.z);
            throwVec = throwVec.normalized;
            ball.ThrowBall(_mover.gameObject, throwVec);
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
