﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
    [Header("X Position")]
    [Tooltip("Min and Max values for the cameras X Position")]
    public float xPosMin;
    public float xPosMax;
    [Header("Y Position")]
    [Tooltip("Min and Max values for the cameras y Position")]
    public float yPosMin;
    public float yPosMax;
    [Header("Z Position")]
    [Tooltip("Min and Max values for the cameras z Position")]
    public float zPosMin;
    public float zPosMax;
    [Header("Speed")]
    [Tooltip("Speed for camera rotation")]
    [SerializeField]
    float moveSpeed = 50.0f;
    [Tooltip("Speed for camera movement")]
    [SerializeField]
    float rotSpeed  = 45.0f;
    [Header("Camera Target")]
    public GameObject CameraTarget;
    public GameObject Camera;

    Vector3 targetPosition;
    Vector3 startPosition;
    float moveToTargetLerp = 0;
    float rotateLerp = 1;
    Vector3 targetRotation;
    int yRotation = 45;
    float currentScroll = -1;
    bool movingCamera = false;
    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        
        //Move left and right with A & D
        if (CameraTarget.transform.position.x >= xPosMin && CameraTarget.transform.position.x <= xPosMax)
        {
            if (Input.GetKey(KeyCode.A) && CameraTarget.transform.position.x >= xPosMin)
            {
                movingCamera = false;
                CameraTarget.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D) && CameraTarget.transform.position.x <= xPosMax)
            {
                movingCamera = false;
                CameraTarget.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            } 
        }
        //reset movement within boundries
        if (CameraTarget.transform.position.x < xPosMin || CameraTarget.transform.position.x > xPosMax)
        {
            Vector3 p = CameraTarget.transform.position;
            CameraTarget.transform.position = new Vector3(Mathf.Clamp(p.x, xPosMin, xPosMax), p.y, p.z);
        }

        
        //Move forwards and backwards with W & S
        if (CameraTarget.transform.position.z >= zPosMin && CameraTarget.transform.position.z <= zPosMax)
        {
            if (Input.GetKey(KeyCode.S) && CameraTarget.transform.position.z >= zPosMin)
            {
                movingCamera = false;
                CameraTarget.transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.W) && CameraTarget.transform.position.z <= zPosMax)
            {
                movingCamera = false;
                CameraTarget.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }
            
        }
        //reset movement within boundries
        if (CameraTarget.transform.position.z < zPosMin || CameraTarget.transform.position.z > zPosMax)
        {
            Vector3 p = CameraTarget.transform.position;
            CameraTarget.transform.position = new Vector3(p.x, p.y, Mathf.Clamp(p.z, zPosMin, zPosMax));
        }

        
        //Rotate camera
        if (Input.GetKeyDown(KeyCode.Q))
        {
            yRotation += 90;
            targetRotation = new Vector3(0, yRotation, 0);
            rotateLerp = 0;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            yRotation -= 90;
            targetRotation = new Vector3(0, yRotation, 0);
            rotateLerp = 0;
        }
        if (rotateLerp < 1)
        {
            rotateLerp += Time.deltaTime;
            CameraTarget.transform.rotation = Quaternion.Lerp(CameraTarget.transform.rotation, Quaternion.Euler(targetRotation), Mathf.SmoothStep(0, 1, rotateLerp));
        }

        
        //Zoom in and out
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && currentScroll < 1)
        {
            currentScroll += Input.GetAxis("Mouse ScrollWheel");
            Camera.transform.position += Camera.transform.forward * Input.GetAxis("Mouse ScrollWheel") * 10;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && currentScroll > -1)
        {
            currentScroll += Input.GetAxis("Mouse ScrollWheel");
            Camera.transform.position += Camera.transform.forward * Input.GetAxis("Mouse ScrollWheel") * 10;
        }

        //Moves camera to selected unit
        if (moveToTargetLerp <= 1 && movingCamera)
        {
            moveToTargetLerp += Time.deltaTime / 0.5f;
            CameraTarget.transform.position = Vector3.Lerp(startPosition, targetPosition, Mathf.SmoothStep(0, 1, moveToTargetLerp));
        }
    }

    public void MoveToTarget(Vector3 selectedPosition, float time)
    {
        movingCamera = true;
        moveToTargetLerp = time;
        startPosition = CameraTarget.transform.position;
        targetPosition = selectedPosition;
    }
}
