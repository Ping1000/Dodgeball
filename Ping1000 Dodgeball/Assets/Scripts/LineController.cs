using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the directional lines while planning out Actions
/// </summary>
public class LineController : MonoBehaviour
{
    private Stack<Vector3> savedStarts;
    private Stack<LineRenderer> lines;
    private PlayerActionController ac;
    private Vector3 startPoint;

    public float throwDistanceLength;
    private float moveDistanceLength;

    // Start is called before the first frame update
    void Start()
    {
        savedStarts = new Stack<Vector3>();
        lines = new Stack<LineRenderer>();
        ac = GetComponent<PlayerActionController>();
        moveDistanceLength = ac.maxDistance;
        startPoint = transform.position; // will need to reset when the planning ends
    }

    public void ClearRecentLine() {
        if (lines.Count > 0) {
            Destroy(lines.Pop().gameObject);
            startPoint = savedStarts.Pop();
        }
    }

    /// <summary>
    /// Deletes all stored lines
    /// </summary>
    public void ClearLines() {
        while (lines.Count > 0) {
            Destroy(lines.Pop().gameObject);
            savedStarts.Pop();
        }
        startPoint = transform.position;
    }

    /// <summary>
    /// Begin tracking position for moving.
    /// </summary>
    public void StartTrackingMove() {
        GameObject newLine = Instantiate((GameObject)Resources.Load("Prefabs/Move Line Renderer"),
            transform);

        StartCoroutine(TrackingMove(newLine.GetComponent<LineRenderer>()));
    }

    /// <summary>
    /// Coroutine handling getting mouseposition and drawing line from startPosition.
    /// Matches tag so only draws on movable ground
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    IEnumerator TrackingMove(LineRenderer line) {
        line.positionCount = 2;
        line.SetPosition(0, startPoint);
        line.SetPosition(1, startPoint);
        int oldActionsSet = ac.numActionsSet;

        Vector3 hitPoint = Vector3.zero;
        Vector3 dir = Vector3.zero;
        RaycastHit hit;
        Ray ray;
        while (ac.isWaiting) {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // old: hit.collider.gameObject.CompareTag(ac.floorTag)
            // old: && ((ac.clickableMask | (1 << hit.collider.gameObject.layer)) != 0)
            if (Physics.Raycast(ray, out hit, 1000) &&
                (hit.collider.CompareTag(ac.floorTag) ||
                hit.collider.CompareTag("Ball"))) {
                hitPoint = hit.point;
                hitPoint.y = transform.position.y;

                if (Vector3.Distance(startPoint, hitPoint) > moveDistanceLength)
                {
                    dir = new Vector3(hitPoint.x - startPoint.x,
                        0, hitPoint.z - startPoint.z);
                    dir.Normalize();
                    dir.Scale(new Vector3(moveDistanceLength, 0, moveDistanceLength));
                    hitPoint = startPoint + dir;
                    line.SetPosition(1, hitPoint);
                }
                else
                {
                    line.SetPosition(1, hitPoint);
                }

                
            }
            // line.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            yield return null;
        }
        if (ac.numActionsSet > oldActionsSet) {
            savedStarts.Push(startPoint);
            lines.Push(line);
            if (hitPoint != Vector3.zero)
                startPoint = hitPoint;
        } else {
            Destroy(line.gameObject);
        }
    }

    /// <summary>
    /// Begin tracking mouse position for throwing
    /// </summary>
    public void StartTrackingThrow() {
        GameObject newLine = Instantiate((GameObject)Resources.Load("Prefabs/Throw Line Renderer"),
            transform);

        StartCoroutine(TrackingThrow(newLine.GetComponent<LineRenderer>()));
    }

    /// <summary>
    /// Coroutine for handling mouse tracking for throwing. Draws line only up to a certain distance
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    IEnumerator TrackingThrow(LineRenderer line) {
        line.positionCount = 2;
        line.SetPosition(0, startPoint);
        line.SetPosition(1, startPoint);
        int oldActionsSet = ac.numActionsSet;

        RaycastHit hit;
        Ray ray;
        while (ac.isWaiting) {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000)) {
                Vector3 hitPoint = hit.point;
                hitPoint.y = transform.position.y;

                if (Vector3.Distance(startPoint, hitPoint) > throwDistanceLength) {
                    Vector3 dir = new Vector3(hitPoint.x - startPoint.x, 
                        0, hitPoint.z - startPoint.z);
                    dir.Normalize();
                    dir.Scale(new Vector3(throwDistanceLength, 0, throwDistanceLength));
                    line.SetPosition(1, startPoint + dir);
                } else {
                    line.SetPosition(1, hitPoint);
                }
            }
            // line.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            yield return null;
        }
        if (ac.numActionsSet > oldActionsSet) {
            savedStarts.Push(startPoint);
            lines.Push(line);
            // shouldn't update start point for throws
        } else {
            Destroy(line.gameObject);
        }
    }
}
