using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPosition : MonoBehaviour
{
    public bool isActive;
    public Transform player;
    public int ID;
    public static int spawnPointID = -1;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;


        if (isActive && spawnPointID == ID)
        {
            player.position = transform.position;
            player.rotation = transform.rotation;
        }

    }

}
