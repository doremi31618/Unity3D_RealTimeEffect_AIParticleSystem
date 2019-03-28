using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class ParticleLifeCycle  : ParticleBase {
    [HideInInspector]public int index;

    [Range(0,10)]public float rebornDelayTimeRange;
    float rebornDelayTime;

    [Range(0, 1000)]public float localMoveSpeed = 250;
    public float maxVal;

    ScreenSpaceBoundary m_Boundary;
    Rigidbody m_rigidbody;

    private void Start()
    {
        Initialized();
        AttributeInitialze();
    }

    // Update is called once per frame
    void Update () {


        Run();
        OutputValue();
        CollisionBoundary();

	}

    void Run()
    {
        
        lifeTime -= Time.deltaTime;
        direction += new Vector3(
            Mathf.Sin(Mathf.Rad2Deg * Random.Range(-Mathf.PI,Mathf.PI)),
            Mathf.Cos(Mathf.Rad2Deg * Random.Range(-Mathf.PI, Mathf.PI)), 0);
        direction = direction.normalized;
        direction.Set(direction.x, direction.y, 0);
        velocity = Mathf.PerlinNoise(Time.time + index, index) * direction * localMoveSpeed * Time.deltaTime;



    }
    void OutputValue()
    {
        
        m_rigidbody.velocity = velocity;
        m_rigidbody.angularVelocity = angVelocity;

    }
    public void Initialized()
    {
        
        m_Boundary = transform.parent.GetComponent<ScreenSpaceBoundary>();
        m_rigidbody = GetComponent<Rigidbody>();


    }
    public void AttributeInitialze()
    {
        transform.localPosition = new Vector3(
            Random.Range(m_Boundary.minX, m_Boundary.maxX),
            Random.Range(m_Boundary.minY, m_Boundary.maxY), 0);
        
        velocity = Vector3.zero;
        angVelocity = Vector3.zero;
        index = Random.Range(0, 823231);
    }

    public void CollisionBoundary()
    {

        bool checkPositionX = (transform.position.x < m_Boundary.maxX && transform.position.x > m_Boundary.minX);
        bool checkPositionY = (transform.position.y < m_Boundary.maxY && transform.position.y > m_Boundary.minY);

        if (checkPositionX && checkPositionY)
        {
            StayBoundary();
        }
        else
        {
            ExitBoundary();
        }



    }

    public void ExitBoundary()
    {
        AttributeInitialze();
        OutputValue();
    }

    public void StayBoundary()
    {
        Run();
    }
    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(0.1f);
    }
    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(0.1f);
    }
}
