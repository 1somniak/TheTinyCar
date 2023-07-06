using System.Collections;
using UnityEngine;
using UnityEngine.Audio;


public class WheelsFXAI : MonoBehaviour
{
    #region AUDIO REFERENCES

    private AudioClip skidAudio;
    private AudioSource skidAudioSource;
    private float skidAudioVolume;

    private AudioClip suspensionsSound;
    private AudioSource suspensionsAudioSource;
    private float suspensionsAudioVolume;
    private WheelHit wheelHit;

    private bool gameStarted = false;

    private bool suspensionSoundOn = false;

    #endregion

    #region VISUAL SKID REFERENCES

    private ParticleSystem skidParticles;
    private static Transform skidTrailsDetachedParent;
    private Transform skidTrail;
    private Transform skidTrailPrefab;
    private WheelCollider wheelCollider;
    private AnyCarAI ACAI;

    #endregion

    #region BOOLS

    public bool skidding { get; private set; }
    public bool playingAudio { get; private set; }

    #endregion

    private void Start()
    {
        #region VISUAL

        skidParticles = transform.parent.parent.GetComponentInChildren<ParticleSystem>();
        skidTrailPrefab = Resources.Load<Transform>("SkidTrail");
        ACAI = transform.parent.parent.GetComponent<AnyCarAI>();

        if (skidParticles == null)
        {
            Debug.LogWarning(" no particle system found on car to generate smoke particles", gameObject);
        }
        else
        {
            if (!ACAI.smokeOn)
            {
                skidParticles.Stop();
            }            
        }

        wheelCollider = this.transform.GetComponent<WheelCollider>();

        if (skidTrailsDetachedParent == null)
        {
            skidTrailsDetachedParent = new GameObject("TemporarySkids").transform;                
        }

        #endregion

        #region AUDIO

        skidAudio = ACAI.skidSound;
        skidAudioVolume = ACAI.skidVolume;

        if(ACAI.skidSource == null)
        {
            SetUpSkidAudioSource(skidAudio);
        }
        else
        {
            skidAudioSource = ACAI.skidSource;
        }
        
        playingAudio = false;

        suspensionsSound = ACAI.suspensionsSound;
        suspensionsAudioVolume = ACAI.suspensionsVolume;

        if(ACAI.suspensionsSource == null)
        {
            SetUpSuspensionsAudioSource(suspensionsSound);
        }
        else
        {
            suspensionsAudioSource = ACAI.suspensionsSource;
        }

        #endregion
    }

    private void Update()
    {
        WheelHit tempWheelHit;

        this.GetComponent<WheelCollider>().GetGroundHit(out tempWheelHit);

        if (tempWheelHit.normal != wheelHit.normal)
        {
            if (gameStarted)
            {
                suspensionSoundOn = true;
            }
            else
            {
                gameStarted = true;
            }
        }
        else
        {
            suspensionSoundOn = false;
        }

        wheelHit.normal = tempWheelHit.normal;

        SuspensionsSound();
    }

    #region VISUAL FUNCTIONS
    public void EmitTyreSmoke()
    {
        skidParticles.transform.position = transform.position - transform.up * wheelCollider.radius;
        skidParticles.Emit(1);
        if (!skidding)
        {
            StartCoroutine(StartSkidTrail());
        }
    }

    public IEnumerator StartSkidTrail()
    {

        skidTrail = Instantiate(skidTrailPrefab);
        skidding = true;

        while (skidTrail == null)
        {
            yield return null;
        }
        
        skidTrail.parent = this.gameObject.transform;
        skidTrail.localPosition = -Vector3.up * wheelCollider.radius;
    }

    public void EndSkidTrail()
    {
        if (!skidding)
        {
            return;
        }

        skidding = false;
        skidTrail.parent = skidTrailsDetachedParent;
        Destroy(skidTrail.gameObject, 15);
    }

    #endregion

    #region AUDIO FUNCTIONS

    public void PlayAudio()
    {
        skidAudioSource.Play();
        playingAudio = true;
    }

    public void StopAudio()
    {
        skidAudioSource.Stop();
        playingAudio = false;
    }

    private void SuspensionsSound()
    {
        if (suspensionSoundOn)
        {
            if (!suspensionsAudioSource.isPlaying)
            {
                suspensionsAudioSource.PlayOneShot(suspensionsSound);
            }
        }
    }

    private AudioSource SetUpSkidAudioSource(AudioClip clip)
    {
        if (ACAI.skidSource == null)
        {
            skidAudioSource = this.transform.parent.gameObject.AddComponent<AudioSource>();
            ACAI.skidSource = skidAudioSource;
        }
        else
        {
            skidAudioSource = ACAI.skidSource;
        }

        skidAudioSource.clip = clip;
        skidAudioSource.volume = skidAudioVolume;
        skidAudioSource.loop = false;
        skidAudioSource.pitch = 1f;

        skidAudioSource.playOnAwake = false;
        skidAudioSource.minDistance = 5;
        skidAudioSource.reverbZoneMix = 1.5f;
        skidAudioSource.maxDistance = 600;
        skidAudioSource.dopplerLevel = 2;

        return skidAudioSource;
    }

    private AudioSource SetUpSuspensionsAudioSource(AudioClip clip)
    {

        if (ACAI.suspensionsSource == null)
        {
            suspensionsAudioSource = this.transform.parent.gameObject.AddComponent<AudioSource>();
            ACAI.suspensionsSource = suspensionsAudioSource;
        }
        else
        {
            suspensionsAudioSource = ACAI.suspensionsSource;
        }

        suspensionsAudioSource.clip = clip;
        suspensionsAudioSource.volume = suspensionsAudioVolume;
        suspensionsAudioSource.loop = false;
        suspensionsAudioSource.pitch = 1f;

        suspensionsAudioSource.playOnAwake = false;
        suspensionsAudioSource.minDistance = 5;
        suspensionsAudioSource.reverbZoneMix = 1.5f;
        suspensionsAudioSource.maxDistance = 600;
        suspensionsAudioSource.dopplerLevel = 2;

        return suspensionsAudioSource;
    }

    #endregion
}
