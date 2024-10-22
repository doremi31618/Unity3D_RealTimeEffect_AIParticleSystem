﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleMouse : MonoBehaviour {
    public int EatingNumber;
    public float EatingTimeInterval = 0.5f;
    public bool isEating = false;
    public bool isBeEaten = false;
    [HideInInspector] public Collider MouseCollider;
    [HideInInspector] public float timer;

    public LayerManager[] HuntingTargets;
    //ParticleSystem m_particleEffect;
	// Use this for initialization
	void Start () {
        MouseCollider = GetComponent<SphereCollider>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void setHunigTarget(LayerManager[] _HuntingTargets)
    {
        HuntingTargets = _HuntingTargets;
    }
    public void Reset()
    {
        EatingNumber = 0;
        timer = 0;
        isEating = false;

    }
    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.layer);
        if (!isEating)
        {
            EatingEvent(other.gameObject);
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (!isEating)
        {
            EatingEvent(other.gameObject);
        }
    }

    public void BeEaten()
    {
        isBeEaten = true;
    }

    //call event 
    void EatingEvent(GameObject beEatenGameObject)
    {

        if(HuntingTargets == null)return;
        //Debug.Log(beEatenGameObject.layer);
        for(int i = 0;i<HuntingTargets.Length;i++)
        {
            if(beEatenGameObject.layer == (int)HuntingTargets[i])
            {
                
                switch(beEatenGameObject.layer)
                {
                    case 9:
                        beEatenGameObject.GetComponent<MainParticleLifeCycle>().BeEaten();
                        isEating = true;
                        break;

                    //player layer
                    case 10:
                        break;


                    //spring worm layer 
                    case 12:
                        beEatenGameObject.GetComponent<ParticleBase>().BeEaten();
                        isEating = true;
                        break;

                    case 13:
                        if(beEatenGameObject.GetComponent<SnakeHead>() != true)return;
                        beEatenGameObject.GetComponent<SnakeHead>().beEaten = true;
                        isEating = true;
                        break;

                    case 14:
                        if (beEatenGameObject.GetComponent<ParticleMouse>() == null || beEatenGameObject.layer == gameObject.layer) return;
                        if (!beEatenGameObject.GetComponent<ParticleMouse>().isBeEaten)
                            beEatenGameObject.GetComponent<ParticleMouse>().BeEaten();
                        else Debug.Log("Emitter has already be eaten");
                        break;

                    case 16:
                        
                        break;
                }
                break;
            }
        }
        

    }

}
