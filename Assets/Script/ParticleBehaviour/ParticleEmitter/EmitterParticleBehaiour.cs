using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class EmitterHunter : Hunter
{
    /*
     * public LayerManager m_Layer;
     * public LayerManager[] HuntingTargets;
     * public GameObject HuntingSensor;
     * [HideInInspector] public bool ifHasTarget;
     * [HideInInspector] public List<GameObject> HuntingList;
     */
     
    public EmitterHunter(EmitterParticleBehaiour _emitterParticle)
    {
        if(_emitterParticle != null)
            emitterParticle = _emitterParticle;

        
        
    }

    public float TracingSpeed;
    [HideInInspector]public EmitterParticleBehaiour emitterParticle;
    int eatingNumber;

    public GameObject currentTarget = null;
    public GameObject getCurrentTarget{ get { return currentTarget; } }

    public int getEatingNumber{get{return eatingNumber;}}
    public bool isTracing{get{ return (currentTarget != null);}}

    public void Hunting()
    {
        //Debug.Log("enter hunting");
        if (!isTracing && ifHasTarget)
        {
            SelectTracingTarget(emitterParticle.transform);
            //Debug.Log("select target");
            return;
        }
        else if (!ifHasTarget)
        {
            
            //Debug.Log("currentTarget = null");
            return;
        }

       
        //Debug.Log("ifHasTarget : " + ifHasTarget + " ; isTracing : " + isTracing);
        //TracingTarget();
    }

    public void SelectTracingTarget(Transform m_transform)
    {
       
        if (HuntingList.Count > 0)
        {
            //Debug.Log("select hunting target");
            float mostCloseDistance = 0;
            GameObject mostCloseTarget = null;
            if(mouse.HuntingTargets.Length == 0){
                mouse.setHunigTarget(HuntingTargets);
                Debug.Log("Hunting Targets" + mouse.HuntingTargets[0]);
            }
            for (int i = 0; i < HuntingList.Count; i++)
            {
                if(HuntingList[i] == null)continue;
                float distanceToTarget = Vector3.Distance(m_transform.position, HuntingList[i].transform.position);
                if(distanceToTarget > 10)
                {
                    HuntingList.Remove(HuntingList[i]);
                    return;
                }
                if (mostCloseTarget == null)
                {
                    currentTarget = HuntingList[i];
                    mostCloseDistance = Vector3.Distance(m_transform.position, currentTarget.transform.position);
                }
                else if (distanceToTarget < mostCloseDistance)
                {
                    currentTarget = HuntingList[i];
                    mostCloseDistance = distanceToTarget;
                }
            }

        }
        else if(HuntingList.Count == 0)
        {
            currentTarget = null;
            //Debug.Log("hunting List is empty");
        }

    }

    //public void TracingTarget(Transform m_transform)
    //{
    //    float sensorRadius = HuntingSensor.GetComponent<SphereCollider>().radius * HuntingSensor.transform.localScale.x;
    //    float distanceToTarget = Vector3.Distance(m_transform.position, currentTarget.transform.position);

    //    TracingMove(m_transform);

    //    if(distanceToTarget < sensorRadius)
    //    {
    //        EatingBehaviour();
    //    }
    //}

    //void TracingMove(Transform m_transform)
    //{
    //    float _distance = Vector3.Distance(currentTarget.transform.position, m_transform.position);
    //    Vector3 dir = (currentTarget.transform.position - m_transform.transform.position).normalized;
    //    float t = Time.deltaTime * _distance / 2 * TracingSpeed * 0.35f;
    //    if (t > 0.5) t = 0.5f;
    //    m_transform.GetComponent<Rigidbody>().MovePosition(
    //        Vector3.Slerp(m_transform.position, currentTarget.transform.position, t));
    //}
    //void EatingBehaviour()
    //{
    //    eatingNumber += 1;
    //    if(eatingNumber > emitterParticle.HowMuchNumberForEatingToGrowUp)
    //    {
    //        emitterParticle.GrowUp();
    //        eatingNumber = 0;
    //    }
    //    emitterParticle.motionStateNow = ParticleMotionState.Eating;
    //}
}

