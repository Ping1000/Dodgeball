using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public BallState currentState; 
    public GameObject thrownBy;
    public string floorTag;

    public Rigidbody _rb;
    public Collider _collider;

    private PhaseController _pc;
    private float lifeSpan;

    public float throwSpeed;
    private bool hasBeenThrown;
    private Vector3 thrownDirection;

    private List<BallController> ballList;

    public enum BallState
    {
        OnGround,
        WasThrown,
        IsHeld
    }
    // Start is called before the first frame update
    void Awake()
    {
        currentState = BallState.OnGround;
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        hasBeenThrown = false;
        ballList = FindObjectOfType<PhaseController>().balls;
        _pc = FindObjectOfType<PhaseController>();
        // ballList.Add(this);
        lifeSpan = 3;
    }

    private void Update() {
        if (hasBeenThrown) {
            Move();
        }
    }

    private void Move() {
        if (thrownDirection == null)
            return;

        transform.position += thrownDirection * throwSpeed * Time.deltaTime;
    }

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
        _pc.ballTimer = Mathf.Clamp(_pc.ballTimer + lifeSpan, 0, lifeSpan);
        Destroy(this.gameObject, lifeSpan);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Ball hit " + other.gameObject.name);
        /*if (other.gameObject.CompareTag(floorTag)) // Hit the floor
        {
            currentState = BallState.OnGround;
            thrownBy = null;
        }*/
        // Maybe handle below in ActionController?
        //else if (currentState == BallState.WasThrown && !other.gameObject.CompareTag(thrownBy.tag))
        //{
        //    if (!collision.gameObject.CompareTag("Dummy"))
        //    {
        //        collision.gameObject.GetComponent<ActionController>().PlayerOut(collision);
        //    }
            
        //}
        if (currentState == BallState.OnGround && (other.gameObject.CompareTag("Red Player") || other.gameObject.CompareTag("Blue Player")))
        {
            if (other.gameObject.GetComponentInChildren<BallController>())
                return;
            //if (other.gameObject.CompareTag("Red Team") || other.gameObject.CompareTag("Blue Team")) { return; }

            Debug.Log(this.name + " being picked up by " + gameObject.name);
            
            currentState = BallState.IsHeld;
            _collider.enabled = false;
            transform.SetParent(other.gameObject.GetComponent<Transform>());
            Vector3 handPosition = other.gameObject.transform.Find("Hands").position;
            transform.SetPositionAndRotation(handPosition, Quaternion.identity);
        }
        else if (currentState == BallState.WasThrown && !other.gameObject.CompareTag(thrownBy.tag)) {
            ActionController other_ac = other.gameObject.GetComponent<ActionController>();
            if (other_ac) {
                other_ac.PlayerOut(thrownDirection);
            }
            ballList.Remove(this);
            Destroy(this.gameObject);
        }
    }

 /*   void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Ball hit something");
        if (collision.gameObject.CompareTag(floorTag)) // Hit the floor
        {
            currentState = BallState.OnGround;
            thrownBy = null;
        }
        // Maybe handle below in ActionController?
        else if (currentState == BallState.WasThrown && !collision.gameObject.CompareTag(thrownBy.tag))
        {
            collision.gameObject.GetComponent<ActionController>().PlayerOut(collision);
        }
        else if (currentState == BallState.OnGround)
        {
            Debug.Log("Ball being picked up");
            _rb.MovePosition(collision.gameObject.GetComponentInChildren<Transform>().position);
            currentState = BallState.IsHeld;
            _collider.
        }
    }*/



}
