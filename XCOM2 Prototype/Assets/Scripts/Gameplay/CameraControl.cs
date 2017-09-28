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


    void Update()
    {
        //
        //Move left and right with A & D
        if (transform.position.x >= xPosMin && transform.position.x <= xPosMax)
        {
            if (Input.GetKey(KeyCode.A) && transform.position.x >= xPosMin)
            {
                transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D) && transform.position.x <= xPosMax)
            {
                transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            } 
        }
        //reset movement within boundries
        if (transform.position.x < xPosMin || transform.position.x > xPosMax)
        {
            Vector3 p = transform.position;
            transform.position = new Vector3(Mathf.Clamp(p.x, xPosMin, xPosMax), p.y, p.z);
        }

        //
        //Move forwards and backwards with W & S
        if (transform.position.z >= zPosMin && transform.position.z <= zPosMax)
        {
            if (Input.GetKey(KeyCode.S) && transform.position.z >= zPosMin)
            {
                transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.W) && transform.position.z <= zPosMax)
            {
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }
            
        }
        //reset movement within boundries
        if (transform.position.z < zPosMin || transform.position.z > zPosMax)
        {
            Vector3 p = transform.position;
            transform.position = new Vector3(p.x, p.y, Mathf.Clamp(p.z, zPosMin, zPosMax));
        }

        //
        //Rotate camera
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.down * rotSpeed * Time.deltaTime, Space.World);
        }

        //
        //Zoom in and out
        
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            transform.Translate(Vector3.down * moveSpeed * Input.GetAxis("Mouse ScrollWheel"));
        }
        //reset position if out of bounds
        if (transform.position.y < yPosMin || transform.position.y > yPosMax)
        {
            Vector3 p = transform.position;
            transform.position = new Vector3(p.x, Mathf.Clamp(p.y, yPosMin, yPosMax), p.z);
        }
    }
}
