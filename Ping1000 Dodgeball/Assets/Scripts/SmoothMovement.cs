using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class SmoothMovement : MonoBehaviour
{
    private Rigidbody _body;
    private Collider _col;

    // public float stoppingDistance;
    public float moveSpeed;
    public float rotateSpeed;

    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();

        _body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        isMoving = false;
        isRotating = false;
    }

    public void MoveTo(Vector3 dest) {
        StartCoroutine(Moving(dest));
    }

    [HideInInspector]
    public bool isMoving;
    IEnumerator Moving(Vector3 dest) {
        isMoving = true;
        float startTime = Time.time;
        Vector3 startPos = transform.position;
        dest.y = startPos.y;
        float distCovered;
        float journeyLength = Vector3.Distance(startPos, dest);
        float fractionOfJourney = 0;

        while (fractionOfJourney < 0.95f) {
            distCovered = (Time.time - startTime) * moveSpeed;

            fractionOfJourney = distCovered / journeyLength;

            transform.position = Vector3.Lerp(startPos, dest, fractionOfJourney);
            yield return new WaitForEndOfFrame();
        }
        isMoving = false;
    }

    public void RotateTowards(Vector3 destPoint) {
        StartCoroutine(Rotating(destPoint));
    }

    [HideInInspector]
    public bool isRotating;
    IEnumerator Rotating(Vector3 destPoint) {
        isRotating = true;

        destPoint.y = transform.position.y;

        Vector3 direction = (destPoint - transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        while (!QuaternionApprox(transform.rotation, lookRotation, 0.005f)) {
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation,
                Time.deltaTime * rotateSpeed);
            yield return new WaitForEndOfFrame();
        }

        isRotating = false;
    }

    bool QuaternionApprox(Quaternion qA, Quaternion qB, float acceptableRange) {
        return 1 - Mathf.Abs(Quaternion.Dot(qA, qB)) < acceptableRange;
    }

    // Probably need to make this an RPC
    public void GetKnockedOut(Vector3 impactDir) {
        _body.isKinematic = false;
        _body.constraints = RigidbodyConstraints.None;

        // add some ragdoll stuff with like randomness in the direction? and scaling?
        _body.AddForce(Vector3.Scale(impactDir, new Vector3(500, 500, 500)));
        Destroy(this.gameObject, 5); // FIX THIS TO WAIT UNTIL AFTER ACTING IS FINISHED
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
