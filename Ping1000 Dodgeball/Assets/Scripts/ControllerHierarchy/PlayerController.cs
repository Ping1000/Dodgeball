using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    
    NavMeshAgent _agent;
    Rigidbody _rb;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
    }
    public bool Move(Vector3 waypoint)
    {
        if (_rb.position == waypoint)
        {
            return false;
        }
        _agent.SetDestination(waypoint);

        return true;
        
    }
}
