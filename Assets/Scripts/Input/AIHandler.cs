using System;
using UnityEngine;
using UnityEngine.Splines;
using Utilities;

[RequireComponent(typeof(Rigidbody2D))]
public class AIHandler : MonoBehaviour, IInputHandler
{
    public float Throttle { get; set; }
    public float Steering { get; set; }
    public bool HandBrake { get; set; }

    [field: SerializeReference] public AIState CurrentAIState { get; private set; }
    [field: SerializeReference] public Rigidbody2D RB { get; private set; }
    [field: SerializeReference] public SplineContainer WaypointSpline { get; private set; }
    private float _lookAhead;
    private float _splinePos;
    private float _curvature;
    private Vector3 _targetPosition;

    [Header("Driving")]
    [field: SerializeField] public float MaxSteeringAngle { get; private set; }
    [field: SerializeField] public float SteeringSmooth { get; private set; }

    [Header("Speed Control")]
    [field: SerializeField] public float MaxThrottle { get; private set; }
    [field: SerializeField] public float MinThrottle { get; private set; }
    [field: SerializeField] public float MinBrakeSpeedFactor { get; private set; }
    [field: SerializeField] public float BrakeAngle { get; private set; }
    [field: SerializeField] public float BrakeTime { get; private set; }
    [field: SerializeField] public float StuckDetectionTime { get; private set; }
    [field: SerializeField] public LayerMask WallLayer { get; private set; }

    private bool _isActive = false;
    private bool _isStuck = false;

