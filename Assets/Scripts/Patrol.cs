using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Patrol : MonoBehaviour
{
    [System.Serializable]

    private struct Point
    {
        public GameObject point;
        public bool isRunning;
    }
    [Header("Speed")]
    [SerializeField] private float walkSpeed = 1.0f;
    [SerializeField] private float runSpeed = 2.0f;
    [SerializeField] private float acceleration = 0.5f;
    [SerializeField] private float speedOffset = 0.1f;

    [Header("Path")]
    [SerializeField] private float pointOffset = 0.2f;
    [SerializeField] private Point [] pathPoints;

    private UnityEngine.AI.NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private int _speedHash = Animator.StringToHash("Speed");

    private float _walkRunSpeedDiff;
    [SerializeField]
    private float _targetSpeed;
    [SerializeField]
    private float _currentSpeed;
    private int _currentTargetIndex = 0;
    private int CurrentTargetIndex{
        set
        {
            _currentTargetIndex = value;
            _currentTargetIndex %= pathPoints.Length;
            _targetSpeed = pathPoints[_currentTargetIndex].isRunning ? runSpeed : walkSpeed;
            _navMeshAgent.destination = pathPoints[_currentTargetIndex].point.transform.position;
        }
        get
        {
            return _currentTargetIndex;
        }
    }
    private void Awake()
    {
        _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _walkRunSpeedDiff = runSpeed - walkSpeed;
        _currentSpeed = walkSpeed;
        _navMeshAgent.speed = _currentSpeed;
        CurrentTargetIndex = 0;
    }

    private void FixedUpdate()
    {
        if (IsPointReached())
        {
            CurrentTargetIndex++;
        }
        UpdateSpeed();
    }

    private bool IsPointReached()
    {
        if (Vector3.Distance(transform.position, _navMeshAgent.destination) < pointOffset)
        {
            return true;
        }
        return false;
    }

    private void UpdateSpeed() 
    {
        var speedDiff = _targetSpeed - _currentSpeed;
        if (speedDiff == 0) return;
        if (speedDiff > speedOffset) 
        {
            _currentSpeed += acceleration * Time.fixedDeltaTime;
        }
        else if (speedDiff < -speedOffset)
        {
            _currentSpeed -= acceleration * Time.fixedDeltaTime;
        }
        else 
        {
            _currentSpeed = _targetSpeed;
        }
        _navMeshAgent.speed = _currentSpeed;
        UpdateAnimationSpeed();
    }

    private void UpdateAnimationSpeed()
    {
        _animator.SetFloat(_speedHash, (_currentSpeed - walkSpeed) / _walkRunSpeedDiff);
    }
}
