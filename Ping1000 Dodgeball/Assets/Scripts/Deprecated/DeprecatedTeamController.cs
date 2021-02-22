using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//TODO look into cleanup, does C# have GC?
public class DTeamController : MonoBehaviour
{
    public GameObject[] players;
    public NavMeshAgent[] playerAgents;
    public bool movingPhase; // remove this probably, kinda unneccessary 
    public bool planningPhase;
    public LayerMask playerMask;
    public LayerMask courtMask;
    public string playerTag;
    public string courtTag;
    public GameObject debugSpherePrefab;
    LayerMask clickableMask;


    
    private GameObject debugSphere;
    private Dictionary<GameObject, Queue<Vector3>> playerWaypoints; // Need a better way of doing this
    //private bool playerSelected = false;
    public GameObject selectedPlayer;
    
 
    
    void Start()
    {
        movingPhase = false;
        planningPhase = true;
        clickableMask = playerMask | courtMask;
    }

    void Update()
    {
        if (planningPhase)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (debugSphere != null)
                {
                    Destroy(debugSphere);
                }
    ;
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, 100f, clickableMask))
                {

                    // Debugging raycast
                    Debug.DrawRay(ray.origin, ray.direction * 20f, Color.red, 2f);
                    Debug.Log("Hit " + hit.collider.gameObject.name);
                    debugSphere = Instantiate(debugSpherePrefab, hit.point, Quaternion.identity);

                    // Check if a player is selected
                    if (hit.collider.CompareTag(playerTag))
                    {
                        selectedPlayer = hit.collider.gameObject;
                        Debug.Log("Player selected: " + selectedPlayer);
                        // selectedPlayer.ActiveIndicator();
                    }


                    else if (selectedPlayer != null && hit.collider.CompareTag(courtTag)) // player is already selected
                    {
                        // If navmesh is hit, append that waypoint to that player's list of waypoints
                        Debug.Log("Waypoint selected: " + hit.point);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                planningPhase = false;
            }
        }
        else
        {
            if (!PerformActions()) 
            {
                planningPhase = true;
            }
        }
        
    }

    bool PerformActions()
    {
        // Move players to waypoints
        // Throw ball in direction
        // Catch in direction
        if (true) // waypoint dictionary is not empty
        {
            // hacky code to get it working
            foreach (GameObject player in players)
            {
                
            }
        }
        
        // If actions in progress, return true
        // once all actions are complete, return false
        return false;
    }


}
