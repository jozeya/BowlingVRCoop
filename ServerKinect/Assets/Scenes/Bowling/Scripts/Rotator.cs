using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {
    // Update is called once per frame
    //use update instead of fixed update because you are not using forces
    private bool rotating;
    private Vector3 startPosition;

   
    
    private void Start()
    {
        rotating = false;
        startPosition = transform.position;

        
    }
    void Update()
    {
        if (rotating)
        {
            transform.Rotate(new Vector3(-1000, 3, 4) * Time.deltaTime);
            
        }
    }

    public void restart()
    {
        transform.position = startPosition;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        rotating = false;
    }

    public void rotar()
    {
        rotating = true;
    }

    
}
