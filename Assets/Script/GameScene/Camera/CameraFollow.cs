﻿using System;
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

    private void Update()
    {
        // 镜头随鼠标上下移动
        lookAtY += Input.GetAxis("Mouse Y") * Time.deltaTime;
        
        // 限制上下移动的范围
        lookAtY = Mathf.Clamp(lookAtY, 1, 3);
    }

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
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(lookAtPos - transform.position), Time.deltaTime * 50); 
    }
}