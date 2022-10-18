using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance
    {
        get; private set;
    }
    public GameObject defaultHitVFX;
    public GameObject defaultBlockVFX;
    public GameObject counterHitVFX;
    public GameObject defaultHitSFX;
    public GameObject defaultBlockSFX;
    public GameObject counterHitSFX;
    public GameObject exMoveVFX;

    public GameObject defaultProjectileVFX;
    public GameObject defaultProjectileSFX;

    public GameObject recoveryFX;
    public GameObject wakeupFX;

    public GameObject throwFX;
    public GameObject throwSFX;

    public GameObject throwBreakVFX;
    public GameObject throwBreakSFX;

    public List<ParticleObject> particles;
    public List<ParticleObject> deletedParticles;
    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.advanceGameState += AdvanceParticles;
    }
    [Button]
    public void AdvanceParticles()
    {
        deletedParticles.Clear();
        foreach (var item in particles)
        {
            if (item.ps == null)
            {
                deletedParticles.Add(item);
                continue;
                //Delete shit
            }

            float tempTime = 0;
            tempTime = (GameManager.Instance.gameFrameCount - item.startFrame) * Time.fixedDeltaTime;
            item.ps.Simulate(Time.fixedDeltaTime, true, false, true);
            if (item.ps.time == item.ps.main.duration)
            {
                deletedParticles.Add(item);
                //Delete shit
            }

        }
        for (int i = 0; i < deletedParticles.Count; i++)
        {
            particles.Remove(deletedParticles[deletedParticles.Count - i - 1]);
            if (deletedParticles[deletedParticles.Count - i - 1].ps != null)
                Destroy(deletedParticles[deletedParticles.Count - i - 1].ps.gameObject);
        }
    }

    public void RevertParticles()
    {
        foreach (var item in particles)
        {

            float tempTime = 0;
            tempTime = (GameManager.Instance.gameFrameCount - item.startFrame) * Time.fixedDeltaTime;
            item.ps.Simulate(tempTime, true, true, true);
        }
    }

    public void AddParticle(ParticleSystem ps, int ID)
    {

        ParticleObject p = new ParticleObject(ps, GameManager.Instance.gameFrameCount);
        ps.Simulate(Time.fixedDeltaTime, true, false, true);
        ps.Pause();
        particles.Add(p);
    }
}

[System.Serializable]
public class ParticleObject
{
    public ParticleObject(ParticleSystem p, int i)
    {
        ps = p;
        startFrame = i;
    }
    public ParticleSystem ps;
    public int startFrame;
}