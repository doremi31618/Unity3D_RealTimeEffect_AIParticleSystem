﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarCellMove : ParticleBase {

    [HideInInspector] public int index;
    [Range(0, 1000)] public float spinAroundSpeed = 100;
    [Range(0, 1000)] public float localMoveSpeed = 100;
    //public bool isUseSphereBoundary = false;
    public float distance;

    // Use this for initialization
    void Start () {
        Initialized();
        StartCoroutine(spinAround());
	}
	
	// Update is called once per frame
	void Update () {
        Run();
        OutputValue();
	}
    void RandomRotation()
    {
        
    }
    public float speed = 10f;
    IEnumerator spinAround()
    {
        while (true)
        {
            transform.Rotate(0, 0, ((spinAroundSpeed * Mathf.PerlinNoise(Time.time, Time.time)) * Time.deltaTime));
            Run();
            yield return new WaitForEndOfFrame();
        }

    }

    void Run()
    {
        direction += new Vector3(
            Mathf.Sin(Mathf.Rad2Deg * Random.Range(-Mathf.PI, Mathf.PI)),
            Mathf.Cos(Mathf.Rad2Deg * Random.Range(-Mathf.PI, Mathf.PI)), 0);
        direction = direction.normalized;
        direction.Set(direction.x, direction.y, 0);
        velocity = Mathf.PerlinNoise(Time.time + index, index) * direction * localMoveSpeed * Time.deltaTime;
        //distance = transform.parent.localScale.x * transform.parent.GetComponent<SphereCollider>().radius;
        //if(Vector3.Distance(this.transform.localPosition,transform.parent.position) > distance * 0.5f && isUseSphereBoundary)
        //{
        //    velocity = -velocity;
        //}
    }

    Rigidbody m_rigidbody;
    void OutputValue()
    {

        m_rigidbody.velocity = velocity;
        m_rigidbody.angularVelocity = angVelocity;

    }
    public void AttributeInitialze()
    {
        velocity = Vector3.zero;
        angVelocity = Vector3.zero;
        index = Random.Range(0, 823231);
    }
    public void Initialized()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

}
