using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Schema;
using System.Runtime.CompilerServices;
public enum mainParticleStage
{
    rebornDelay,
    start,
    update,
    beEaten,
    end
}

 
public class MainParticleLifeCycle : ParticleBase
{
    [HideInInspector] public int index;

    [Range(0, 10)] public float rebornDelayTimeRange = 5f;
    float rebornDelayTime = 5f;

    [Range(0, 100)] public float localMoveSpeed = 2;

    //private Color m_color;

    public Color fisrtColor;
    public Color finalColor;

    ScreenSpaceBoundary m_Boundary;
    Rigidbody m_rigidbody;
    Renderer m_renderer;

    #region time comtrol
    public float lerpTime = 1f;
    #endregion
    public mainParticleStage m_stage = mainParticleStage.rebornDelay;
    [HideInInspector]public MainParticleMove m_move = new MainParticleMove();
    public BoundaryEvent m_event;
    //ParticleSystem m_particleEffect;
    private void Start()
    {
        Initialized();
        RebornPositionSetting();
        StartCoroutine(StageControl());
    }
    private void Update()
    {
        if (GetComponent<Rigidbody>().velocity.magnitude > maxMoveSpeed)
        {
            GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * maxMoveSpeed;
        }
    }

    private void FixedUpdate()
    {
        OutputValue();
    }
    public new void BeEaten()
    {
        //Debug.Log("number" + index + "be eaten");
        //m_particleEffect.Play();
        m_stage = mainParticleStage.beEaten;
        GetComponent<MeshRenderer>().enabled = false;
    }
    public void Run()
    {
        
        direction += new Vector3(
            Mathf.Sin(Mathf.Rad2Deg * Random.Range(-Mathf.PI, Mathf.PI) * Mathf.PerlinNoise(Time.time + index, index)),
            Mathf.Cos(Mathf.Rad2Deg * Random.Range(-Mathf.PI, Mathf.PI) * Mathf.PerlinNoise(Time.time + index, index)), 0);
        
        direction = direction.normalized;
        direction.Set(direction.x, direction.y, 0);
        //Debug.Log("Run");
        velocity = Mathf.PerlinNoise( Time.time + index, index ) * direction * localMoveSpeed * Time.deltaTime;

    }

