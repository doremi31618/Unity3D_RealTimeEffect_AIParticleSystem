using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleMouse : MonoBehaviour {
    public int EatingNumber;
    public float EatingTimeInterval = 0.5f;
    public bool isEating = false;
    [HideInInspector] public float timer;
    //ParticleSystem m_particleEffect;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Reset()
    {
        EatingNumber = 0;
        timer = 0;
        isEating = false;

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!isEating)
        {
            //Debug.Log("eating");
            EatingEvent(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isEating)
        {
            //Debug.Log("eating");
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
                //m_particleEffect.Play();
                break;

            //player layer
            case 10:
                break;

            //spring worm layer 
            case 12:
                beEatenGameObject.GetComponent<ParticleBase>().BeEaten();
                break;

            case 13:
                break;
        }

    }

}
