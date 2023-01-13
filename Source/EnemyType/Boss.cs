using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Boss : MonoBehaviour
{
    PlayerController playerController;

    AudioSource audio;

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    public float ShakeAmplitude = 1.2f;
    public float ShakeFrequency = 2.0f;

    private GameObject GameEndUI;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();

        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        virtualCamera = GameObject.Find("CMV Camera").GetComponent<CinemachineVirtualCamera>();

        if (virtualCamera != null)
            virtualCameraNoise = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }
    public void Scream(GameObject GameEndUI)
    {
        this.GameEndUI = GameEndUI;

        StartCoroutine(BossScream());
    }

    public IEnumerator BossScream()
    {
        StartCoroutine(playerController.End());

        yield return new WaitForSeconds(4.0f);

        audio.Play();
        virtualCameraNoise.m_AmplitudeGain = ShakeAmplitude;
        virtualCameraNoise.m_FrequencyGain = ShakeFrequency;

        yield return new WaitForSeconds(3.0f);

        virtualCameraNoise.m_AmplitudeGain = 0f;
        virtualCameraNoise.m_FrequencyGain = 0f;

        GameEndUI.SetActive(true);
    }
}
