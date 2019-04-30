using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleMouse : MonoBehaviour {
    public int EatingNumber;
    public float EatingTimeInterval = 0.5f;
    public bool isEating = false;
    public bool isBeEaten = false;
    public Collider MouseCollider;
    [HideInInspector] public float timer;
    //ParticleSystem m_particleEffect;
	// Use this for initialization
	void Start () {
        MouseCollider = GetComponent<SphereCollider>();
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
    private void OnTriggerEnter(Collider other)
    {
        if (!isEating)
        {
            //Debug.Log("eating");

            EatingEvent(other.gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!isEating)
        {
            //Debug.Log("eating");

            EatingEvent(collision.gameObject);
        }
    }
    public void BeEaten()
    {
        Debug.Log(" Emitter : Be eaten");
        isBeEaten = true;
    }

    //call event 
    void EatingEvent(GameObject beEatenGameObject)
    {
        switch(beEatenGameObject.layer)
        {
            case 9:
                beEatenGameObject.GetComponent<MainParticleLifeCycle>().BeEaten();
                isEating = true;
                //m_particleEffect.Play();
                break;

            //player layer
            case 10:
                //isEating = true;
                break;


            //spring worm layer 
            case 12:
                beEatenGameObject.GetComponent<ParticleBase>().BeEaten();
                isEating = true;

                break;

            case 13:
                //isEating = true;
                break;

            case 14:
                if (beEatenGameObject.GetComponent<ParticleMouse>() == null || beEatenGameObject.layer == gameObject.layer) return;
                if (!beEatenGameObject.GetComponent<ParticleMouse>().isBeEaten)
                    beEatenGameObject.GetComponent<ParticleMouse>().BeEaten();
                else Debug.Log("Emitter has already be eaten");
                break;
        }

    }

}
