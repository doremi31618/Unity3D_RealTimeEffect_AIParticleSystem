using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ParticleBase : MonoBehaviour{
    [HideInInspector]public float mass;
    public float lifeTime;

    [HideInInspector]public Vector3 direction;
    //[HideInInspector]public Vector3 position;
    [HideInInspector]public Vector3 velocity;
    [HideInInspector]public Vector3 angVelocity;



}
