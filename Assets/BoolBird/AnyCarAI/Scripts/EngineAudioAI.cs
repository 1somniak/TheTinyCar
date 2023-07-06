using UnityEngine;

public class EngineAudioAI : MonoBehaviour
{
    #region REFERENCES

    private AudioClip lowAccelClip;
    private AudioClip lowDecelClip;
    private AudioClip highAccelClip;
    private AudioClip highDecelClip;
    private AudioClip nosAudioClip;
    private AudioClip turboAudioClip;

    private AudioSource lowAccelSource;
    private AudioSource lowDecelSource;
    private AudioSource highAccelSource;
    private AudioSource highDecelSource;
    private AudioSource nosAudioSource;
    private AudioSource turboAudioSource;

    private AnyCarAI ACAI;

    #endregion

    #region UTILITY

    private float engineVolume;
    private float nosVolume;
    private float turboVolume;

    private float lowPitchMin = 1f;
    private float lowPitchMax = 6f;
    private float pitchMultiplier = 1f;
    private float highPitchMultiplier = 0.25f;

    #endregion

    void Start()
    {
        ACAI = this.transform.GetComponent<AnyCarAI>();

        #region MAIN ENGINE

        #region VOLUME

        engineVolume = ACAI.engineVolume;

        #endregion

        #region CLIPS

        lowAccelClip = ACAI.lowAcceleration;
        lowDecelClip = ACAI.lowDeceleration;
        highAccelClip = ACAI.highAcceleration;
        highDecelClip = ACAI.highDeceleration;

        #endregion

        #region SOURCES

        highAccelSource = SetUpEngineAudioSource(highAccelClip);
        lowAccelSource = SetUpEngineAudioSource(lowAccelClip);
        lowDecelSource = SetUpEngineAudioSource(lowDecelClip);
        highDecelSource = SetUpEngineAudioSource(highDecelClip);

        #endregion

        #endregion

        #region TURBO

        if (ACAI.turboON)
        {
            turboVolume = ACAI.turboVolume;
            turboAudioClip = ACAI.turboAudioClip;
            turboAudioSource = SetUpEngineAudioSource(turboAudioClip);
            turboAudioSource.volume = turboVolume;
        }

        #endregion
    }


    void Update()
    {
        PlayEngineSound();
    }

    private void PlayEngineSound()
    {
        float pitch = SoundLerp(lowPitchMin, lowPitchMax, ACAI.RPM);

        pitch = Mathf.Min(lowPitchMax, pitch);


        lowAccelSource.pitch = pitch * pitchMultiplier;
        lowDecelSource.pitch = pitch * pitchMultiplier;
        highAccelSource.pitch = pitch * highPitchMultiplier * pitchMultiplier;
        highDecelSource.pitch = pitch * highPitchMultiplier * pitchMultiplier;

        float ACAIFade = Mathf.Abs(ACAI.AccelInput);
        float decFade = 1 - ACAIFade;


        float highFade = Mathf.InverseLerp(0.2f, 0.8f, ACAI.RPM);
        float lowFade = 1 - highFade;


        highFade = 1 - ((1 - highFade) * (1 - highFade));
        lowFade = 1 - ((1 - lowFade) * (1 - lowFade));
        ACAIFade = 1 - ((1 - ACAIFade) * (1 - ACAIFade));
        decFade = 1 - ((1 - decFade) * (1 - decFade));


        lowAccelSource.volume = lowFade * ACAIFade * engineVolume;
        lowDecelSource.volume = lowFade * decFade * engineVolume;
        highAccelSource.volume = highFade * ACAIFade * engineVolume;
        highDecelSource.volume = highFade * decFade * engineVolume;

        if (ACAI.turboON)
        {
            turboAudioSource.pitch = pitch * highPitchMultiplier * pitchMultiplier;
            turboAudioSource.volume = highFade * ACAIFade * turboVolume + 0.1f;
        }
    }

    private AudioSource SetUpEngineAudioSource(AudioClip clip)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = 0;
        source.loop = true;
        source.time = Random.Range(0f, clip.length);
        source.Play();
        source.minDistance = 5;
        source.dopplerLevel = 0;
        return source;
    }

    private static float SoundLerp(float from, float to, float value)
    {
        return (1.0f - value) * from + value * to;
    }
}
