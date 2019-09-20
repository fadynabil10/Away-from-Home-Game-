using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public GameObject platform;

    public GameObject [] waypoints;

    [Range(0.0f, 10.0f)]
    public float moveSpeed = 5f;
    public float waitAtWaypointTime = 1f;

    public bool moving = true;
    public bool loop = true;

    Transform _Transform;
    int _waypointIndex = 0;
    float _moveTime;
   

    // Start is called before the first frame update
    void Start()
    {
        _Transform = platform.transform;
        _moveTime = 0f;
        moving = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= _moveTime)
            Movement();
    }

    void Movement()
    {
        if(waypoints.Length !=0 && moving)
        {
            _Transform.position = Vector3.MoveTowards(_Transform.position, waypoints[_waypointIndex].transform.position, moveSpeed * Time.deltaTime);

            if(Vector3.Distance(waypoints[_waypointIndex].transform.position, transform.position) <= 0){
                _waypointIndex++;
                _moveTime = Time.time + waitAtWaypointTime;
            }

            if(_waypointIndex >= waypoints.Length)
            {
                if (loop)
                    _waypointIndex = 0;
                else
                    moving = false;
            }
        }
    }
}
