using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class PhotonBallController : MonoBehaviour
{
    public BallState currentState;
    public GameObject thrownBy;
    public string floorTag;

    [HideInInspector]
    public Rigidbody _rb;
    [HideInInspector]
    public Collider _collider;

    public float throwSpeed;
    private bool hasBeenThrown;
    private Vector3 thrownDirection;

    private List<PhotonBallController> ballList;

    private PhotonView photonView;

    public enum BallState
    {
        OnGround,
        WasThrown,
        IsHeld
    }
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        currentState = BallState.OnGround;
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        hasBeenThrown = false;
        ballList = FindObjectOfType<PhotonPhaseController>().balls;
        // ballList.Add(this);
    }

    private void Update()
    {
        if (hasBeenThrown)
        {
            Move();
        }
    }

    private void Move()
    {
        if (thrownDirection == null)
            return;

        transform.position += thrownDirection * throwSpeed * Time.deltaTime;
    }

    // might need to make this an RPC, but the ball is already instatiated on the server so any transforms should be tracked
    public void ThrowBall(GameObject player, Vector3 direction)
    {
        if (currentState != BallState.IsHeld)
        {
            Debug.LogError("Throwing a ball that is not held!");
        }
        currentState = BallState.WasThrown;
        thrownBy = player;

        // put in ball throwing code here
        transform.SetParent(null);
        hasBeenThrown = true;
        thrownDirection = direction.normalized;
        _collider.enabled = true;

        ballList.Remove(this);
        Destroy(this.gameObject, 7);
    }

    void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Ball hit " + other.gameObject.name);
        if (other.gameObject.CompareTag(floorTag)) // Hit the floor
        {
            currentState = BallState.OnGround;
            thrownBy = null;
        }
        // Maybe handle below in ActionController?
        //else if (currentState == BallState.WasThrown && !other.gameObject.CompareTag(thrownBy.tag))
        //{
        //    if (!collision.gameObject.CompareTag("Dummy"))
        //    {
        //        collision.gameObject.GetComponent<ActionController>().PlayerOut(collision);
        //    }

        //}
        else if (currentState == BallState.OnGround)
        {
            if (other.gameObject.GetComponentInChildren<PhotonBallController>())
                return;
            Debug.Log("Ball being picked up");

            currentState = BallState.IsHeld;
            _collider.enabled = false;
            transform.SetParent(other.gameObject.GetComponent<Transform>());
            Vector3 handPosition = other.gameObject.transform.Find("Hands").position;
            transform.SetPositionAndRotation(handPosition, Quaternion.identity);
        }
        else if (currentState == BallState.WasThrown && !other.gameObject.CompareTag(thrownBy.tag))
        {
            PhotonPlayerController other_ac = other.gameObject.GetComponent<PhotonPlayerController>();
            if (other_ac)
            {
                other_ac.PlayerOut(thrownDirection);
            }
            ballList.Remove(this);
            Destroy(this.gameObject);
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {

        }
        else
        {

        }
    }
}
