using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    public CinemachineConfiner confiner;
    public CinemachineVirtualCamera[] cameras;
    public Movement mov;
    public CinemachineFreeLook freeLookCam;

    CinemachineBasicMultiChannelPerlin[] noises;
    [SerializeField] private float shakeTimer;
    private float startTimer;
    private float startIntensity;
    public bool toggle;
    public CinemachineVirtualCamera lockOnCam;
    public CinemachineVirtualCamera lockOnCam2;


    public CinemachineTargetGroup targetGroup;
    public CinemachineVirtualCamera groupCamera;

    public Transform defaultTarget;
    public float xMult, yMult;
    CinemachineComposer topComposer, middleComposer, bottomComposer;
    public float camLerp;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //GameObject boundingBox = GameObject.FindGameObjectWithTag("CameraBoundary");
        //if (boundingBox != null)
        //    confiner.m_BoundingVolume = boundingBox.GetComponent<Collider>();

        noises = new CinemachineBasicMultiChannelPerlin[cameras.Length];
        for (int i = 0; i < noises.Length; i++)
        {
            noises[i] = cameras[i].GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }


        freeLookCam.m_XAxis.Value = 0;
        freeLookCam.m_YAxis.Value = 0.5f;

        //freeLookCam.m_YAxisRecentering.RecenterNow();
        //freeLookCam.m_RecenterToTargetHeading.RecenterNow();

        ShakeCamera(0, 0.1F);


        topComposer = freeLookCam.GetRig(0).GetCinemachineComponent<CinemachineComposer>();
        middleComposer = freeLookCam.GetRig(1).GetCinemachineComponent<CinemachineComposer>();
        bottomComposer = freeLookCam.GetRig(2).GetCinemachineComponent<CinemachineComposer>();
    }

    void Noise(float amplitude)
    {

        freeLookCam.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitude;
        freeLookCam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitude;
        freeLookCam.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitude;
    }

    public void SetLockOnTarget(Transform temp)
    {
        toggle = !toggle;
        StartCoroutine("DelayCamSwitch", temp);
        //if (toggle)
        //{
        //    lockOnCam.LookAt = temp;


        //    lockOnCam.Priority = 15;
        //    lockOnCam2.Priority = 14;


        //    lockOnCam.gameObject.SetActive(true);
        //    lockOnCam2.gameObject.SetActive(false);

        //}
        //else
        //{
        //    lockOnCam2.LookAt = temp;

        //    lockOnCam.Priority = 14;
        //    lockOnCam2.Priority = 15;
        //    lockOnCam2.gameObject.SetActive(true);
        //    lockOnCam.gameObject.SetActive(false);

        //}

    }

    IEnumerator DelayCamSwitch(Transform temp)
    {

        lockOnCam.LookAt = targetGroup.transform;
        yield return new WaitForFixedUpdate();
        lockOnCam.Priority = 15;
        SetGroupTarget(temp);
        
        //if (toggle)
        //{
        //    lockOnCam.LookAt = targetGroup.transform;
        //    lockOnCam.gameObject.SetActive(true);
        //    yield return new WaitForFixedUpdate();
        //    lockOnCam.Priority = 15;
        //    lockOnCam2.Priority = 14;
        //    SetGroupTarget(temp);
        //    lockOnCam2.gameObject.SetActive(false);

        //}
        //else
        //{
        //    lockOnCam2.LookAt = targetGroup.transform;
        //    lockOnCam2.gameObject.SetActive(true);
        //    yield return new WaitForFixedUpdate();
        //    lockOnCam.Priority = 14;
        //    lockOnCam2.Priority = 15;
        //    SetGroupTarget(temp);
        //    lockOnCam.gameObject.SetActive(false);

        //}

    }

    public void DisableLockOn()
    {
        lockOnCam.Priority = 10;
        targetGroup.m_Targets[1].target = defaultTarget;
    }

    public void SetGroupTarget(Transform temp)
    {
        targetGroup.m_Targets[1].target = temp;
        // groupCamera.gameObject.SetActive(true);
        shakeTimer = 0;
        for (int i = 0; i < noises.Length; i++)
            noises[i].m_AmplitudeGain = 0;
    }

    public void SetGroupTarget()
    {
        groupCamera.gameObject.SetActive(true);
        shakeTimer = 0;
        for (int i = 0; i < noises.Length; i++)
            noises[i].m_AmplitudeGain = 0;
    }

    public void RevertCamera()
    {
        groupCamera.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            {
                for (int i = 0; i < noises.Length; i++)
                {
                    noises[i].m_AmplitudeGain = Mathf.Lerp(startIntensity, 0f, (1 - (shakeTimer / startTimer)));

                }
                Noise(Mathf.Lerp(startIntensity, 0f, (1 - (shakeTimer / startTimer))));

            }
        }


        if (GameManager.isPaused || GameManager.menuOpen)
        {
            freeLookCam.m_XAxis.m_InputAxisValue = 0;
            freeLookCam.m_YAxis.m_InputAxisValue = 0;
            LoadData();
            return;

        }
        freeLookCam.m_XAxis.m_InputAxisValue = InputManager.Instance.lookDirection.x * xMult;
        freeLookCam.m_YAxis.m_InputAxisValue = InputManager.Instance.lookDirection.y * yMult;

        if (!mov.ground && !mov.stairs)
        {
            topComposer.m_ScreenY = Mathf.Lerp(topComposer.m_ScreenY ,0.55f, camLerp);
            middleComposer.m_ScreenY = Mathf.Lerp(topComposer.m_ScreenY, 0.45F, camLerp);
            bottomComposer.m_ScreenY = Mathf.Lerp(topComposer.m_ScreenY, 0.50f, camLerp);
            freeLookCam.m_Orbits[0].m_Radius = Mathf.Lerp(freeLookCam.m_Orbits[0].m_Radius, 4, camLerp);
            freeLookCam.m_Orbits[1].m_Radius = Mathf.Lerp(freeLookCam.m_Orbits[1].m_Radius, 6, camLerp);
            freeLookCam.m_Orbits[2].m_Radius = Mathf.Lerp(freeLookCam.m_Orbits[2].m_Radius, 4, camLerp);
        }
        else {
            topComposer.m_ScreenY = Mathf.Lerp(topComposer.m_ScreenY, 0.65f, camLerp);
            middleComposer.m_ScreenY = Mathf.Lerp(topComposer.m_ScreenY, 0.55F, camLerp);
            bottomComposer.m_ScreenY = Mathf.Lerp(topComposer.m_ScreenY, 0.60f, camLerp);
            freeLookCam.m_Orbits[0].m_Radius = Mathf.Lerp(freeLookCam.m_Orbits[0].m_Radius, 3, camLerp);
            freeLookCam.m_Orbits[1].m_Radius = Mathf.Lerp(freeLookCam.m_Orbits[1].m_Radius, 5, camLerp);
            freeLookCam.m_Orbits[2].m_Radius = Mathf.Lerp(freeLookCam.m_Orbits[2].m_Radius, 3, camLerp);
        }
    }

    void LoadData()
    {
        //xMult = DataManager.Instance.currentSaveData.settings.cameraX / (float)50;
        //yMult = DataManager.Instance.currentSaveData.settings.cameraY / (float)50;
    }

    public void ShakeCamera(float intensity, float time)
    {
        startIntensity = intensity;
        shakeTimer = time;
        startTimer = time;
    }
}