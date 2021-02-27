using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonCharacterAction : MonoBehaviour
{
    private SmoothMovement _mover;
    private ActionType _type;
    private Vector3 destPoint; // destination point for movement, throwing, etc
    public float throwForce = 500f;

    public PhotonCharacterAction(ActionType type, SmoothMovement mover, Vector3 destPoint)
    {
        _type = type;
        _mover = mover;
        this.destPoint = destPoint;
        // do we need additional params / do we need to case on type?

        isActing = false;
    }

    // invoke coroutine and have a bool for isActing
    /// <summary>
    /// Invoke the appropriate coroutine according to the action type.
    /// Set isActing so other classes can wait for this action to finish
    /// </summary>
    public void DoAction()
    {
        switch (_type)
        {
            case ActionType.Move:
                if (isActing)
                {
                    Debug.LogError("Called DoMove while already acting!");
                }
                else
                {
                    _mover.StartCoroutine(DoMove());
                }
                break;
            case ActionType.Throw:
                if (isActing)
                {
                    Debug.LogError("Called DoThrow while already acting!");
                }
                else
                {
                    _mover.StartCoroutine(DoThrow());
                }
                break;
        }
    }

    public bool isActing;
    /// <summary>
    /// Handle the "move" action.
    /// </summary>
    /// <returns></returns>
    IEnumerator DoMove()
    {
        isActing = true;
        _mover.MoveTo(destPoint);
        // TODO fix this, players ignoring middle waypoint for some reason
        // or don't bc it might resolve itself with other code implemented
        yield return new WaitUntil(() => !(_mover.isMoving));
        // yield return new WaitUntil(() => Vector3.Distance(_agent.transform.position, _agent.destination) <= _agent.stoppingDistance);
        // Debug.Log("Moving complete!");
        isActing = false;
    }

    IEnumerator DoThrow()
    {
        Debug.Log("Do Throw!");
        isActing = true;

        _mover.RotateTowards(destPoint);
        yield return new WaitUntil(() => !(_mover.isRotating));

        // Might want to just create/delete the obj in the player's hands as needed
        // and then just instantiate a new one when need to throw. Might help if there is wonkiness
        // That way we can just have a bool isHoldingBall instead of checking for child objects
        PhotonBallController ball = _mover.gameObject.GetComponentInChildren<PhotonBallController>();
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

}
