using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollyZoom : MonoBehaviour
{
    public Transform target; // 目标物体
    public float frustumHeight; // 视锥体高度
    float distance;
    Camera camera1;
    // Start is called before the first frame update
    void Start()
    {
        camera1 = GetComponent<Camera>();
        distance = Vector3.Distance(transform.position, target.position);
        frustumHeight = 2.0f * distance * Mathf.Tan(camera1.fieldOfView * 0.5f * Mathf.Deg2Rad);
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(transform.position, target.position);
        camera1.fieldOfView = 2.0f * Mathf.Atan(frustumHeight * 0.5f / distance) * Mathf.Rad2Deg;
    }
}
