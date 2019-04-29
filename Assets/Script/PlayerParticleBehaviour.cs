﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Boo.Lang;

[System.Serializable]
public class PlayerHunter : Hunter
{
    
}
public enum ForceType
{
    attractive,
    explosition
}
public class PlayerParticleBehaviour : ParticleBehaiour{
    private static PlayerParticleBehaviour _instance;
    public PlayerParticleBehaviour instance{
        get
        {
            if (_instance == null) _instance = this;
            return _instance;
        }
    }

    int index;
    float timer;

    /* 參考用
    public ApearenceStructure apearence;
    public PhysicMotionSetting physicMotion;
    public bool isUseRandomReborn;
    
    //public Hunter hunter;
    
    [Header("State  setting")]
    public bool isHunter = false;
    public ParticleLifeState stateNow;
    public ParticleMotionState motionStateNow;
    
    [Header("Growing Up attribute")]
    public bool canItGrowUp = false;
    public int HowMuchNumberForEatingToGrowUp;
    [HideInInspector] public bool beEaten = false;

    //int index;
    [HideInInspector]public ScreenSpaceBoundary m_Boundary;
    [HideInInspector]public Rigidbody m_rigidbody;
    [HideInInspector]public SphereCollider m_collider;
    */
    public PlayerHunter m_hunter;

    [Header("Force effect Radius")]
    [Range(0.5f, 10f)] public float attractiveRadius = 7.5f;
    [Range(0.5f, 10f)] public float collisionRadius = 2f;
    [Range(0.5f, 10f)] public float explosionRadius = 7.5f;

    [Header("Force")]
    public float RepulsiveForce = 100f;
    public float AttrativeForce = 10f;

    [Header("Time Setting")]
    public float explosionTime = 0.3f;
    public float attractiveTime = 10f;

    [Header("Move motion setting")]
    public Vector2 stepRandomMoveDistance = new Vector2(1, 2);
    public Vector2 maxMoveAngle = new Vector2(60, 120);

    [Header("Interactive target")]
    [Tooltip("the particle who will affected by player particle")]
    public LayerManager[] InteractTarget;
    public LayerManager[] HuntingList; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Inititalize()
    {
        m_collider = this.GetComponent<SphereCollider>();
        m_rigidbody = this.GetComponent<Rigidbody>();


    }

    public override void LifeCycleStateSelector()
    {
        timer += Time.deltaTime;

        switch (stateNow)
        {
            case ParticleLifeState.Start:
                break;
            case ParticleLifeState.Update:
                UpdateCycleStateSelector();
                break;
            case ParticleLifeState.End:
                break;
        }
    }
    public override void UpdateCycleStateSelector()
    {
        switch (motionStateNow)
        {
            case ParticleMotionState.Idle:
                break;
            case ParticleMotionState.Hunting:
                break;
            case ParticleMotionState.Eating:
                break;
            case ParticleMotionState.interactive:
                break;
            case ParticleMotionState.BeEaten:
                break;
        }
    }

    public void InteractiveStateSelector(GameObject collisionObject)
    {
        if (!checkIfInTargetList(InteractTarget, collisionObject))
            return;


        if(stateNow == ParticleLifeState.Update)
        {
            switch (motionStateNow)
            {
                case ParticleMotionState.FirstUpdate:
                    ForceSelector(collisionObject,ForceType.explosition);
                    break;
                case ParticleMotionState.Idle:
                    ForceSelector(collisionObject, ForceType.attractive);
                    break;
                case ParticleMotionState.Hunting:
                    break;
                case ParticleMotionState.Eating:
                    break;
                case ParticleMotionState.interactive:
                    break;
                case ParticleMotionState.BeEaten:
                    break;
                case ParticleMotionState.EndOfUpdate:
                    break;
            }  
        }

    }
    /// <summary>
    /// direct add force to input game object
    /// </summary>
    /// <param name="_collisionObject">.</param>
    /// <param name="type">Type.</param>
    public void ForceSelector(GameObject _collisionObject,ForceType type)
    {
        Vector3 Force = Vector3.zero;
        Rigidbody pRigidbody = _collisionObject.GetComponent<Rigidbody>();
        float distance = Vector3.Distance(transform.position, _collisionObject.transform.position);
        switch(type)
        {
            case ForceType.attractive:
                Force = (this.transform.position - _collisionObject.transform.position).normalized *
                    Mathf.Lerp(AttrativeForce, AttrativeForce / 2, distance / attractiveRadius);
                break;

            case ForceType.explosition:
                Force = (_collisionObject.transform.position - this.transform.position).normalized *
                    Mathf.Lerp(0, RepulsiveForce, distance / explosionRadius);
                break;
        }

        pRigidbody.AddForce(Force);
    }

    public bool checkIfInTargetList(LayerManager[] list, GameObject checkThisObject)
    {
        if(list.Length == 0)
        {
            Debug.Log("the list dont have any target");
            return false;
        }

        for (int i = 0; i < list.Length;i++)
        {
            if(checkThisObject.layer == (int)list[i])
            {
                return true;
            }
        }
        return false;
    }
}