using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleAI : AI
{
    public float alertRadius;
    public float alertAttackRadius;

    public float rerollTime = 1F;
    bool rerollStart;

    protected override void Alert()
    {
        if (status.currentState == Status.State.InAnimation)
        {
            movement.direction = FindPath().normalized * 0.3F;
        }
        else if (Vector3.Distance(transform.position, target.transform.position) < alertRadius)
        {
            movement.direction = -FindPath().normalized * 0.3F;

        }
        else movement.direction = Vector3.zero;

        movement.strafeTarget = target;
        movement.strafe = true;
        movement.forwardOnly = false;

        if (!inCooldown)
        {
            if (!rerollStart)
            {
                rerollStart = true;
                StartCoroutine("Reroll");
            }
            if (Vector3.Distance(transform.position, target.transform.position) < alertAttackRadius)
            {
                rerollStart = false;
                movement.strafe = false;
                movement.forwardOnly = true;
                currentState = State.Attack;
            }
        }
    }

    IEnumerator Reroll()
    {
        if (currentState != State.Alert) yield break;
        int RNG = Random.Range(0, 2);
        if (RNG == 0)
        {
            movement.strafe = false;
            movement.forwardOnly = true;
            currentState = State.Attack;
            rerollStart = false;
            yield break;
        }

        else
            yield return new WaitForSeconds(rerollTime);
        StartCoroutine("Reroll");
    }
}
