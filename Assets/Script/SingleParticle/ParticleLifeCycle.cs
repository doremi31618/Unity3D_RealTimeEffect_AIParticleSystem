using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Schema;


public class ParticleLifeCycle : ParticleBase
{
    [HideInInspector] public int index;

    [Range(0, 10)] public float rebornDelayTimeRange;
    float rebornDelayTime;

    [Range(0, 1000)] public float localMoveSpeed = 100;

    public Color m_color;


    ScreenSpaceBoundary m_Boundary;
    Rigidbody m_rigidbody;
    Renderer m_renderer;

    #region time comtrol
    public float lerpTime = 1f;
    #endregion
    public mainParticleStage m_stage = mainParticleStage.rebornDelay;
    public enum mainParticleStage
    {
        rebornDelay,
        start,
        update,
        end
    }

    private void Awake()
    {
        

    }
    private void Start()
    {
        Initialized();
        AttributeInitialze();
        StartCoroutine(StageControl());
    }
    private void Update()
    {
        
    }
    private void FixedUpdate()
    {
        OutputValue();
    }

    void Run()
    {
        
        direction += new Vector3(
            Mathf.Sin(Mathf.Rad2Deg * Random.Range(-Mathf.PI, Mathf.PI) * Mathf.PerlinNoise(Time.time + index, index)),
            Mathf.Cos(Mathf.Rad2Deg * Random.Range(-Mathf.PI, Mathf.PI) * Mathf.PerlinNoise(Time.time + index, index)), 0);
        
        direction = direction.normalized;
        direction.Set(direction.x, direction.y, 0);

        velocity = Mathf.PerlinNoise( Time.time + index, index ) * direction * localMoveSpeed * Time.deltaTime;

    }
    void OutputValue()
    {

        //m_rigidbody.velocity = velocity;
        m_rigidbody.MovePosition(transform.position + velocity);
        //m_rigidbody.AddForce(velocity);
        m_rigidbody.angularVelocity = angVelocity;

    }
    public void Initialized()
    {
        rebornDelayTime = Random.Range(0, rebornDelayTimeRange);
        m_Boundary = transform.parent.GetComponent<ScreenSpaceBoundary>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_renderer = GetComponent<Renderer>();
    }
    public void AttributeInitialze()
    {

        RebornPositionSetting();
        velocity = Vector3.zero;
        angVelocity = Vector3.zero;
        index = Random.Range(0, 823231);
        rebornDelayTime = Random.Range(0, rebornDelayTimeRange);

    }
    void RebornPositionSetting()
    {
        transform.localPosition = new Vector3(
            Random.Range(m_Boundary.minX, m_Boundary.maxX),
            Random.Range(m_Boundary.minY, m_Boundary.maxY), 0);
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
        m_stage = mainParticleStage.rebornDelay;
    }

    public void StayBoundary()
    {
        Run();
    }

    IEnumerator StageControl()
    {
        while (true)
        {
            
            float timer = 0;
            Color nowColor;
            Color lerpColor = new Color(m_color.r, m_color.g, m_color.b, 0);

            switch(m_stage)
            {
                case mainParticleStage.rebornDelay:
                    while (timer / rebornDelayTime < 1)
                    {
                        timer += Time.deltaTime;
                        velocity = Vector3.zero;
                        angVelocity = Vector3.zero;
                        m_renderer.material.SetColor("_TintColor", lerpColor);
                        yield return new WaitForEndOfFrame();
                    }
                    m_stage = mainParticleStage.start;
                    break;
                case mainParticleStage.start:
                    AttributeInitialze();
                    while (timer / lerpTime < 1)
                    {
                        timer += Time.deltaTime;
                        nowColor = Color.Lerp(lerpColor, m_color, timer / lerpTime);
                        m_renderer.material.SetColor("_TintColor", nowColor);
                        CollisionBoundary();

                        //Debug.Log("fade in");
                        yield return new WaitForEndOfFrame();
                    }
                    m_stage = mainParticleStage.update;
                    break;

                case mainParticleStage.update:
                    while (timer / lifeTime < 1)
                    {
                        timer += Time.deltaTime;
                        CollisionBoundary();
                        yield return new WaitForEndOfFrame();
                    }
                    m_stage = mainParticleStage.end;
                    break;

                case mainParticleStage.end:
                    
                    while (timer / lerpTime < 1)
                    {
                        timer += Time.deltaTime;
                        nowColor = Color.Lerp(m_color, lerpColor, timer / lerpTime);
                        m_renderer.material.SetColor("_TintColor", nowColor);
                        CollisionBoundary();
                        yield return new WaitForEndOfFrame();
                    }
                    m_stage = mainParticleStage.rebornDelay;
                    break;

            }

        }


    }

  

}
