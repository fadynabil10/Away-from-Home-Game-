using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Range(0f, 10f)]
    public float moveSpeed = 10f;

    public GameObject[] wayPoints;

    public float waitAtWaypointTime = 0f;

    public bool loopWayPoints = true;

    Transform _transform;
    Rigidbody2D _rigidbody;
    Animator _animator;
    AudioSource _audio;

    int _waypointIndex = 0;
    float _moveTime = 0f;
    float _vx = 0f;
    bool _moving = true;

    private void Awake()
    {
        _transform = GetComponent<Transform>();

        _rigidbody = GetComponent<Rigidbody2D>();
        if (_rigidbody == null)
            _rigidbody = gameObject.AddComponent<Rigidbody2D>();

        _animator = GetComponent<Animator>();
        if (_animator == null)
            _animator = gameObject.AddComponent<Animator>();

        _moveTime = 0f;
        _moving = true;
    }

    private void Update()
    {
        if (Time.time >= _moveTime)
            EnemyMovement();

		transform.localScale = new Vector3(Mathf.Sign(_rigidbody.velocity.x), 1, 1);
    }

    void EnemyMovement()
    {
        Flip(_vx);

        if (wayPoints.Length != 0 && _moving)
        {
            _transform.position = Vector3.MoveTowards(_transform.position, wayPoints[_waypointIndex].transform.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(wayPoints[_waypointIndex].transform.position, transform.position) <= 0)
            {
                _waypointIndex++;
                _moveTime = Time.time + waitAtWaypointTime;
            }

            if (_waypointIndex >= wayPoints.Length)
            {
                if (loopWayPoints)
                    _waypointIndex = 0;
                else
                    _moving = false;
            }
        }

    }
    void Flip(float _vx)
    {
        Vector3 localScale = _transform.localScale;

        if (_vx > 0f && localScale.x < 0f)
            localScale.x *= -1;
        else if (_vx < 0f && localScale.x > 0f)
            localScale.x *= -1;

        _transform.localScale = localScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player") {
            other.gameObject.GetComponent<Charachter>().Respawn();
        }
    }
}
