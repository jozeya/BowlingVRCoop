using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody rb;
    public GameObject ball;
    void Start()
    {
        rb = ball.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            rb.AddForce(new Vector3(0, 35, 0), ForceMode.Impulse);
        }
    }

    public void JumpNow()
    {
        rb.AddForce(new Vector3(0, 35, 0), ForceMode.Impulse);
    }
}
