using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum LayerManager
{
    Object = 9,
    Player = 10,
    SpringJointWorm = 12,
    Snake = 13,
}
public class SnakeHead : MonoBehaviour {

    //public LayerManager HuntingTarget;
    [HideInInspector]public bool beEaten =  false;
    public LayerManager[] HuntingTargets;
    public List<GameObject> HuntingList = new List<GameObject>();
    public GameObject snakeMouse;
    public bool getIfEating
    {
        get{
            return snakeMouse.GetComponent<SnakeMouse>().isEating;
        }
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        
        for (int i = 0; i < HuntingTargets.Length;i++)
        {
            //Debug.Log(other.gameObject.layer + " " + (int)HuntingTargets[i]);
            if(other.gameObject.layer == (int)HuntingTargets[i])
            {
                HuntingList.Add(other.gameObject);
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
