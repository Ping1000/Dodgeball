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
    }

    public void ThrowBall(GameObject player, Vector3 direction, float magnitude)
    {
        if (currentState != BallState.IsHeld)
        {
            Debug.LogError("Throwing a ball that is not held!");
        }
        currentState = BallState.WasThrown;
        thrownBy = player;

        // put in ball throwing code here
        _collider.enabled = true;
        _rb.constraints = RigidbodyConstraints.FreezePositionY; // TODO fix throw direction and make this none
        transform.SetParent(null);
        _rb.AddForce(direction * magnitude);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Ball hit " + collision.gameObject.name);
        if (collision.gameObject.CompareTag(floorTag)) // Hit the floor
        {
            currentState = BallState.OnGround;
            thrownBy = null;
        }
        // Maybe handle below in ActionController?
        else if (currentState == BallState.WasThrown && !collision.gameObject.CompareTag(thrownBy.tag))
        {
            if (!collision.gameObject.CompareTag("Dummy"))
            {
                collision.gameObject.GetComponent<ActionController>().PlayerOut(collision);
            }
            
        }
        else if (currentState == BallState.OnGround)
        {
            Debug.Log("Ball being picked up");
            
            currentState = BallState.IsHeld;
            _collider.enabled = false;
            _rb.constraints = RigidbodyConstraints.FreezeAll;
            transform.SetParent(collision.gameObject.GetComponent<Transform>());
            Vector3 handPosition = collision.gameObject.transform.Find("Hands").position;
            transform.SetPositionAndRotation(handPosition, Quaternion.identity);
            
            
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
