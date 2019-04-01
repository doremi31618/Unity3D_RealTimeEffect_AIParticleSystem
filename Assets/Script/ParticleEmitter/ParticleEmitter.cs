using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ScreenSpaceBoundary))]
public class ParticleEmitter : MonoBehaviour {
    public int maxNumber = 100;
    public GameObject particle;

    [HideInInspector]public List<GameObject> particleList;
    private void Initialized()
    {
        if(particleList.Count > 0)
        {
            particleList.Clear();
        }

    }
    private void Awake()
    {
        Initialized();
    }
    // Use this for initialization
    void Start () {
        
        Emitter();
		
	}
	
	// Update is called once per frame
	void Update () {
		
    }
    void Emitter()
    {
        
        for (int i = 0; i < maxNumber;i++)
        {
            GameObject clone = Instantiate(particle) as GameObject;
            clone.transform.parent = this.transform;
            particleList.Add(clone);
        }

    }
}
