using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPush : MonoBehaviour
{
    public float pushForce = 500f; 
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PushBall();
        }
    }

    private void PushBall()
    {
        rb.AddForce(transform.right * pushForce);
        Debug.Log("Ball was pushed!");
    }
}


