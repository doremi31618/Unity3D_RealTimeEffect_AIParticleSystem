using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMouse : MonoBehaviour {
    public int EatingNumber;
    public float EatingTimeInterval = 0.5f;
    public bool isEating = false;
    float timer;
	// Use this for initialization
	void Start () {
		
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

    private void OnCollisionEnter(Collision collision)
    {
        if(!isEating)
        {
            EatingEvent(collision.gameObject);
        }
    }

    //call event 
    void EatingEvent(GameObject beEatenGameObject)
    {
        beEatenGameObject.SetActive(false);
    }

}
