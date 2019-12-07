using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MoveForward : MonoBehaviour
{
 	public float forwardspeed = 2.0f;
    private bool rotating;
    private Vector3 initBallPosition, initForward;
    public GameObject ball;
    private Rotator rotator;

    //jump
    private Rigidbody rb;
    public LayerMask groundLayers;
    public float jumpForce = 7;
    private SphereCollider col;
    //

    public GameObject pins;
    private BowlingPin pinScript;

    private AudioSource[] audio;
    private AudioClip clip1;
    private AudioClip clip2;

    // Use this for initialization 
    void Start()
    {
        rotator = ball.GetComponent<Rotator>();
        rotating = false;
        initBallPosition = transform.position;
        initForward = transform.forward;

        rb = ball.GetComponent<Rigidbody>();
        col = ball.GetComponent<SphereCollider>();

        pinScript = pins.GetComponent<BowlingPin>();

        audio = GetComponents<AudioSource>();
        clip1 = audio[0].clip;
        clip2 = audio[1].clip;
        //ReproduceClip2();
    }

 	// Update is called once per frame 
 	void Update()
    {
        if (rotating)
            transform.Translate(Vector3.forward * Time.deltaTime * -forwardspeed);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rotating = true;
            rotator.rotar();
            ReproduceClip1();
        }

        if (IsGrounded() && Input.GetKeyDown(KeyCode.W))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            ReproduceClip2();
            //rb.AddForce(new Vector3(0, 35, 0), ForceMode.Impulse);

            /*if (ball.transform.position.y < 0.55f) { 
                ball.GetComponent<Rigidbody>().AddForce(new Vector3(0, 35, 0), ForceMode.Impulse);
            }*/
        }
    }

    private bool IsGrounded()
    {
        return Physics.CheckCapsule(col.bounds.center, new Vector3(col.bounds.center.x, 
            col.bounds.min.y, col.bounds.center.z), col.radius * .9f, groundLayers);
    }
    private void FixedUpdate()
    {
        if (ball.transform.position.y < 0.0)
        {
            transform.position = initBallPosition;
            transform.forward = initForward;
            rotating = false;

            rotator.restart();
            pinScript.StatusPins();
            StopMusic();
        }
    }


    public void ShootBall()
    {
        rotating = true;
        rotator.rotar();
        //ReproduceClip1();
    }

    public void Jump()
    {
        if (IsGrounded())
        {
            ReproduceClip2();
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            
        }
    }
    private void ReproduceClip1()
    {
        GetComponent<AudioSource>().PlayOneShot(clip1);
    }

    private void ReproduceClip2()
    {
        GetComponent<AudioSource>().PlayOneShot(clip2);
    }

    private void StopMusic()
    {
        GetComponent<AudioSource>().Stop();
    }

}
