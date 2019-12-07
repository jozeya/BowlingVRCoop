using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.IO;

using UnityEngine.SceneManagement;

public class ServerMenu : MonoBehaviour
{
    // Start is called before the first frame update
    private List<ServerClient> clients;
    private List<ServerClient> disconnectList;

    public int port = 6322;
    private TcpListener server;
    private bool serverStarted;

    void Start()
    {
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

    // Update is called once per frame
    void Update()
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
    }

    public void SendMessage(string msg)
    {
        BroadCast(msg, clients);
    }

    public void loadScene(string value)
    {
        if (value == "SampleScene")
        {
            BroadCast("P1", clients);
            server.Stop();
            SceneManager.LoadScene(value);
        }
        else
        {
            BroadCast("P2", clients);
            server.Stop();
            SceneManager.LoadScene(value);
        }
        
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
}
/*public class ServerClientMenu
{
    public TcpClient tcp;
    public string clientName;

    public ServerClientMenu(TcpClient clientSocket)
    {
        clientName = "Guest";
        tcp = clientSocket;
    }
}*/
