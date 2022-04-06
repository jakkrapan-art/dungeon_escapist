using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSnatch : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector2 panLimit;

    public void setTarget(Transform player)
    {
        target = player;
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, -panLimit.x, panLimit.x);
            smoothedPosition.z = Mathf.Clamp(smoothedPosition.z, -panLimit.y - Mathf.Abs(offset.z), panLimit.y - Mathf.Abs(offset.z));
            transform.position = smoothedPosition;
        }
    }
}