[RequireComponent(typeof(Rigidbody),typeof(SphereCollider))]
public class EmitterParticleBehaiour : ParticleBehaiour{
    /* 參考用
    //public ApearenceStructure apearence;
    //public PhysicMotionSetting physicMotion;
    //public Hunter hunter;
    //public ParticleLifeState stateNow;
    //public ParticleMotionState motionStateNow;
    //public int HowMuchNumberForEatingToGrowUp;
    //public bool canItGrowUp = false;
    //public bool isHunter = false;
    //public bool ifNeedRandomReborn;
    //int index;

    //ScreenSpaceBoundary m_Boundary;
    //Rigidbody m_rigidbody;
    //SphereCollider m_collider;
    */

    private static EmitterParticleBehaiour _instance ;
    public EmitterParticleBehaiour instance{
        get{
            if (_instance == null) _instance = this;
            return _instance;
        }
    }


    int index;
    int direction = 1;
    float timer;
    float timerAtEnd;
    float rebornDelayTime;

    public EmitterHunter m_hunter = new EmitterHunter(_instance);

    [Header("Motion Related Attribbute")]
    public Vector2 rebornDelay = new Vector2(2, 5);
    [Tooltip("How long will it reborn after leaving boundary")]
    public float endingDelay = 1.5f;

    [Tooltip("(min,max)")]
    public Vector2 stepRandomMoveDistance = new Vector2(1,2);
    public Vector2 maxMoveAngle = new Vector2(60,120);
    private Vector3 nextPosition;

    [Header("ParticleEffect")]
    public ParticleSystem TraceEffect;
    public ParticleSystem ExplosionEffect;
    
	// Use this for initialization
    void Awake()
    {
        if(instance == null){
            Debug.Log("initialize");
        }
    }
	void Start () {
        saveData();
        Initialize();


	}
	
	// Update is called once per frame
	void Update () 
    {
        LifeCycleStateSelector();
	}

    public override void Initialize()
    {
        InitializeComponent();
        m_hunter.emitterParticle = instance;
        if(isUseRandomReborn)
        {
            RandomReborn();
        }


    }

    public override void RandomReborn()
    {
        direction = UnityEngine.Random.Range(0, 100) > 50 ? 1 : -1;
        transform.position = RegenerateStartPosition(direction);
        PositionInitialize();
    }

    public override void LifeCycleStateSelector()
    {
        timer += Time.deltaTime;
        //switch
        switch (stateNow)
        {
            case ParticleLifeState.RebornDelay:
                RebornDelayBehaviour();
                break;

            case ParticleLifeState.Start:
                StartBehaviour();
                break;

            case ParticleLifeState.Update:
                UpdateBehaviour();
                break;

            case ParticleLifeState.End:
                EndBehaviour();
                break;
        }
    }

    void RebornDelayBehaviour()
    {
        if (timer > rebornDelayTime)
        {

            stateNow = ParticleLifeState.Start;

        }
    }

    void StartBehaviour()
    {
        timer = 0;
        m_hunter.mouse.isBeEaten = false;
        stateNow = ParticleLifeState.Update;

        direction = UnityEngine.Random.Range(0, 100) > 50 ? 1 : -1;
        transform.localPosition = RegenerateStartPosition(direction);
        PositionInitialize();
        m_hunter.HuntingList.Clear();
        m_hunter.currentTarget = null;
        transform.localScale = scale;
        ExplosionEffect.Stop();
        TraceEffect.Play();
        GetComponent<MeshRenderer>().material.SetFloat("_Alpha", 1);
    }

    void UpdateBehaviour()
    {
        UpdateCycleStateSelector();
    }
    void EndBehaviour()
    {
        StartCoroutine(fadeOut(0.5f));

    }

    Vector3 nextRandomPos;
    public override void UpdateCycleStateSelector() 
    {
        //Debug.Log(m_updateStage);
        EventManagement();
        switch (motionStateNow)
        {
            case ParticleMotionState.Idle:
                GerridaeMoveWay();
                break;
            case ParticleMotionState.Hunting:
                GerridaeMoveWay();
                //Hunter();
                break;
            case ParticleMotionState.Eating:
                if (!isgrowing)
                    StartCoroutine(growing(1f));
                motionStateNow = ParticleMotionState.Idle;

                break;
            case ParticleMotionState.BeEaten:
                StartCoroutine(scaleDown(ExplosionEffect.main.duration, ExplosionEffect));
                break;
        }

       
        //if particle leave boundary it will translate to end state
        //but here I do is for avoiding particle stuck on the boundary line
        if(!m_Boundary.isPointInside(transform) && timer > 5)
        {
            timerAtEnd += Time.deltaTime;
            //Debug.Log("isPointInside : " + m_Boundary.isPointInside(transform) + "; timer : " + timer + " ; timerAtEnd : " + timerAtEnd);

            if (timerAtEnd > endingDelay)
            {
                timer = 0;
                timerAtEnd = 0;
                nextPosition = Vector3.zero;
                stateNow = ParticleLifeState.End;
                //Debug.Log("enter end state");
            }
        }
    }


