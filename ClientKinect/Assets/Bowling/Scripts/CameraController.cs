using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public GameObject camera;
    private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = camera.transform.position - player.transform.position;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        //transform.position = player.transform.position + new Vector3(-0.8f*-0.05f, 1.1f, -35.3f*-0.07f); ;
        //camera.transform.position = player.transform.position + offset;
        camera.transform.LookAt(player.transform);
    }

}
