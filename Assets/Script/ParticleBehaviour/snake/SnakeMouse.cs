using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SnakeMouse : MonoBehaviour {
    public int EatingNumber;
    public float EatingTimeInterval = 0.5f;
    public bool isEating = false;
    [HideInInspector]public float timer;
    ParticleSystem m_particleEffect;
    public LayerManager[] HuntingTargets;

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
            //Debug.Log("eating");
            EatingEvent(other.gameObject);
        }
    }

    //call event 
    void EatingEvent(GameObject beEatenGameObject)
    {
        if(HuntingTargets == null)return;
        for(int i = 0;i<HuntingTargets.Length;i++)
        {
            if(beEatenGameObject.layer == (int)HuntingTargets[i])
            {
                //Debug.Log(beEatenGameObject.layer);
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
                        // if(beEatenGameObject.GetComponent<SnakeHead>() != true)return;
                        // beEatenGameObject.GetComponent<SnakeHead>().beEaten = true;
                        // isEating = true;
                        break;

                    case 14:
                        if (beEatenGameObject.GetComponent<ParticleMouse>() == null || beEatenGameObject.layer == gameObject.layer) return;
                        if (!beEatenGameObject.GetComponent<ParticleMouse>().isBeEaten)
                            beEatenGameObject.GetComponent<ParticleMouse>().BeEaten();
                        else Debug.Log("Emitter has already be eaten");
                        isEating = true;
                        break;

                    case 16:
                        
                        //Destroy(beEatenGameObject);
                        break;

                }
                break;
            }
        }

    }

}