    public void EventManagement()
    {

        if(m_hunter.mouse.isBeEaten)
        {
            motionStateNow = ParticleMotionState.BeEaten;
        }
        else if(m_hunter.ifHasTarget && !m_hunter.getIsEating)
        {
            motionStateNow = ParticleMotionState.Hunting;

        }
        else if (m_hunter.getIsEating)
        {
            motionStateNow = ParticleMotionState.Eating;

            m_hunter.mouse.Reset();
        }
        else 
        {
            motionStateNow = ParticleMotionState.Idle;
        }


    }
    void HuntingTracingMove()
    {
        LerpMove(nextPosition, this.transform.position, moveDistance);
    }

    /// <summary>
    /// 水皿移動
    /// </summary>
    void GerridaeMoveWay()
    {
        float currentDistance = Vector3.Distance(transform.position, nextPosition);
        //Debug.Log("transform.position : " + transform.position + " ; nextPosition : " + nextPosition + "Vector3.Distance(transform.position, nextPosition) : " + Vector3.Distance(transform.position, nextPosition));
        if (currentDistance < 1.5f || currentDistance > 10f)
        {
            if(motionStateNow == ParticleMotionState.Idle)
                GeneratNextPosion();
            else if(motionStateNow == ParticleMotionState.Hunting)
            {
                //Debug.Log("GerridaeMoveWay hunting");
                m_hunter.Hunting();

                if(m_hunter.isTracing)
                {
                    if (Vector3.Distance(transform.position, m_hunter.currentTarget.transform.position) > 15)
                    {
                        m_hunter.currentTarget = null;
                        motionStateNow = ParticleMotionState.Idle;
                        //Debug.Log("tracing 2");
                        return;
                    }
                    nextPosition = this.transform.position + (m_hunter.getCurrentTarget.transform.position - transform.position).normalized * 5;
                    //Debug.Log("tracing 1");
                    //Debug.Log("reset next position in hunting : current distance " + currentDistance);
                }
                else
                {
                    motionStateNow = ParticleMotionState.Idle;
                    //Debug.Log("return to idle");
                    return;

                }
                //Debug.Log("nothing happen");
                    
            }
                
        }
        else if(currentDistance < 10f && currentDistance > 1.5f)
            LerpMove(nextPosition, this.transform.position, moveDistance);
        
        //LerpMove(nextPosition, this.transform.position, moveDistance);
    }

    float moveDistance;
    void GeneratNextPosion()
    {
        
        float mediumAngle = direction == 1 ? (maxMoveAngle.x + maxMoveAngle.y) * Mathf.Deg2Rad * 0.5f : (maxMoveAngle.x + maxMoveAngle.y ) * Mathf.Deg2Rad * 0.5f + 180f* Mathf.Deg2Rad;
        float moveAngle = UnityEngine.Random.Range(maxMoveAngle.x * Mathf.Deg2Rad - mediumAngle, maxMoveAngle.y * Mathf.Deg2Rad - mediumAngle);
        moveDistance = UnityEngine.Random.Range(stepRandomMoveDistance.x, stepRandomMoveDistance.y);

        float newX = (float)(moveDistance * Mathf.Cos(moveAngle));
        float newY = (float)(moveDistance * Mathf.Sin(moveAngle));

        //if(m_Boundary.isPointInside())
        nextPosition = this.transform.position + new Vector3(newX, newY,0);
        //Debug.Log("reset next position in idle");
    }

