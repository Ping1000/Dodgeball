using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionController : MonoBehaviour
{
    public uint numActions;
    public int numActionsSet { get; protected set; }
    public bool areActionsBuilt { get; protected set; }
    protected bool canBuildActions;
    public string teamTag;
    public string ballTag;
    public ActionType selectedAction { get; protected set; }

    public LinkedList<CharacterAction> actionsList;
    protected SmoothMovement _mover;
    protected SkinnedMeshRenderer _skinnedRenderer;
    protected Animator _animController;
    public TeamController teamController;

    public bool isBuilding { get; protected set; }
    public bool isActing { get; protected set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public abstract void ExecuteActions();

    public abstract void SelectCharacter();

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
