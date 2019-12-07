using UnityEngine;
using System.Collections;

public class Turning : MonoBehaviour {
    public bool turnLeft;
    public bool turnRight;
    private bool rotating;
    public float rotatespeed = 2f;
    private float xAxis;
    
	// Use this for initialization
	void Start () {
        xAxis = 1;
	}
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (turnRight)
            {
                turnRight = false;
                turnLeft = true;
            }
            else
            {
                turnLeft = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (turnLeft)
            {
                turnLeft = false;
                turnRight = true;
            }
            else
            {
                turnRight = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            turnLeft = false;
            turnRight = false;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            turnLeft = false;
            turnRight = false;
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (turnLeft)
        {

            //transform.Rotate(new Vector3(0, -200, 0) * Time.deltaTime);
            transform.Rotate(Vector3.down * Time.deltaTime * (rotatespeed * xAxis));


        }
        if (turnRight)
        {
            //transform.Rotate(new Vector3(0, 200, 0) * Time.deltaTime);
            transform.Rotate(Vector3.up * Time.deltaTime * (rotatespeed * xAxis));
        }
    }

    public void GetXAxis(float x)
    {
        xAxis = x;
    }
}