    void OutputValue()
    {
        if(m_move.index % 2 != 0)
        {
            m_rigidbody.AddForce(m_move.velocity * 150);
        }   
           
        else{
            m_rigidbody.MovePosition(transform.position + m_move.velocity);
            //Debug.Log("Move position"+m_move.index);
        }
            
        //m_rigidbody.AddForce(velocity);
        m_rigidbody.angularVelocity = m_move.angVelocity;
        //Debug.Log("m_move" + m_move.velocity);

    }
    public void Initialized()
    {
        rebornDelayTime = Random.Range(0, rebornDelayTimeRange);
        m_Boundary = transform.parent.GetComponent<ScreenSpaceBoundary>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_renderer = GetComponent<Renderer>();
        m_event = new MainParticleBoundaryEvent(GetComponent<MainParticleLifeCycle>());
        m_move = new MainParticleMove();


        //m_color = fisrtColor ;
        m_renderer.material.SetColor("_TintColor", new Color(fisrtColor.a,fisrtColor.g,fisrtColor.b,0));
    }
    public void AttributeInitialze()
    {

        RebornPositionSetting();
        m_move.velocity = Vector3.zero;
        m_move.angVelocity = Vector3.zero;
        m_move.index = Random.Range(12345, 823231);
        m_move.localMoveSpeed = localMoveSpeed;
        rebornDelayTime = Random.Range(0, rebornDelayTimeRange);

    }
    void RebornPositionSetting()
    {
        transform.localPosition = new Vector3(
            Random.Range(m_Boundary.minX, m_Boundary.maxX),
            Random.Range(m_Boundary.minY, m_Boundary.maxY), 0);
    }
    /*
    //public void CollisionBoundary()
    //{

    //    bool checkPositionX = (transform.position.x < m_Boundary.maxX && transform.position.x > m_Boundary.minX);
    //    bool checkPositionY = (transform.position.y < m_Boundary.maxY && transform.position.y > m_Boundary.minY);

    //    if (checkPositionX && checkPositionY)
    //    {
    //        StayBoundary();
    //    }
    //    else
    //    {
    //        ExitBoundary();
    //    }
    //}



    //public void StayBoundary()
    //{
    //    Run();
    //}
    */
    public void ExitBoundary()
    {
        //AttributeInitialze();
        //m_renderer.material.SetColor("_TintColor", new Color(fisrtColor.r, fisrtColor.g, fisrtColor.b, 0));
        m_stage = mainParticleStage.end;
    }
    IEnumerator StageControl()
    {
        while (true)
        {
            
            float timer = 0;
            Color nowColor;
            Color lerpColor = new Color(fisrtColor.r, fisrtColor.g, fisrtColor.b, 0);
            Color m_color = m_renderer.material.GetColor("_TintColor");
            switch(m_stage)
            {
                case mainParticleStage.rebornDelay:
                    //velocity = Vector3.zero;
                    angVelocity = Vector3.zero;
                    //GetComponent<Rigidbody>().
                    GetComponent<MeshRenderer>().enabled = true;
                    GetComponent<Collider>().enabled = false;
                    m_renderer.material.SetColor("_TintColor", lerpColor);
                    while (timer / rebornDelayTime < 1)
                    {
                        timer += Time.deltaTime;

                        if(m_stage != mainParticleStage.rebornDelay)
                        {
                            break;
                        }
                        yield return new WaitForEndOfFrame();
                    }
                    m_stage = mainParticleStage.start;
                    break;

                case mainParticleStage.start:
                    AttributeInitialze();
                    GetComponent<Collider>().enabled = true;
                    GetComponent<Rigidbody>().isKinematic = false;
                    //m_rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                    while (timer / lerpTime < 1)
                    {
                        timer += Time.deltaTime;
                        nowColor = Color.Lerp(m_color, fisrtColor, timer / lerpTime);
                        m_renderer.material.SetColor("_TintColor", nowColor);
                        //CollisionBoundary();
                        m_Boundary.CollisionBoundary(transform,ref m_event);
                        if (m_stage != mainParticleStage.start)
                        {
                            break;
                        }

                        yield return new WaitForEndOfFrame();
                    }
                    m_stage = mainParticleStage.update;
                    break;

                case mainParticleStage.update:
                    while (timer / lifeTime < 1)
                    {
                        timer += Time.deltaTime;
                        m_Boundary.CollisionBoundary(transform, ref m_event);
                        if (m_stage != mainParticleStage.update)
                        {
                            break;
                        }
                        yield return new WaitForEndOfFrame();
                    }
                    m_stage = mainParticleStage.end;
                    break;
                case mainParticleStage.beEaten:
                    m_stage = mainParticleStage.rebornDelay;
                    break;
                case mainParticleStage.end:
                    
                    while (timer / lerpTime < 1)
                    {
                        timer += Time.deltaTime;
                        nowColor = Color.Lerp(m_color, lerpColor, timer / lerpTime);
                        m_renderer.material.SetColor("_TintColor", nowColor);
                        //CollisionBoundary();
                        m_Boundary.CollisionBoundary(transform, ref m_event);
                        if(m_Boundary.isPointInside(transform))
                        {
                            m_move.Run();
                        }
                        if (m_stage != mainParticleStage.end)
                        {
                            
                            break;

                        }
                        yield return new WaitForEndOfFrame();
                    }
                    m_stage = mainParticleStage.rebornDelay;
                    break;

            }

        }


    }



}

public class MainParticleBoundaryEvent : BoundaryEvent
{
    public MainParticleLifeCycle _particle;
    public MainParticleBoundaryEvent(MainParticleLifeCycle particle)
    {
        _particle = particle;
        //Debug.Log(_particle);
    }
    public override void StayBoundary()
    {
        _particle.m_move.Run();
        //Debug.Log("Stay boundary");

    }
    public override void ExitBoundary()
    {
        _particle.m_stage = mainParticleStage.end;
        _particle.velocity = Vector3.zero;

        //Debug.Log("Exit boundary");
    }
}

public class MainParticleMove : ParticleBase
{
    public int index;
    public float localMoveSpeed = 100;

    public void Run()
    {
        direction += new Vector3(
            Mathf.Sin(Mathf.Rad2Deg * Random.Range(-Mathf.PI, Mathf.PI) * Mathf.PerlinNoise(Time.time + index, index)),
            Mathf.Cos(Mathf.Rad2Deg * Random.Range(-Mathf.PI, Mathf.PI) * Mathf.PerlinNoise(Time.time + index, index)), 0);

        direction = direction.normalized;
        direction.Set(direction.x, direction.y, 0);

        velocity = Mathf.PerlinNoise(Time.time + index, index) * direction * localMoveSpeed * Time.deltaTime;

    }
}