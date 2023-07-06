using System.Collections.Generic;
using UnityEngine;

public class DamageSystemAI : MonoBehaviour
{
    #region VISUAL COLLISION

    private Collision hitPoint;
    private bool collisionSystemOn = false;

    #region COLLISION PARTICLES

    #endregion

    #endregion

    #region AUDIO COLLISION

    private AudioSource crashSound;
    private AudioClip collisionSound;
    private float collisionVolume;

    #endregion

    private AnyCarAI ACAI;

    void Start()
    {
        ACAI = this.transform.GetComponent<AnyCarAI>();

        collisionSystemOn = ACAI.collisionSystem;

        #region AUDIO

        collisionSound = ACAI.collisionSound;
        SetUpCollisionAudioSource(collisionSound);
        collisionVolume = ACAI.collisionVolume;

        #endregion

        if (collisionSystemOn)
        {
            #region VISUAL      

            if (ACAI.optionalMeshList.Length == 0 || !ACAI.customMesh)
            {
                ACAI.bodyMesh.AddComponent<MeshCollisionScriptAI>();

                ACAI.bodyMesh.GetComponent<MeshCollisionScriptAI>().maxCollisionStrength /= ACAI.demolutionStrenght;
                ACAI.bodyMesh.GetComponent<MeshCollisionScriptAI>().demolutionRange = ACAI.demolutionRange;

                ACAI.bodyMesh.GetComponent<MeshCollisionScriptAI>().meshFilter = ACAI.bodyMesh.GetComponent<MeshFilter>();

                ACAI.bodyMesh.GetComponent<MeshCollisionScriptAI>().collisionParticlesON = ACAI.collisionParticles;
            }
            else
            {
                if (ACAI.customMesh)
                {
                    foreach (var optional in ACAI.optionalMeshList)
                    {
                        optional.modelMesh.gameObject.AddComponent<MeshCollisionScriptAI>();

                        optional.modelMesh.gameObject.GetComponent<MeshCollisionScriptAI>().maxCollisionStrength /= ACAI.demolutionStrenght;
                        optional.modelMesh.gameObject.GetComponent<MeshCollisionScriptAI>().demolutionRange = ACAI.demolutionRange;

                        optional.modelMesh.gameObject.GetComponent<MeshCollisionScriptAI>().meshFilter = optional.modelMesh.gameObject.GetComponent<MeshFilter>();

                        optional.modelMesh.gameObject.GetComponent<MeshCollisionScriptAI>().collisionParticlesON = ACAI.collisionParticles;

                        optional.modelMesh.gameObject.GetComponent<MeshCollisionScriptAI>().loseAftCollisions = optional.loseAftCollisions;

                        if (optional.modelMesh.gameObject.GetComponent<MeshCollider>() == null)
                        {
                            optional.modelMesh.gameObject.AddComponent(typeof(MeshCollider));
                            optional.modelMesh.gameObject.GetComponent<MeshCollider>().convex = true;
                            optional.modelMesh.gameObject.GetComponent<MeshCollider>().isTrigger = true;
                        }
                    }
                }
            }

            #endregion
        }
    }

    
    public void OnCollisionEnter(Collision collision)
    {
        hitPoint = collision;

        if (collisionSystemOn)
        {
            if (ACAI.optionalMeshList.Length == 0 || !ACAI.customMesh)
            {
                ACAI.bodyMesh.GetComponent<MeshCollisionScriptAI>().hitPoint = hitPoint;
                ACAI.bodyMesh.GetComponent<MeshCollisionScriptAI>().collisionHappened = true;
            }
            else
            {
                if (ACAI.customMesh)
                {
                    foreach (var optional in ACAI.optionalMeshList)
                    {
                        optional.modelMesh.gameObject.GetComponent<MeshCollisionScriptAI>().hitPoint = hitPoint;
                    }
                }
            }
        }

        #region AUDIO

        crashSound.volume = hitPoint.relativeVelocity.magnitude / 100 * collisionVolume;
        crashSound.Play();

        #endregion
    }

    private AudioSource SetUpCollisionAudioSource(AudioClip clip)
    {
        crashSound = this.transform.gameObject.AddComponent<AudioSource>();

        crashSound.clip = clip;
        crashSound.loop = false;
        crashSound.pitch = 1f;

        crashSound.playOnAwake = false;
        crashSound.minDistance = 5;
        crashSound.reverbZoneMix = 1.5f;
        crashSound.maxDistance = 600;
        crashSound.dopplerLevel = 2;
        return crashSound;
    }
}
