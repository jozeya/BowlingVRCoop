using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour
{

    public int port = 6322;
    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    public GameObject ball;
    public GameObject ball2;
    private string position, fwd;
    private Rotator rotator;

    public GameObject pins;
    private RefreshPins statusPins; 

    private void Start()
    {
        rotator = ball2.GetComponent<Rotator>();
        statusPins = pins.GetComponent<RefreshPins>();
        ConnectToServer();
    }
    public void ConnectToServer()
    {
        if (socketReady)
            return;

        string host = "192.168.10.3";
        //string host = "localhost";
        //port = 6322;

        //create the socket

        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            socketReady = true;

        }
        catch (System.Exception e)
        {
            Debug.LogError("Socket error : " + e.Message);
        }
    }

    private void Update()
    {
        if (socketReady)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                {
                    if (data == "P1")
                    {
                        SceneManager.LoadScene("SampleScene");
                    }
                    else if (data == "StartRotator")
                    {
                        rotator.rotar();
                    }
                    else if (data == "Pins")
                    {
                        string dataPins = reader.ReadLine();
                        string[] mArray = dataPins.Split(',');
                        statusPins.Refresh(mArray);
                    }
                    else if (data == "Exit")
                    {
                        SceneManager.LoadScene("Intro");
                    }
                    else
                    {
                        OnIncomingData(data);
                    }
                }
            }
        }
    }

    private void OnIncomingData(string data)
    {
        string[] mData = SplitMsg(data);
        ball.transform.position = StringToVector3(mData[0]);
        ball.transform.forward = StringToVector3(mData[1]);

        //Debug.Log(data);
    }

    private static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }

    private static string[] SplitMsg(string msg)
    {
        string[] mArray = msg.Split('#');
    ;
        return mArray;
    }
}
