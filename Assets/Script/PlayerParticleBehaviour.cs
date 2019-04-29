using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerParticleBehaviour : ParticleBehaiour{
    private static PlayerParticleBehaviour _instance;
    public PlayerParticleBehaviour instance{
        get
        {
            if (_instance == null) _instance = this;
            return _instance;
        }
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
