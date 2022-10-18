using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomVariant : MonoBehaviour
{
    public GameObject[] variants;
    // Start is called before the first frame update
    void Start()
    {
        EnableRandom();
    }

    void EnableRandom() {
        int RNG = Random.Range(0, variants.Length);
        for (int i = 0; i < variants.Length; i++)
        {
            variants[i].SetActive(false);
        }
        variants[RNG].SetActive(true);
    }

    // Update is called once per frame
    void OnValidate()
    {
        EnableRandom();
    }
}
