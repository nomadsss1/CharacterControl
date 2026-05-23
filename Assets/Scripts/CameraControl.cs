using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraControl : MonoBehaviour
{
    [SerializeField]
    private Transform m_focus;
    
    [SerializeField, Range(1f, 20f)]
    private float m_distance = 5f;
    private Vector3 m_focusPoint,m_previousFocusPoint;
    [SerializeField, Min(0f)]private float m_freeMoveRadius = 1f;
    [SerializeField, Range(0f, 1f)]private float m_focusCentering = 0.5f;
    private Vector2 m_orbitAngles = new Vector2(45f, 0f);
    [SerializeField, Range(1f, 360f)]private float m_rotationSpeed = 90f;
    [SerializeField, Range(0f, 90f)]private float alignSmoothRange = 45f;
    [SerializeField, Range(-89f, 89f)]private float m_minVerticalAngle = -30f, m_maxVerticalAngle = 60f;
    [SerializeField, Min(0f)]private float m_alignDelay = 5f;
    private float m_lastManualRotationTime;
    private Camera m_camera;
    private Vector3 cameraHalfExtends{
        get
        {
            Vector3 halfExtends;
            halfExtends.y = m_camera.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * m_camera.fieldOfView);
            halfExtends.x = halfExtends.y * m_camera.aspect;
            halfExtends.z = 0f;
            return halfExtends;
        }
    }

    void Awake()
    {
        m_focusPoint = m_focus.position;
        transform.localRotation = Quaternion.Euler(m_orbitAngles);
        m_camera = GetComponent<Camera>();
    }
    void LateUpdate()
    {
        UpdateFocusPoint();
        Quaternion lookRotation;
        if(ManualRotation() || AutomaticRotation())
        {
            ConstrainAngles();
            lookRotation = Quaternion.Euler(m_orbitAngles);
        }
        else
        {
            lookRotation = transform.localRotation;
        }
        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = m_focusPoint - lookDirection * m_distance;

        Vector3 rectOffset = lookDirection * m_camera.nearClipPlane;
        Vector3 rectPosition = lookPosition + rectOffset;
        Vector3 castFrom = m_focus.position;
        Vector3 castLine = rectPosition - castFrom;
        float castDistance = castLine.magnitude;
        Vector3 castDirection = castLine / castDistance;
        if(Physics.BoxCast(castFrom, cameraHalfExtends, castDirection , out RaycastHit hit, lookRotation ,castDistance,1<<0))
        {
            rectPosition = castFrom + castDirection * hit.distance;
            lookPosition = rectPosition - rectOffset;
        }
        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }
    void UpdateFocusPoint()
    {
        m_previousFocusPoint = m_focusPoint;
        Vector3 targetPoint = m_focus.position;
        if(m_freeMoveRadius > 0)
        {
            float distance = Vector3.Distance(targetPoint,m_focusPoint);
            float t = 1f;
            if(distance > 0.01f && m_focusCentering > 0)
            {
                t = Mathf.Pow(1 - m_focusCentering, Time.unscaledDeltaTime);
            }
            if(distance > m_freeMoveRadius)
            {
                //m_focusPoint = Vector3.Lerp(targetPoint,m_focusPoint,m_freeMoveRadius / distance);
                t = Mathf.Min(t, m_freeMoveRadius / distance);
            }
            m_focusPoint = Vector3.Lerp(targetPoint,m_focusPoint,t);
        }
        else
        {
            m_focusPoint = targetPoint;
        }
    }
    bool ManualRotation()
    {
        Vector2 input = new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
        if(input.x < -float.Epsilon || input.x > float.Epsilon || input.y < -float.Epsilon || input.y > float.Epsilon)
        {
            m_orbitAngles += m_rotationSpeed * Time.unscaledDeltaTime * input;
            m_lastManualRotationTime = Time.unscaledTime;
            return true;
        }
        return false;
    }

    private void ConstrainAngles()
    {
        m_orbitAngles.x = Mathf.Clamp(m_orbitAngles.x, m_minVerticalAngle, m_maxVerticalAngle);
        if(m_orbitAngles.y < 0f)
        {
            m_orbitAngles.y += 360f;
        }
        else if(m_orbitAngles.y >= 360f)
        {
            m_orbitAngles.y -= 360f;
        }
    }

    bool AutomaticRotation()
    {
        if(Time.unscaledTime - m_lastManualRotationTime < m_alignDelay)
        {
            return false;
        }
        Vector2 movement = new Vector2(m_focusPoint.x - m_previousFocusPoint.x, m_focusPoint.z - m_previousFocusPoint.z);
        float movementDeltaSqr = movement.sqrMagnitude;
        if(movementDeltaSqr < 0.0001f)
        {
            return false;
        }
        float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));
        float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(m_orbitAngles.y, headingAngle));
        float rotationChange = m_rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);
        if (deltaAbs < alignSmoothRange)
        {
            rotationChange *= deltaAbs / alignSmoothRange;
        }
        else if (180f - deltaAbs < alignSmoothRange)
        {
            rotationChange *= (180f - deltaAbs) / alignSmoothRange;
        }
        m_orbitAngles.y = Mathf.MoveTowardsAngle(m_orbitAngles.y, headingAngle, rotationChange);
        return true;
    }

    private static float GetAngle(Vector2 direction)
    {
        float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
        return direction.x < 0f ? 360f - angle : angle;
    }
    private void OnValidate()
    {
        if (m_maxVerticalAngle < m_minVerticalAngle)
        {
            m_maxVerticalAngle = m_minVerticalAngle;
        }
    }
}
