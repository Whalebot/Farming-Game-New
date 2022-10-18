using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponParticles : MonoBehaviour
{

    public GameObject[] particles;
    // Start is called before the first frame update
    void Start()
    {
    }


    public void InterruptAttack()
    {
    }

    public void ActivateParticle(int i)
    {

        DeactivateParticles();
        if (particles.Length > 0)
            if (i < particles.Length)
            {
                ParticleSystem[] ps = particles[i].GetComponentsInChildren<ParticleSystem>();

                foreach (ParticleSystem thisPS in ps)
                {
                    thisPS.Play();
                }

            }
            else
            {
                particles[0].SetActive(true);

                ParticleSystem[] ps = particles[0].GetComponentsInChildren<ParticleSystem>();

                foreach (ParticleSystem thisPS in ps)
                {
                    thisPS.Play();
                }
            }
    }


    public void DeactivateParticles()
    {

        foreach (GameObject particle in particles)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                ParticleSystem[] ps = particles[i].GetComponentsInChildren<ParticleSystem>();

                foreach (ParticleSystem thisPS in ps)
                {
                    thisPS.Stop();
                }
            }
        }
    }
}
