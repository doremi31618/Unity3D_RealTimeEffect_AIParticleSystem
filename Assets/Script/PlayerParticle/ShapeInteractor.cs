using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractiveSystem),typeof(Star))]
public class ShapeInteractor : MonoBehaviour {
    int EatingToGrowUp;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void ChangingStarFrequency()
    {
        int substractNumber;
        int starFrequency = GetComponent<Star>().frequency;



    }
}
