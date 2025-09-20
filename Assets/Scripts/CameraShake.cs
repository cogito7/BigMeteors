using UnityEngine;
using Unity.Cinemachine;


public class CameraShake : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cineCam;
    [SerializeField] private float shakeTime = 0.3f;
    [SerializeField] private float amplitude = 2f;
    [SerializeField] private float frequency = 3f;

    private CinemachineBasicMultiChannelPerlin noise;
    private float timer;

    void Awake()
    {
        noise = cineCam.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeWithCustomValues(float customAmplitude, float customShakeTime)
    {
        if (noise != null)
        {
            noise.AmplitudeGain = customAmplitude;
            noise.FrequencyGain = frequency;
            timer = customShakeTime;
        }
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                noise.AmplitudeGain = 0;
                noise.FrequencyGain = 0;
            }
        }
    }
}