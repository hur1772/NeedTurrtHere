using UnityEngine;
//using Altair_Memory_Pool;
using Altair_Memory_Pool_Pro;
using System.Collections;

public class EffectReturn : MemoryPoolingFlag
{
    private ParticleSystem particle;
    [SerializeField] private float disableTimeCount = 0.0f;
    [SerializeField] private float disableTime = 5.0f;

    [SerializeField] private AudioClip[] startSound = null;
    //private new AudioSource audio;

    private bool isInit = false;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
        //audio = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (isInit) SoundPlay();
        else isInit = true;
        particle.Play();
        disableTimeCount = 0f;
    }

    //private void Start() => StartFunc();

    private void StartFunc()
    {

    }

    private void SoundPlay()
    {
        //audio.PlayOneShot(startSound[Random.Range(0, startSound.Length)]);
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        disableTimeCount += Time.deltaTime;
        if (disableTimeCount > disableTime)
            ObjectReturn();
    }
}
