using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ScreenSpaceBoundary))]
public class ParticleEmitter : MonoBehaviour {
    public int maxNumber = 100;
    public GameObject particle;
    //public MainParticleLifeCycleAttribbute allParticleAttribute = new MainParticleLifeCycleAttribbute();

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
        Emitter();
    }
    void Emitter()
    {
        
        for (int i = 0; i < maxNumber;i++)
        {
            GameObject clone = Instantiate(particle);
            clone.transform.parent = this.transform;
            particleList.Add(clone);
        }

    }
}
