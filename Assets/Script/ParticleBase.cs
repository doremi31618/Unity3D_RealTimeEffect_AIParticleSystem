using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelOfParticle
{
    level1 = 1,
    level2,
    level3,
    level4
}
[RequireComponent(typeof(Rigidbody))]
public class ParticleBase : MonoBehaviour{
    [HideInInspector]public float mass;
    public float lifeTime;
    public LevelOfParticle level = LevelOfParticle.level1; 

    [HideInInspector]public Vector3 direction;
    //[HideInInspector]public Vector3 position;
    [HideInInspector]public Vector3 velocity;
    [Range(0, 100)] public float maxMoveSpeed = 100;

    [HideInInspector]public Vector3 angVelocity;
    //public float lerpTime = 1f;
    private void Update()
    {
        if(GetComponent<Rigidbody>().velocity.magnitude > maxMoveSpeed)
        {
            GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * maxMoveSpeed;
        }
    }

}