    private float _timeSinceHandbrakeStarted = 0f;
    private float _timeSinceStuck = 0f;
    private float _timeSinceUnstuck = 0f;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_isActive || WaypointSpline == null)
        {
            Throttle = 0;
            Steering = 0;
            HandBrake = false;
            return;
        }

        UpdateSplinePosition();
        DetectState();
        HandleInput();
    }

    private void DetectState()
    {
        switch(CurrentAIState)
        {
            case AIState.Driving:
                DetectStuck();
                break;
            case AIState.Reversing:
                DetectClear();
                break;
        }
    }

    private void DetectClear()
    {
        RaycastHit2D forwardHit = Physics2D.Raycast(transform.position, transform.up, Constants.AI_MAX_CLEAR_DETECTION_DISTANCE, WallLayer);
        _isStuck = (forwardHit.collider != null);
        Debug.DrawRay(transform.position, transform.up * Constants.AI_MAX_CLEAR_DETECTION_DISTANCE, Color.yellow);
        if (!_isStuck) {
            _timeSinceUnstuck += Time.deltaTime;
            if (_timeSinceUnstuck >= Constants.AI_UNSTUCK_DETECTION_TIME)
            {
                CurrentAIState = AIState.Driving;
                return;
            }
        } else
        {
            _timeSinceUnstuck = 0f;
        }
    }

    private void DetectStuck()
    {
        if (Throttle < MinThrottle) return;

        float currentForwardSpeed = Vector2.Dot(RB.linearVelocity, transform.up);
        RaycastHit2D forwardHit = Physics2D.Raycast(transform.position, transform.up, Constants.AI_MAX_STUCK_DETECTION_DISTANCE, WallLayer);

        _isStuck = (currentForwardSpeed <= Constants.AI_MAX_STUCK_DETECTION_SPEED && forwardHit.collider != null);
        Debug.DrawRay(transform.position, transform.up * Constants.AI_MAX_STUCK_DETECTION_DISTANCE, _isStuck ? Color.red : Color.green);
        if (_isStuck)
        {
            _timeSinceStuck += Time.deltaTime;
            if (_timeSinceStuck >= StuckDetectionTime)
            {
                CurrentAIState = AIState.Reversing;
                return;
            }
        } else if (_timeSinceStuck > 0f)
        {
            _timeSinceStuck = 0;
        }
    } 

    private void OnDrawGizmos()
    {
        if (_targetPosition != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_targetPosition, 2f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _targetPosition);
        }
    }

    private void HandleInput()
    {
        float speedFactor = Mathf.InverseLerp(0f, MaxThrottle, Mathf.Abs(Throttle));

        //float speedFactor = Mathf.InverseLerp(0f, maxSpeed, speed);
        _lookAhead = Mathf.Lerp(
            Constants.AI_SPLINE_MIN_LOOK_AHEAD,
            Constants.AI_SPLINE_MAX_LOOK_AHEAD,
            speedFactor
        );

        float targetT = Mathf.Clamp01(_splinePos + _lookAhead);
        Vector3 forwardNow = WaypointSpline.EvaluateTangent(_splinePos);
        Vector3 forwardAhead = WaypointSpline.EvaluateTangent(targetT);

        _targetPosition = WaypointSpline.EvaluatePosition(targetT);
        _curvature = Vector3.Angle(forwardNow, forwardAhead);
        float curveFactor = Mathf.Clamp01(_curvature / 90f);
        if (CurrentAIState == AIState.Driving)
        {
            HandleThrottleInput(speedFactor, curveFactor);
            HandleSteeringInput(true);
        }
        else if (CurrentAIState == AIState.Reversing)
        {
            Throttle = - MaxThrottle;
            HandleSteeringInput(false);
        }
        else
        {
            Throttle = 0;
            Steering = 0;
            HandBrake = false;
        }
    }

    private void HandleThrottleInput(float speedFactor, float curveFactor)
    {
        float brakeThreshold = BrakeAngle / 90f;
        if (HandBrake)
        {
            _timeSinceHandbrakeStarted += Time.deltaTime;
            if ((curveFactor < brakeThreshold * 0.9f) || _timeSinceHandbrakeStarted > BrakeTime)
            {
                HandBrake = false;
                _timeSinceHandbrakeStarted = 0f;
            }
        }
        else if (speedFactor > MinBrakeSpeedFactor && curveFactor > brakeThreshold)
        {
            HandBrake = true;
            _timeSinceHandbrakeStarted = 0f;
        }
        float throttleFactor = (curveFactor > brakeThreshold) ? 0f : curveFactor;
        Throttle = Mathf.Lerp(MaxThrottle, MinThrottle, throttleFactor);
    }

    private void HandleSteeringInput(bool isForward)
    {
        Vector2 direction = (_targetPosition - transform.position).normalized;
        float angle = -Vector2.SignedAngle(transform.up, direction);

        float targetSteering = Mathf.Clamp(angle / MaxSteeringAngle, -1f, 1f);
        if (!isForward)
        {
            targetSteering = -targetSteering;
        }
        //Steering = Mathf.Clamp(angle / MaxSteeringAngle, -1f, 1f);
        Steering = Mathf.Lerp(
            Steering,
            targetSteering,
            Time.deltaTime * SteeringSmooth
        );
    }

    private void UpdateSplinePosition()
    {
        float bestT = _splinePos;
        float bestDist = float.MaxValue;

        int steps = 20;
        float searchRange = 0.05f; // how far ahead/behind to search

        for (int i = 0; i <= steps; i++)
        {
            float offset = (i / (float)steps - 0.5f) * searchRange;
            float sampleT = Mathf.Clamp01(_splinePos + offset);

            Vector3 point = WaypointSpline.EvaluatePosition(sampleT);
            float dist = (transform.position - point).sqrMagnitude;

            if (dist < bestDist)
            {
                bestDist = dist;
                bestT = sampleT;
            }
        }

        _splinePos = (bestT + 1f) % 1f;
    }

    public void SetWaypointSpline()
    {
        GameObject splineObject = GameObject.FindGameObjectWithTag("WaypointSpline");
        if (splineObject == null)
        {
            Debug.LogError("Error setting WaypointSpline for AI, splineObject not found");
        }
        WaypointSpline = splineObject.GetComponent<SplineContainer>();
    }

    public void Initialise(string difficulty)
    {
        if (Enum.TryParse(difficulty, out Difficulty parsedDifficulty))
        {
            SetVariables(parsedDifficulty);
        }
        else
        {
            SetVariables(Difficulty.Easy);
        }
        CurrentAIState = AIState.Driving;
        SetWaypointSpline();
        _isStuck = false;
        _timeSinceStuck = 0f;
        _timeSinceUnstuck = 0f;
        _isActive = true;
    }

    private void SetVariables(Difficulty difficulty)
    {
        if (difficulty == Difficulty.Easy)
        {
            MaxSteeringAngle = Constants.AI_EASY_MAX_STEERING_ANGLE;
            SteeringSmooth = Constants.AI_EASY_STEER_SMOOTHING;
            MaxThrottle = Constants.AI_EASY_MAX_THROTTLE;
            MinThrottle = Constants.AI_EASY_MIN_THROTTLE;
            MinBrakeSpeedFactor = Constants.AI_EASY_MIN_BRAKE_SPEED_FACTOR;
            BrakeAngle = Constants.AI_EASY_BRAKE_ANGLE;
            BrakeTime = Constants.AI_EASY_BRAKE_TIME;
            StuckDetectionTime = Constants.AI_EASY_STUCK_DETECTION_TIME;
        }
        else
        {
            MaxSteeringAngle = Constants.AI_HARD_MAX_STEERING_ANGLE;
            SteeringSmooth = Constants.AI_HARD_STEER_SMOOTHING;
            MaxThrottle = Constants.AI_HARD_MAX_THROTTLE;
            MinThrottle = Constants.AI_HARD_MIN_THROTTLE;
            MinBrakeSpeedFactor = Constants.AI_HARD_MIN_BRAKE_SPEED_FACTOR;
            BrakeAngle = Constants.AI_HARD_BRAKE_ANGLE;
            BrakeTime = Constants.AI_HARD_BRAKE_TIME;
            StuckDetectionTime = Constants.AI_HARD_STUCK_DETECTION_TIME;
        }
    }
}
