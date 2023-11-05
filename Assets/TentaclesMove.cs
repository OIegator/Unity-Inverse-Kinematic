using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentaclesMove : MonoBehaviour
{
    public float rotationSpeed = 30.0f;
    public KeyCode rotateLeftKey = KeyCode.Alpha1;
    public KeyCode rotateRightKey = KeyCode.Alpha2;
    public Vector3 rotationAxis = Vector3.forward;

    void Update()
    {
        float rotationAmount = 0.0f;

        if (Input.GetKey(rotateLeftKey))
        {
            rotationAmount = -1.0f; 
        }
        else if (Input.GetKey(rotateRightKey))
        {
            rotationAmount = 1.0f;
        }

        rotationAmount *= rotationSpeed * Time.deltaTime;
        transform.Rotate(rotationAxis, rotationAmount);
    }
}
