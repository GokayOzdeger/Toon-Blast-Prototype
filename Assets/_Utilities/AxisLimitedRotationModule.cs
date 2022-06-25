using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class AxisLimitedRotationModule : ModifiedMonoBehaviour
{
    [SerializeField] private float defaultSpeed = 50;
    [Header("Axises")]
    [SerializeField] private bool x;
    [SerializeField] private bool y;
    [SerializeField] private bool z;

    private bool _paused = true;
    private Quaternion _targetQuaternion;
    private float _timeNeeded = 0;
    private float _counter = 0;
    private Quaternion _defaultRotation;
    private bool _returningToDefault;

    private void Start()
    {
        _defaultRotation = transform.localRotation;
    }


    private void Update()
    {
        if (!_paused)
        {
            _counter += Time.deltaTime;
            if (!_returningToDefault) Rotate();
            else RotateToDefault();
        }
    }

    private void Rotate()
    {
        float interpolation = _counter / _timeNeeded;
        Vector3 eulers = transform.eulerAngles;
        Vector3 calculatedAngles = Quaternion.Slerp(transform.rotation, _targetQuaternion, interpolation).eulerAngles;
        if (!x)
        {
            calculatedAngles = new Vector3(eulers.x, calculatedAngles.y, calculatedAngles.z);
        }
        if (!y)
        {
            calculatedAngles = new Vector3(calculatedAngles.x, eulers.y, calculatedAngles.z);
        }
        if (!z)
        {
            calculatedAngles = new Vector3(calculatedAngles.x, calculatedAngles.y, eulers.z);
        }
        transform.eulerAngles = calculatedAngles;

        if (interpolation > 1) { _paused = true; }
    }

    private void RotateToDefault()
    {
        float interpolation = _counter / _timeNeeded;
        Vector3 eulers = transform.localEulerAngles;
        Vector3 calculatedAngles = Quaternion.Slerp(transform.localRotation, _targetQuaternion, interpolation).eulerAngles;
        if (!x)
        {
            calculatedAngles = new Vector3(eulers.x, calculatedAngles.y, calculatedAngles.z);
        }
        if (!y)
        {
            calculatedAngles = new Vector3(calculatedAngles.x, eulers.y, calculatedAngles.z);
        }
        if (!z)
        {
            calculatedAngles = new Vector3(calculatedAngles.x, calculatedAngles.y, eulers.z);
        }
        if (interpolation > 1) { _paused = true; _returningToDefault = false; return; }
        transform.localEulerAngles = calculatedAngles;
    }

    public void ReturnToDefaultRotation(float speed = 0)
    {
        float usedSpeed = defaultSpeed;
        if (speed != 0) usedSpeed = speed;
        _counter = 0;
        _paused = false;
        _returningToDefault = true;
        _targetQuaternion = _defaultRotation;
        _timeNeeded = Quaternion.Angle(transform.rotation, _targetQuaternion) / usedSpeed;
    }

    public void TurnTowardsWithTime(Vector3 point,float seconds)
    {
        _counter = 0;
        _paused = false;
        _returningToDefault = false;
        Vector3 lookVector = point - transform.position;
        if (lookVector != Vector3.zero)
        {
            _targetQuaternion = Quaternion.LookRotation(lookVector);
        }
        _timeNeeded = seconds;
    }

    public void TurnTowardsWithSpeed(Vector3 point, float speed = 0)
    {
        float usedSpeed = defaultSpeed;
        if (speed != 0) usedSpeed = speed;
        _counter = 0;
        _paused = false;
        _returningToDefault = false;
        Vector3 lookVector = point - transform.position;
        if (lookVector != Vector3.zero)
        {
            _targetQuaternion = Quaternion.LookRotation(lookVector);
        }
        _timeNeeded = Quaternion.Angle(transform.rotation, _targetQuaternion) / usedSpeed;
    }

    public void TurnTowardsWithTime(Quaternion quat, float seconds)
    {
        _counter = 0;
        _paused = false;
        _returningToDefault = false;
        _targetQuaternion = quat;
        _timeNeeded = seconds;
    }

    public void TurnTowardsWithSpeed(Quaternion quat, float speed)
    {
        _counter = 0;
        _paused = false;
        _returningToDefault = false;
        _targetQuaternion = quat;
        _timeNeeded = Quaternion.Angle(transform.rotation, _targetQuaternion) / speed;
    }


}
