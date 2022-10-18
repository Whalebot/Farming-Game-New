using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSize : MonoBehaviour
{
    public Vector3 randomRange;

    // Start is called before the first frame update
    void OnEnable()
    {
        RandomizeSize();
    }

    void RandomizeSize() {
        transform.localScale = new Vector3(1 + randomRange.x * Random.Range(-1f,1f), 1 + randomRange.y * Random.Range(-1f, 1f), 1 + randomRange.z * Random.Range(-1f, 1f));
    }

    private void OnValidate()
    {
        RandomizeSize();
    }
}
