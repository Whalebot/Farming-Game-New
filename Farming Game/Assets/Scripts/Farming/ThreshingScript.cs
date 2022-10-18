using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ThreshingScript : MonoBehaviour
{
    InputManager input;
    public int wheatStraws;
    public int wheatGrains;
    [Range(0, 1)] public float handPosition;
    public float handSpeed;
    public bool handSwitch;
    public int repetitions;
    public int repetitionThreshold;


    [Range(0, 1)] public float beatPercentage;
    [Range(0, 1)] public float percentagePerHit;
    public bool charging;
    [Range(0, 1)] public float beatCharge;
    [Range(0, 1)] public float beatSpeed;
    public int beatenGrains;
    // Start is called before the first frame update
    void Start()
    {
        input = InputManager.Instance;
        input.R2input += StartCharge;
        input.westInput += BeatStraws;
    }
    private void OnDisable()
    {

    }
    void StartCharge()
    {
        charging = true;
    }
    [Button]
    void BeatStraws()
    {
        if (charging)
        {
            beatPercentage = Mathf.Clamp01(Mathf.Lerp(beatPercentage, 1, percentagePerHit * beatCharge) + 0.01F);
            beatCharge = 0;
            charging = false;
        }
    }

    [Button]
    void FinishBeating()
    {
        wheatGrains += (int)(10 * beatPercentage * wheatStraws);
        wheatStraws = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (charging)
        {
            if (input.R2Hold)
                beatCharge = Mathf.Clamp01(beatCharge + beatSpeed);
            else beatCharge = Mathf.Clamp01(beatCharge - 0.01F);
        }
        handPosition = Mathf.Clamp01(handPosition + (handSpeed * input.lookDirection.y));
        if (handSwitch)
        {
            if (handPosition > 0.8F)
            {
                handSwitch = false;
                repetitions++;
            }

        }
        else
        {
            if (handPosition < 0.2F)
            {
                handSwitch = true;
                repetitions++;
            }
        }

        if (repetitions > repetitionThreshold)
        {
            repetitions = 0;
            ShellStraw();
        }
    }

    void ShellStraw()
    {
        wheatStraws--;
        wheatGrains += 10;

        if (wheatStraws <= 0) FinishThreshing();
    }

    private void FinishThreshing()
    {

    }
}
