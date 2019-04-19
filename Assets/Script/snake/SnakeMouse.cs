﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMouse : MonoBehaviour {
    public int EatingNumber;
    public float EatingTimeInterval = 0.5f;
    public bool isEating = false;
    float timer;
    ParticleSystem m_particleEffect;
	// Use this for initialization
	void Start () {
        m_particleEffect = this.GetComponent<ParticleSystem>();
        m_particleEffect.Pause();
	}

    private void FixedUpdate()
    {
        if(isEating)
        {
            timer += Time.deltaTime;
            if(timer > EatingTimeInterval)
            {
                isEating = false;
            }

        }
    }

    public void Reset()
    {
        EatingNumber = 0;
        timer = 0;
        isEating = false;

    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isEating)
        {
            Debug.Log("eating");
            EatingEvent(other.gameObject);
        }
    }

    //call event 
    void EatingEvent(GameObject beEatenGameObject)
    {
        switch(beEatenGameObject.layer)
        {
            case 9:
                beEatenGameObject.GetComponent<MainParticleLifeCycle>().BeEaten();
                m_particleEffect.Play();
                break;

            //player layer
            case 10:
                break;

            //spring worm layer 
            case 12:
                break;

            case 13:
                break;
        }

    }

}