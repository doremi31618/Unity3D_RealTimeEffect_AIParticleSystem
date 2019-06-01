using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trshcan : MonoBehaviour
{
    MainParticleLifeCycle[] recycleObjectList;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        recycleObjectList = transform.GetComponentsInChildren<MainParticleLifeCycle>();
        foreach(MainParticleLifeCycle particle in recycleObjectList)
        {
            if(particle.m_stage == mainParticleStage.end)
            {
                
                Destroy(particle.transform.gameObject);
            }
        }
    }
}
