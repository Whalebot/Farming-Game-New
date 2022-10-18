using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    public bool isActive;
    public bool onlyX = true;
    public Vector3 offset;
    public Transform copyTransform;
    Movement mov;
    // Start is called before the first frame update
    void Start()
    {
        mov = GetComponentInParent<Movement>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isActive) return;

        if (mov.strafeTarget != null) {
            transform.LookAt(mov.strafeTarget);
            if (onlyX) transform.localRotation = Quaternion.Euler(transform.rotation.eulerAngles.x + offset.x, 0, transform.rotation.eulerAngles.z);
        }
        else
        {
            transform.rotation = copyTransform.rotation;
            if (onlyX) transform.localRotation = Quaternion.Euler(transform.rotation.eulerAngles.x + offset.x, 0, transform.rotation.eulerAngles.z);
        }
    }
}
