using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.IO;

using UnityEngine.SceneManagement;

public class Server : MonoBehaviour
{
    private List<ServerClient> clients;
    private List<ServerClient> disconnectList;

    public int port = 6321;
    private TcpListener server;
    private bool serverStarted;

    public GameObject ballObject;
    public GameObject playerObject;
    private bool rotating = true;
    private string strPosition;
    private Vector3 startPosition;
    

    private void Start()
    {
        startPosition = ballObject.transform.position;
        strPosition = playerObject.transform.position.ToString();

        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();

        try
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            StartListening();
            serverStarted = true;

            Debug.Log("Server has been started on port " + port.ToString());
        }
        catch (System.Exception e)
        {
            Debug.Log("Socket error : " + e.Message);
        }
    }

    private void Update()
    {
        if (!serverStarted)
            return;

        foreach (ServerClient c in clients)
        {
            if (!IsConnected(c.tcp))
            {
                c.tcp.Close();
                disconnectList.Add(c);
                continue;
            }
            else
            {
                NetworkStream s = c.tcp.GetStream();
                if (s.DataAvailable)
                {
                    StreamReader reader = new StreamReader(s, true);
                    string data = reader.ReadLine();

                    if (data != null)
                    {
                        ONIncomingData(c, data);
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //rotating = true;
            BroadCast("StartRotator", clients);
        }

        /*if (playerObject.transform.position.y < 0.35)
        {
            BroadCast("StopRotator", clients);
            //rotating = false;
        }*/

        strPosition = playerObject.transform.position.ToString();
    }

    private void FixedUpdate()
    {
        if (rotating)
        {
            //BroadCast("P", clients);
            BroadCast(strPosition + "#" + ballObject.transform.forward.ToString(), clients);
            //BroadCast(ballObject.transform.forward.ToString(), clients);
        }
    }

    public void SendMessage(string msg)
    {
        BroadCast("Pins", clients);
        BroadCast(msg, clients);
    }

    private void ONIncomingData(ServerClient c, string data)
    {
        Debug.Log(c.clientName + " has sent the following message : " + data);
    }

    private void BroadCast(string data, List<ServerClient> cl)
    {
        foreach (ServerClient c in cl)
        {
            try
            {
                StreamWriter writer = new StreamWriter(c.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch (Exception e)
            {
                Debug.LogError("Write error : " + e.Message + " to client " + c.clientName);
            }
        }
    }

    private void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }

    private bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }
    private void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;

        clients.Add(new ServerClient(listener.EndAcceptTcpClient(ar)));
        StartListening();

        //send message to everyone, say someone has connected
        //BroadCast(clients[clients.Count - 1].clientName + " has connected", clients);
    }

    public void Exit()
    {
        BroadCast("Exit", clients);
        server.Stop();
        SceneManager.LoadScene("Menu2");
    }
}
public class ServerClient
{
    public TcpClient tcp;
    public string clientName;

    public ServerClient(TcpClient clientSocket)
    {
        clientName = "Guest";
        tcp = clientSocket;
    }

}
