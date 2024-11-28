using UnityEngine;

public class Keyboard : MonoBehaviour
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


