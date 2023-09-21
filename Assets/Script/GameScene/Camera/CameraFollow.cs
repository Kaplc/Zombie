using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTransform;
    public Vector3 offSet;
    private Vector3 cameraPos;
    public float lookAtY;
    public Vector3 lookAtPos;

    public float followSpeed;

    // Update is called once per frame
    void LateUpdate()
    {
        // 先向后偏移
        cameraPos = playerTransform.position + playerTransform.forward * offSet.z;
        cameraPos += playerTransform.up * offSet.y;
        cameraPos += playerTransform.right * offSet.x;
        // 跟随
        transform.position = Vector3.Lerp(transform.position, cameraPos, Time.deltaTime * followSpeed);

        // 看向
        lookAtPos = playerTransform.position + Vector3.up * lookAtY;
        transform.rotation = Quaternion.LookRotation(lookAtPos - transform.position);
    }
}