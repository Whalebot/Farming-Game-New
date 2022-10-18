using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPosition : MonoBehaviour
{
    public Movement mov;
    public Transform target;
    public float lerp;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = target.position;
        if (mov.strafeTarget != null)
        {
            Quaternion dir = Quaternion.LookRotation((mov.strafeTarget.position - target.position).normalized, Vector3.up);

            transform.rotation = Quaternion.Lerp(transform.rotation, dir, Time.deltaTime * lerp);

        }
    }
}
