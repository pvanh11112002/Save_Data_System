using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Player name: " + DataManager.Instance.GetPlayerName());
            DataManager.Instance.SetPlayerName("Ethan");
            Debug.Log("Player name after innocent: " + DataManager.Instance.GetPlayerName());
        }    
    }
}
