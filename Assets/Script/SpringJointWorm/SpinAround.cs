using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAround : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        StartCoroutine(spinAround());
	}
    public float frequency = 0.5f;
    public int direction = 1;
    public float minSpeed = 50;
    public float maxSpeed = 150;
    public bool isUsePhysicWayToRotate = false;
    IEnumerator spinAround()
    {
        while(true)
        {
            float angle = (minSpeed + (maxSpeed - minSpeed) *
                        Mathf.PerlinNoise(Time.deltaTime *
                                          Mathf.Sin(minSpeed + maxSpeed) *
                                          Mathf.Cos((maxSpeed - minSpeed)),
                                         1)) *
                Time.deltaTime * direction * Mathf.Sin((minSpeed + Time.time) * frequency);

            if(!isUsePhysicWayToRotate)
                transform.Rotate(0,0,angle);
            else
                GetComponent<Rigidbody>().angularVelocity += new Vector3(0, 0, angle);
            //Debug.Log(angle);
            yield return new WaitForEndOfFrame();
        }

    }
}
