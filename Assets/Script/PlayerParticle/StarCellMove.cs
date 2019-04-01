using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class StarCellMove : ParticleBase {



    [HideInInspector] public int index;
    [Range(0, 1000)] public float localMoveSpeed = 100;

    private void OnEnable()
    {
        
    }
    // Use this for initialization
    void Start () {
        Initialized();
	}
	
	// Update is called once per frame
	void Update () {
        Run();
        OutputValue();
	}

    void Run()
    {
        direction += new Vector3(
            Mathf.Sin(Mathf.Rad2Deg * Random.Range(-Mathf.PI, Mathf.PI)),
            Mathf.Cos(Mathf.Rad2Deg * Random.Range(-Mathf.PI, Mathf.PI)), 0);
        direction = direction.normalized;
        direction.Set(direction.x, direction.y, 0);
        velocity = Mathf.PerlinNoise(Time.time + index, index) * direction * localMoveSpeed * Time.deltaTime;
    }
    Rigidbody m_rigidbody;
    void OutputValue()
    {

        m_rigidbody.velocity = velocity;
        m_rigidbody.angularVelocity = angVelocity;

    }
    public void AttributeInitialze()
    {
        velocity = Vector3.zero;
        angVelocity = Vector3.zero;
        index = Random.Range(0, 823231);
    }
    public void Initialized()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        
    }
}
