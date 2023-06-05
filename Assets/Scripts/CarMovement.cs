using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float Speed;

    [Serializable]
    public struct Stats
    {
        [Header("Movement Settings")] [Min(0.001f), Tooltip("Top speed attainable when moving forward.")]
        public float TopSpeed;

        [Tooltip("How quickly the kart reaches top speed.")]
        public float Acceleration;

        [Min(0.001f), Tooltip("Top speed attainable when moving backward.")]
        public float ReverseSpeed;

        [Tooltip("How quickly the kart reaches top speed, when moving backward.")]
        public float ReverseAcceleration;

        [Tooltip(
            "How quickly the kart starts accelerating from 0. A higher number means it accelerates faster sooner.")]
        [Range(0.2f, 1)]
        public float AccelerationCurve;

        [Tooltip("How quickly the kart slows down when the brake is applied.")]
        public float Braking;

        [Tooltip("How quickly the kart will reach a full stop when no inputs are made.")]
        public float CoastingDrag;

        [Range(0.0f, 1.0f)] [Tooltip("The amount of side-to-side friction.")]
        public float Grip;

        [Tooltip("How tightly the kart can turn left or right.")]
        public float Steer;

        [Tooltip("Additional gravity for when the kart is in the air.")]
        public float AddedGravity;

        // allow for stat adding for powerups.
        public static Stats operator +(Stats a, Stats b)
        {
            return new Stats
            {
                Acceleration = a.Acceleration + b.Acceleration,
                AccelerationCurve = a.AccelerationCurve + b.AccelerationCurve,
                Braking = a.Braking + b.Braking,
                CoastingDrag = a.CoastingDrag + b.CoastingDrag,
                AddedGravity = a.AddedGravity + b.AddedGravity,
                Grip = a.Grip + b.Grip,
                ReverseAcceleration = a.ReverseAcceleration + b.ReverseAcceleration,
                ReverseSpeed = a.ReverseSpeed + b.ReverseSpeed,
                TopSpeed = a.TopSpeed + b.TopSpeed,
                Steer = a.Steer + b.Steer,
            };
        }
    }

    public Stats CarStats;

    // Start is called before the first frame update
    void Start()
    {
        Speed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        Movements();
        transform.Translate(Speed * Time.deltaTime, 0f, 0f);
        Debug.Log(Speed);
    }

    public void Movements()
    {
        var VerticalAxis = Input.GetAxisRaw("Vertical");
        var HorizontalAxis = Input.GetAxisRaw("Horizontal");
        //Marcher Courir et Orientation

        if (Speed != 0f)
        {
            Speed += (Speed > 0 ? -Time.deltaTime : Time.deltaTime) * (Speed * Speed / 20f + Speed);
        }
        
        if (VerticalAxis != 0f) // si il fait W ou S
        {
            if (VerticalAxis > 0)
            {
                // if (Speed >= 0f)
                    Speed += 2f * Time.deltaTime * VerticalAxis;
            }
            else
            {
                // transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                // if (Speed >= 0f)
                    Speed += 2f * Time.deltaTime * VerticalAxis;
            }
        }

        if (HorizontalAxis != 0) // si il fait A ou D
        {
            if (HorizontalAxis > 0)
            {
                // transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y + HorizontalAxis * Speed * 50f, transform.rotation.z);
                transform.Rotate(0, HorizontalAxis * Speed * Time.timeScale * 0.05f, 0f);
            }
            else
            {
                // transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y + HorizontalAxis * Speed, transform.rotation.z);
                transform.Rotate(0, HorizontalAxis * Speed * Time.timeScale * 0.05f, 0f);
            }

        }

        // if (VerticalAxis != 0 && HorizontalAxis != 0) // si il fait une combinaison de 2 touches 
        // {
        //     if (VerticalAxis > 0)
        //     {
        //         if (HorizontalAxis > 0)
        //             transform.rotation = Quaternion.Euler(0f, 45f, 0f);
        //         else
        //             transform.rotation = Quaternion.Euler(0f, 315f, 0f);
        //     }
        //     else
        //     {
        //         if (HorizontalAxis > 0)
        //             transform.rotation = Quaternion.Euler(0f, 135f, 0f);
        //         else
        //             transform.rotation = Quaternion.Euler(0f, 225f, 0f);
        //     }
        //
        //     // transform.Translate(0, 0, speed);
        // }
        
        // if (VerticalAxis != 0f || HorizontalAxis != 0f)
        //     transform.Translate(Time.deltaTime, 0f, 0f);
    }
}
