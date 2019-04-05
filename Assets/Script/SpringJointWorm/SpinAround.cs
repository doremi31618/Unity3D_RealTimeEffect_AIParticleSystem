using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAround : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        StartCoroutine(spinAround());
	}
    public float speed = 10f;
    IEnumerator spinAround()
    {
        while(true)
        {
            transform.Rotate(0,0,((speed * Mathf.PerlinNoise(Time.time ,Time.time)) *  Time.deltaTime));
            yield return new WaitForEndOfFrame();
        }

    }
}
