using System;
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

    private float currRota; // 当前开火点的角度
    public Vector3 worldPoint;
    private Camera mainCamera;

    private void Start()
    {
        worldPoint = new Vector3(0, 0, 100);
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (GameManger.Instance.showMenu)
        {
            return;
        }
        
        // 镜头随鼠标上下移动
        lookAtY += Input.GetAxis("Mouse Y") * Time.deltaTime;
        worldPoint.x = Input.mousePosition.x;
        worldPoint.y = Input.mousePosition.y;
        GameManger.Instance.player.GetComponent<Player>().firePos.LookAt(mainCamera.ScreenToWorldPoint(worldPoint));
        GameManger.Instance.player.GetComponent<Player>().crouchFirePos.LookAt(mainCamera.ScreenToWorldPoint(worldPoint));
        // 限制上下移动的范围
        lookAtY = Mathf.Clamp(lookAtY, 1, 3.5f);
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