using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//using Boo.Lang;

[System.Serializable]
public class PlayerHunter : Hunter
{
}
public enum ForceType
{
    attractive,
    explosition
}
public class PlayerParticleBehaviour : ParticleBehaiour{
    private static PlayerParticleBehaviour _instance;
    public PlayerParticleBehaviour instance{
        get
        {
            if (_instance == null) _instance = this;
            return _instance;
        }
    }

    int index;
    int eatNumberCounter;
    float timer;
    float timerAtIdle;
    float timerAtEnd;
    float timerAtFirst;

    /* 參考用
    public ApearenceStructure apearence;
    public PhysicMotionSetting physicMotion;
    public bool isUseRandomReborn;
    
    //public Hunter hunter;
    
    [Header("State  setting")]
    public bool isHunter = false;
    public ParticleLifeState stateNow;
    public ParticleMotionState motionStateNow;
    
    [Header("Growing Up attribute")]
    public bool canItGrowUp = false;
    public int HowMuchNumberForEatingToGrowUp;
    [HideInInspector] public bool beEaten = false;

    //int index;
    [HideInInspector]public ScreenSpaceBoundary m_Boundary;
    [HideInInspector]public Rigidbody m_rigidbody;
    [HideInInspector]public SphereCollider m_collider;
    */
    public PlayerHunter m_hunter;

    [Header("Force effect Radius")]
    [Range(0.5f, 10f)] public float attractiveRadius = 7.5f;
    [Range(0.5f, 10f)] public float collisionRadius = 2f;
    [Range(0.5f, 10f)] public float explosionRadius = 7.5f;

    [Header("Force")]
    public float RepulsiveForce = 100f;
    public float AttrativeForce = 10f;

    [Header("Time Setting")]
    public float lifeTime = 20f;
    public float explosionTime = 0.3f;
    public float attractiveTime = 10f;

    [Header("Move motion setting")]
    public Vector2 stepRandomMoveDistance = new Vector2(1, 2);
    public Vector2 maxMoveAngle = new Vector2(60, 120);

    [Header("Interactive target")]
    [Tooltip("the particle who will affected by player particle")]
    public LayerManager[] InteractTarget;
    public LayerManager[] HuntingTarget;
    public bool isUseMouseToControl = true;
    public int MaxCorner = 18;
    public float stage1Scale = 2.8f;
    public float stage2Scale = 5;
    

    [Header("Audio Sound Effect")]
    public AudioClip EatingSnakeOST;
    public AudioClip EatingLittleParticleOST;
    public AudioClip EatingEmitterParticleOST;
    public AudioClip EatingWaveOST;

    public AudioSource m_audioPlayer;

    private GameObject stage2;

	// Use this for initialization
	void Start () {
		stage2 = GameObject.Find("Stage2");
	}
	
	// Update is called once per frame
	void Update () {
        if(stage2 == null)
        { 
            stage2 = GameObject.Find("Stage2");
            return;
        }
		if(stage2.activeSelf && transform.localScale.x != stage2Scale)
        {
            transform.localScale = Vector3.one * stage2Scale;
        }
        else if(!stage2.activeSelf && transform.localScale.x != stage1Scale)
        {
            transform.localScale = Vector3.one * stage1Scale;
        }
	}

    private void FixedUpdate()
    {
        LifeCycleStateSelector();
    }
    public void Inititalize()
    {
        m_Boundary = transform.parent.GetComponent<ScreenSpaceBoundary>();
        m_collider = this.GetComponent<SphereCollider>();
        m_rigidbody = this.GetComponent<Rigidbody>();

        m_hunter.HuntingTargets = HuntingTarget;
        m_hunter.mouse.HuntingTargets = HuntingTarget;
        timer = 0;
        stateNow = ParticleLifeState.Update;
    }