    void LerpMove(Vector3 forwardPosition, Vector3 currentPosition, float minCloseDistance)
    {
        float _distance = Vector3.Distance(forwardPosition, currentPosition);
        Vector3 dir = (forwardPosition - currentPosition).normalized;
        float t = Time.deltaTime * _distance / minCloseDistance * physicMotion.speed;
        if (t > 0.5) t = 0.5f;
            m_rigidbody.MovePosition(
            Vector3.Slerp(currentPosition, forwardPosition, t));
    }


    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < m_hunter.HuntingTargets.Length;i++)
        {
            if(other.gameObject.layer == (int)m_hunter.HuntingTargets[i])
            {
                if(!m_hunter.HuntingList.Contains(other.gameObject))
                    m_hunter.HuntingList.Add(other.gameObject);
                ForceSelector(100, other.gameObject, ForceType.attractive);
            }
        }

    }
    private void OnTriggerStay(Collider other)
    {
        for (int i = 0; i < m_hunter.HuntingTargets.Length; i++)
        {
            if (other.gameObject.layer == (int)m_hunter.HuntingTargets[i])
            {
                if (!m_hunter.HuntingList.Contains(other.gameObject))
                    m_hunter.HuntingList.Add(other.gameObject);
                ForceSelector(100, other.gameObject, ForceType.attractive);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(m_hunter.HuntingList.Contains(other.gameObject))
        {
            m_hunter.HuntingList.Remove(other.gameObject);

        }
    }

    public void ForceSelector(float forceMagnitude,GameObject _collisionObject, ForceType type)
    {
        Vector3 Force = Vector3.zero;
        Rigidbody pRigidbody = _collisionObject.GetComponent<Rigidbody>();
        float distance = Vector3.Distance(transform.position, _collisionObject.transform.position);
        switch (type)
        {
            case ForceType.attractive:
                Force = (this.transform.position - _collisionObject.transform.position).normalized *
                    Mathf.Lerp(forceMagnitude, forceMagnitude / 2, distance / GetComponent<SphereCollider>().radius * transform.localScale.x);
                break;

            case ForceType.explosition:
                Force = (_collisionObject.transform.position - this.transform.position).normalized *
                    Mathf.Lerp(0, forceMagnitude, distance / GetComponent<SphereCollider>().radius * transform.localScale.x);
                break;
        }

        pRigidbody.AddForce(Force);
    }
    Vector3 scale;
    void saveData()
    {
        scale = transform.localScale;
    }

    bool checkLossyScale(Vector3 _scale)
    {
        return _scale.x * _scale.y * _scale.z > 0;
    }


    IEnumerator scaleDown(float lerpTime,ParticleSystem explosionEffect)
    {
        float time = 0;
        explosionEffect.Play();
        while(checkLossyScale(transform.localScale))
        {
            time += Time.deltaTime;
            transform.localScale = Vector3.Slerp(scale, Vector3.zero, time / lerpTime);
            yield return new WaitForEndOfFrame();
        }
        stateNow = ParticleLifeState.End;
        TraceEffect.Stop();
    }

    IEnumerator fadeOut(float lerpTime)
    {
        float alpha = GetComponent<MeshRenderer>().material.GetFloat("_Alpha");
        float time = 0;
        TraceEffect.Stop();
        while(GetComponent<MeshRenderer>().material.GetFloat("_Alpha") > 0)
        {
            time += Time.deltaTime;
            float _a = Mathf.Lerp(alpha, 0, time / lerpTime);
            GetComponent<MeshRenderer>().material.SetFloat("_Alpha", _a);
            yield return new WaitForEndOfFrame();
        }
        stateNow = ParticleLifeState.RebornDelay;

    }
    bool isgrowing = false;
    IEnumerator growing(float _timer)
    {
        float time = 0;
        isgrowing = true;
        TraceEffect.emissionRate = 120;
        while(time/_timer < 1)
        {
            time += Time.deltaTime;
            transform.localScale = Vector3.Slerp(scale, scale + Vector3.one*0.2f , time / _timer);
            yield return new WaitForEndOfFrame();
        }

        TraceEffect.emissionRate = 8;
        time = 0;
        while (time / _timer < 1)
        {
            time += Time.deltaTime;
            transform.localScale = Vector3.Slerp(scale +Vector3.one * 0.2f,scale, time / _timer);
            yield return new WaitForEndOfFrame();
        }
        transform.localScale = scale;

        isgrowing = false;
    }
   
}
