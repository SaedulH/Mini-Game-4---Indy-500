using UnityEngine;

[CreateAssetMenu(fileName = "AIStats", menuName = "AIStats")]
public class AIStats : ScriptableObject
{
    [field: SerializeField, Range(min: 0, max: 90)] public float SteeringRatio { get; set; }
    [field: SerializeField, Range(min: 0, max: 50)] public float SteeringSmooth { get; set; }
    [field: SerializeField, Range(min: 0, max: 1)] public float MaxThrottle { get; set; }
    [field: SerializeField, Range(min: 0, max: 1)] public float MinThrottle { get; set; }
    [field: SerializeField, Range(min: 0, max: 1)] public float MinBrakeSpeedFactor { get; set; }
    [field: SerializeField, Range(min: 0, max: 100)] public float BrakeAngle { get; set; }
    [field: SerializeField, Range(min: 0, max: 3)] public float BrakeTime { get; set; }
    [field: SerializeField, Range(min: 0, max: 3)] public float StuckDetectionTime { get; set; }
}
