using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshPins : MonoBehaviour
{
    // Start is called before the first frame update
    private List<Transform> pins = new List<Transform>();

    void Start()
    {
        foreach (Transform child in transform)
        {
            pins.Add(child);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Refresh(string[] data)
    {
        for (int i = 0; i < 10; i++)
        {
            if (data[i] == "0")
            {
                pins[i].gameObject.SetActive(false);
            }
        }
    }
}
