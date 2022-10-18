using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainScript : MonoBehaviour
{
    public ParticleSystem ps;
    public Transform playerPos;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = playerPos.position + Vector3.up * 15;
        ps.GetComponent<ParticleSystemRenderer>().velocityScale = 0.1F * Mathf.Clamp(Time.timeScale - 0.1F, 0,1);
    }
}
