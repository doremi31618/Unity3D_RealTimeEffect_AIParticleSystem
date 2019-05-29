using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class SnakeHead : MonoBehaviour {

    //public LayerManager HuntingTarget;
    public bool beEaten =  false;
    public LayerManager[] HuntingTargets;
    public List<GameObject> HuntingList = new List<GameObject>();
    public GameObject snakeMouse;
    public bool getIfEating
    {
        get{
            return snakeMouse.GetComponent<SnakeMouse>().isEating;
        }
    }
    [HideInInspector]public GameObject _currentTarget;

	// Use this for initialization
	void Start () {
		snakeMouse.GetComponent<SnakeMouse>().HuntingTargets = HuntingTargets;
	}
	
	// Update is called once per frame
	void Update () {
		
    
    }

    private void FixedUpdate()
    {
        checkEveryTarget();
    }
    void checkEveryTarget()
    {
        if (HuntingList.Count == 0) return;
        for (int i = 0; i < HuntingList.Count;i++)
        {
            if(HuntingList[i] == null)
            {
                HuntingList.Remove(HuntingList[i]);
                break;
            }
            switch (HuntingList[i].layer)
            {
                //object layer
                case 9:
                    if (HuntingList[i].GetComponent<MainParticleLifeCycle>().m_stage != mainParticleStage.update)
                    {
                        
                        if(_currentTarget == HuntingList[i].gameObject)
                        {
                            _currentTarget = null;
                        }
                        HuntingList.Remove(HuntingList[i].gameObject);
                    }
                    break;

                //player layer
                case 10:
                    break;

                //spring worm layer 
                case 12:
                    break;

                case 13:
                    break;

                case 14:
                    if(Vector3.Distance(transform.position,HuntingList[i].transform.position)>10)
                    {
                        if (_currentTarget == HuntingList[i].gameObject)
                        {
                            _currentTarget = null;
                        }
                        HuntingList.Remove(HuntingList[i].gameObject);
                    }
                    else if(HuntingList[i].GetComponent<ParticleMouse>().isBeEaten)
                    {
                        HuntingList.Remove(HuntingList[i].gameObject);
                    }
                    break;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        
        for (int i = 0; i < HuntingTargets.Length;i++)
        {
            //Debug.Log(other.gameObject.layer + " " + (int)HuntingTargets[i]);
            if(other.gameObject.layer == (int)HuntingTargets[i])
            {
                switch(other.gameObject.layer)
                {
                    //object layer
                    case 9:
                        if (other.gameObject.GetComponent<MainParticleLifeCycle>() == false) return;
                        if(other.gameObject.GetComponent<MainParticleLifeCycle>().m_stage == mainParticleStage.update)
                        {
                            HuntingList.Add(other.gameObject);
                        }
                        break;

                    //player layer
                    case 10:
                        break;

                    //spring worm layer 
                    case 12:
                        break;

                    case 13:
                        break;

                    case 14:
                        if (other.GetComponent<ParticleMouse>() == null) return;
                        if (!other.GetComponent<ParticleMouse>().isBeEaten)
                            HuntingList.Add(other.gameObject);
                        else Debug.Log("Emitter has already be eaten");
                        break;

                    case 16:
                        HuntingList.Add(other.gameObject);
                        break;
                }

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if (other.gameObject.layer == huntingTarget)
           
        for (int i = 0; i < HuntingTargets.Length; i++)
        {
            if (other.gameObject.layer == (int)HuntingTargets[i])
            {
                HuntingList.Remove(other.gameObject);
            }
        }
    }
}
