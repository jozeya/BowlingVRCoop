using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSource : MonoBehaviour
{
    // Start is called before the first frame update
    private ulong player1;
    private ulong player2;

    void Start()
    {
        player1 = 0;
        player2 = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setPlayer1(ulong id)
    {
        player1 = id;
    }

    public void setPlayer2(ulong id)
    {
        player2 = id;
    }
    
    public ulong getPlayer1()
    {
        return player1;
    }

    public ulong getPlayer2()
    {
        return player2;
    }

    public void AddPlayer(ulong id)
    {
        if (player1 == 0)
        {
            player1 = id;
        }
        else
        {
            player2 = id;
        }
    }

    public void RemovePlayer(ulong id)
    {
        if (player1 == id)
        {
            player1 = 0;
        }

        if (player2 == id)
        {
            player2 = 0;
        }
    }

    public int GetPlayer(ulong id)
    {
        if (id == player1)
        {
            return 1;
        }

        else
        {
            return 2;
        }
    }
}
