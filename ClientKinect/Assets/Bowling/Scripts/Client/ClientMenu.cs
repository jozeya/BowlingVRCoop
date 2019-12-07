using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class ClientMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public int port = 6322;
    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;
    void Start()
    {
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

                    if (data == "P2")
                    {
                        SceneManager.LoadScene("022");
                    }
                    
                }
            }
        }
    }
}
