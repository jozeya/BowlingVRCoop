using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BowlingPin : MonoBehaviour
{
    // Start is called before the first frame update
    private List<Vector3> startPos = new List<Vector3>();
    private List<Transform> pins = new List<Transform>();
    private List<int> statusPins = new List<int>();

    public Text scoreText;
    public Text pinsText;

    private int numPinsDown = 0;
    private int pinsTurnOne = 0;

    public GameObject ballOption;
    private MoveForward ballScript;


    public GameObject serverObject;
    private Server msgPins;
    
    void Start()
    {
        ballScript = ballOption.GetComponent<MoveForward>();
        msgPins = serverObject.GetComponent<Server>();

        foreach (Transform child in transform)
        {
            startPos.Add(child.position);
            pins.Add(child);
            statusPins.Add(1);
        }

    }

    // Update is called once per frame
    void Update()
    {
        int value = 0;
        foreach (Transform child in pins)
        {
            if (child.gameObject.activeInHierarchy && ((child.position.y < -5f) ||
            (child.eulerAngles.x > 80.0f && child.eulerAngles.x < 290.0f) ||
            (child.eulerAngles.z > 80.0f && child.eulerAngles.z < 290.0f)))
            {
                child.gameObject.SetActive(false);

                statusPins[value] = 0;

                numPinsDown++;
                pinsText.text = "Pinos Restantes: " + (10 - numPinsDown).ToString();
                scoreText.text = "Puntos Obtenidos: " + (numPinsDown * 25).ToString();
            }
            value++;
        }

        if (numPinsDown == pins.Count)
        {
            Reset();
        }
    }

    private void Reset()
    {
        for (int i = 0; i < pins.Count; i++)
        {
            pins[i].gameObject.SetActive(true);
            pins[i].position = startPos[i];
            pins[i].rotation = Quaternion.identity;
            Rigidbody r = pins[i].GetComponent<Rigidbody>();
            r.velocity = Vector3.zero;
            r.angularVelocity = Vector3.zero;

            statusPins[i] = 1;
        }

        numPinsDown = 0;

    }

    public void StatusPins()
    {
        string msg = "";
        for (int i = 0; i < statusPins.Count - 1; i++)
        {
            msg = msg + statusPins[i].ToString() + ",";
        }
        msg = msg + statusPins[9];

        msgPins.SendMessage(msg);
    }

}
