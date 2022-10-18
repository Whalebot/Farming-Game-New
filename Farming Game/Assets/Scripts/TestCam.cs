using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class TestCam : MonoBehaviour
{
    public InputManager input;
    public CinemachineFreeLook cam;
    public float xMult, yMult;
    // Start is called before the first frame update
    void Start()
    {
        LoadData();
    }

    void LoadData()
    {
        //xMult = DataManager.Instance.currentSaveData.settings.cameraX / (float)50;
        //yMult = DataManager.Instance.currentSaveData.settings.cameraY / (float)50;
    }

    // Update is called once per frame
    void Update()
    {
        LoadData();
        if (GameManager.isPaused || GameManager.menuOpen)
        {
            cam.m_XAxis.m_InputAxisValue = 0;
            cam.m_YAxis.m_InputAxisValue = 0;
            LoadData();
            return;

        }
        cam.m_XAxis.m_InputAxisValue = input.lookDirection.x * xMult;
        cam.m_YAxis.m_InputAxisValue = input.lookDirection.y * yMult;
        // cam.m_YAxis.m_InputAxisValue = Mathf.Clamp(cam.m_YAxis.m_InputAxisValue, -1, 1);
    }
}