    public override void LifeCycleStateSelector()
    {
        timer += Time.deltaTime;
        switch (stateNow)
        {
            case ParticleLifeState.Start:
                Inititalize();
                if (isUseMouseToControl) MouseControlMove();
                else { IdleMove(); }
                stateNow = ParticleLifeState.Update;
                break;
            case ParticleLifeState.Update:
                UpdateCycleStateSelector();
                break;
            case ParticleLifeState.End:
                stateNow = ParticleLifeState.Start;
                break;
        }
    }

    public override void UpdateCycleStateSelector()
    {
        EventManager();
        switch (motionStateNow)
        {
            case ParticleMotionState.FirstUpdate:
                FirstUpdateEventHandler();
                break;
            case ParticleMotionState.Idle:
                IdldeEventHandler();
                if (isUseMouseToControl) MouseControlMove();
                break;
            case ParticleMotionState.Hunting:
                break;
            case ParticleMotionState.Eating:
                m_hunter.mouse.isEating = false;
                break;
            case ParticleMotionState.interactive:
                if (isUseMouseToControl) MouseControlMove();
                break;
            case ParticleMotionState.BeEaten:
                break;
            case ParticleMotionState.EndOfUpdate:
                EndOfUpdateEventHandler();
                break;
        }
    }

    public bool interaciveStateTrigger = false; 
    void EventManager()
    {
        
        if(timerAtFirst / explosionTime < 1)
        {
            motionStateNow = ParticleMotionState.FirstUpdate;
        }
        else if(timerAtIdle/lifeTime < 1)
        {
            if(interaciveStateTrigger)
            {
                motionStateNow = ParticleMotionState.interactive;
            }
            else if (m_hunter.getIsEating)
            {
                motionStateNow = ParticleMotionState.Eating;
            }
            else if(m_hunter.ifHasTarget)
            {
                motionStateNow = ParticleMotionState.Hunting;
            }
            else if(beEaten)
            {
                motionStateNow = ParticleMotionState.BeEaten;
            }
            else
            {
                motionStateNow = ParticleMotionState.Idle;
            }
        }
        else
        {
            motionStateNow = ParticleMotionState.EndOfUpdate;
        }
    }
    void FirstUpdateEventHandler()
    {
        timerAtFirst += Time.deltaTime;
    }
    void IdldeEventHandler()
    {
        timerAtIdle += Time.deltaTime;
    }
    void EndOfUpdateEventHandler()
    {
        timerAtEnd += Time.deltaTime;
        if (timerAtEnd / explosionTime > 1)
        {
            stateNow = ParticleLifeState.End;
            timerAtEnd = 0;
            timerAtIdle = 0;
            timerAtFirst = 0;
        }
            
    }
    private void OnTriggerStay(Collider other)
    {
        InteractiveStateSelector(other.gameObject);
    }
    void OnTriggerEnter(Collider other)
    {
        EatingEvent(other.gameObject);
    }
    public void InteractiveStateSelector(GameObject collisionObject)
    {
        if (!checkIfInTargetList(InteractTarget, collisionObject))
            return;


        if(stateNow == ParticleLifeState.Update)
        {
            switch (motionStateNow)
            {
                case ParticleMotionState.FirstUpdate:
                    ForceSelector(collisionObject,ForceType.explosition);
                    break;
                case ParticleMotionState.Idle:
                    ForceSelector(collisionObject, ForceType.attractive);
                    break;

                case ParticleMotionState.Hunting:
                    break;

                case ParticleMotionState.Eating:
                    Grow();
                    break;

                case ParticleMotionState.interactive:
                    if(collisionObject.GetComponent<Rigidbody>() != null)
                        ForceSelector(collisionObject, ForceType.explosition);
                    if(m_hunter.getIsEating)Grow();
                    break;

                case ParticleMotionState.BeEaten:
                    break;

                case ParticleMotionState.EndOfUpdate:
                    ForceSelector(collisionObject, ForceType.explosition);
                    break;
            }  
        }

    }
     void EatingEvent(GameObject beEatenGameObject)
    {
        switch (beEatenGameObject.layer)
        {
            //object
            case 9:

                break;
            //snake
            case 13:
                if(beEatenGameObject.GetComponent<SnakeHead>() == null)return;
                m_audioPlayer.clip = EatingSnakeOST;
                m_audioPlayer.Play();
                break;
            //emitter partcle
            case 14:
                m_audioPlayer.clip = EatingEmitterParticleOST;
                m_audioPlayer.Play();
                break;

            //wave
            case 16:
                m_audioPlayer.clip = EatingWaveOST;
                m_audioPlayer.Play();
                break;
        }
        

    }
    /// <summary>
    /// direct add force to input game object
    /// </summary>
    /// <param name="_collisionObject">.</param>
    /// <param name="type">Type.</param>
    public void ForceSelector(GameObject _collisionObject,ForceType type)
    {
        Vector3 Force = Vector3.zero;
        Rigidbody pRigidbody = _collisionObject.GetComponent<Rigidbody>();
        float distance = Vector3.Distance(transform.position, _collisionObject.transform.position);
        switch(type)
        {
            case ForceType.attractive:
                Force = (this.transform.position - _collisionObject.transform.position).normalized *
                    Mathf.Lerp(AttrativeForce, AttrativeForce / 2, distance / attractiveRadius);
                break;

            case ForceType.explosition:
                Force = (_collisionObject.transform.position - this.transform.position).normalized *
                    Mathf.Lerp(0, RepulsiveForce, distance / explosionRadius);
                break;
        }

        pRigidbody.AddForce(Force);
    }
    int direction = 1;
    void IdleMove()
    {
        direction = Random.Range(0, 100) > 50 ? 1 : -1;
        float mediumAngle = direction == 1 ? (maxMoveAngle.x + maxMoveAngle.y) * Mathf.Deg2Rad * 0.5f : (maxMoveAngle.x + maxMoveAngle.y) * Mathf.Deg2Rad * 0.5f + 180f * Mathf.Deg2Rad;
        float moveAngle = Random.Range(maxMoveAngle.x * Mathf.Deg2Rad - mediumAngle, maxMoveAngle.y * Mathf.Deg2Rad - mediumAngle);
        float moveDistance = Random.Range(stepRandomMoveDistance.x, stepRandomMoveDistance.y);
        float newX = (float)(moveDistance * Mathf.Cos(moveAngle));
        float newY = (float)(moveDistance * Mathf.Sin(moveAngle));
        m_rigidbody.MovePosition(transform.position + new Vector3(newX, newY, 0).normalized * Time.deltaTime * physicMotion.speed);
    }
    public void MouseControlMove()
    {
        //Vector3 point = new Vector3();
        Event currentEvent = Event.current;
        //Vector2 mousePos = new Vector2();

        Vector3 mousePointToPlayerLayer = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x,
                        Input.mousePosition.y,
                        m_Boundary.distanceToCamera));
        m_rigidbody.MovePosition(mousePointToPlayerLayer);
    }

    public bool checkIfInTargetList(LayerManager[] list, GameObject checkThisObject)
    {
        if(list.Length == 0)
        {
            Debug.Log("the list dont have any target");
            return false;
        }

        for (int i = 0; i < list.Length;i++)
        {
            if(checkThisObject.layer == (int)list[i])
            {
                return true;
            }
        }
        return false;
    }

    private void OnMouseDown()
    {
        m_hunter.mouse.MouseCollider.isTrigger = true;
        interaciveStateTrigger = true;
    }
    private void OnMouseUp()
    {
        m_hunter.mouse.MouseCollider.isTrigger = false;
        interaciveStateTrigger = false;
    }
    void Grow()
    {
        eatNumberCounter += 1;
        m_hunter.mouse.Reset();
        if (eatNumberCounter % HowMuchNumberForEatingToGrowUp == 0 && canItGrowUp)
        {
            Star m_shape = GetComponent<Star>();
            if (m_shape.frequency < MaxCorner)
            {
                m_shape.frequency++;
                m_shape.Reset();
                eatNumberCounter = 0;
            }
        }
    }

}
