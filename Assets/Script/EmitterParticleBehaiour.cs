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
        emitterParticle = _emitterParticle;
        //if(HuntingSensor == null)
        //{
        //    HuntingSensor = new GameObject();
        //    HuntingSensor.transform.parent = emitterParticle.transform;
        //    HuntingSensor.AddComponent<SphereCollider>();
        //}
    }

    public float TracingSpeed;
    [HideInInspector]public EmitterParticleBehaiour emitterParticle;
    int eatingNumber;

    GameObject currentTarget;
    public GameObject getCurrentTarget{ get { return currentTarget; } }

    public int getEatingNumber{get{return eatingNumber;}}
    public bool isTracing{get{ return (currentTarget == null);}}

    public void Hunting()
    {
        if (!ifHasTarget)
        {
            SelectTracingTarget();
        }
        //TracingTarget();
    }

    public void SelectTracingTarget(Transform m_transform)
    {
        if (HuntingList.Count > 0)
        {
            float mostCloseDistance = 0;
            GameObject mostCloseTarget = null;
            for (int i = 0; i < HuntingList.Count; i++)
            {
                float distanceToTarget = Vector3.Distance(m_transform.position, HuntingList[i].transform.position);
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
        else if (HuntingList.Count == 0)
        {
            currentTarget = null;
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

    private static EmitterParticleBehaiour _instance;
    public EmitterParticleBehaiour instance{
        get{
            if (_instance == null) _instance = new EmitterParticleBehaiour();
            _instance = this;
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


	// Use this for initialization
	void Start () {
        Initialize();
        StartCoroutine(StateController());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator StateController()
    {
        while(true){
            LifeCycleStateSelector();
            yield return new WaitForFixedUpdate();
        }

    }

    public override void Initialize()
    {
        InitializeComponent();
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
        stateNow = ParticleLifeState.Update;

        direction = UnityEngine.Random.Range(0, 100) > 50 ? 1 : -1;
        transform.localPosition = RegenerateStartPosition(direction);
        PositionInitialize();
    }

    void UpdateBehaviour()
    {
        UpdateCycleStateSelector();
    }
    void EndBehaviour()
    {
        
        stateNow = ParticleLifeState.RebornDelay;
    }

    Vector3 nextRandomPos;
    public override void UpdateCycleStateSelector() 
    {
        //Debug.Log(m_updateStage);
        EventManagement();
        switch (motionStateNow)
        {
            case ParticleMotionState.Idle:
                //水皿移動
                GerridaeMoveWay();
                break;
            case ParticleMotionState.Hunting:
                //Hunter();
                break;
            case ParticleMotionState.Eating:
                //GrowUp();
                motionStateNow = ParticleMotionState.Idle;
                break;
            case ParticleMotionState.BeEaten:
                stateNow = ParticleLifeState.End;
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

    public override void EventManagement()
    {
        if(!m_hunter.ifHasTarget && !m_hunter.isTracing && !m_hunter.getIsEating && !beEaten)
        {
            motionStateNow = ParticleMotionState.Idle;

        }else if(m_hunter.ifHasTarget && !m_hunter.getIsEating)
        {
            motionStateNow = ParticleMotionState.Hunting;
        }
        else if(m_hunter.getIsEating)
        {
            motionStateNow = ParticleMotionState.Eating;
        }
        else if (beEaten)
        {
            motionStateNow = ParticleMotionState.BeEaten;
        }
    }

    /// <summary>
    /// 水皿移動
    /// </summary>
    void GerridaeMoveWay()
    {
        //Debug.Log("transform.position : " + transform.position + " ; nextPosition : " + nextPosition + "Vector3.Distance(transform.position, nextPosition) : " + Vector3.Distance(transform.position, nextPosition));
        if(nextPosition == Vector3.zero || Vector3.Distance(transform.position, nextPosition) < 1.5f)
        {
            GeneratNextPosion();
        }

        LerpMove(nextPosition, this.transform.position, moveDistance);

    }

    float moveDistance;
    void GeneratNextPosion()
    {
        
        float mediumAngle = direction == 1 ? (maxMoveAngle.x + maxMoveAngle.y) * Mathf.Deg2Rad * 0.5f : (maxMoveAngle.x + maxMoveAngle.y ) * Mathf.Deg2Rad * 0.5f + 180f* Mathf.Deg2Rad;
        float moveAngle = UnityEngine.Random.Range(maxMoveAngle.x * Mathf.Deg2Rad - mediumAngle, maxMoveAngle.y * Mathf.Deg2Rad - mediumAngle);
        moveDistance = UnityEngine.Random.Range(stepRandomMoveDistance.x, stepRandomMoveDistance.y);

        float newX = (float)(moveDistance * Math.Cos(moveAngle));
        float newY = (float)(moveDistance * Math.Sin(moveAngle));

        //if(m_Boundary.isPointInside())
        nextPosition = this.transform.position + new Vector3(newX, newY,0);

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

}
