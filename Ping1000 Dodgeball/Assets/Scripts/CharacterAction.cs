using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enum type dictating the type of action for this object
/// </summary>
public enum ActionType {
    Move,
    Throw,
    Catch
}

public class CharacterAction
{
    private ActionType _type;
    private NavMeshAgent _agent; // the pathfinding agent
    private MonoBehaviour _mono; // need a MonoBehaviour to run async coroutines
    private Vector3 destPoint; // destination point for movement, throwing, etc

    public CharacterAction(ActionType type, MonoBehaviour mono, NavMeshAgent agent, Vector3 destPoint) {
        _type = type;
        _agent = agent;
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
        _agent.SetDestination(destPoint);

        yield return new WaitUntil(() =>
            Mathf.Approximately(_agent.remainingDistance, _agent.stoppingDistance));
        
        isActing = false;
    }

    IEnumerator DoThrow() {
        yield return null;
    }

    IEnumerator DoCatch() {
        yield return null;
    }
}
