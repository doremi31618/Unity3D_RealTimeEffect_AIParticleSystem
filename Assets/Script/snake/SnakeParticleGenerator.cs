using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Security.Cryptography;
using System;
using UnityEngine.Experimental.UIElements;
/// <summary>
/// tip : need change the project setting collision let same tag object wont happen collision
/// </summary>
//[System.Serializable]
//public struct Snake
//{
    
//    public GameObject headPrefab;
//    public GameObject bodyPrefab;

//    public Color headColor;
//    public Color bodyColor;

//    public List<GameObject> bodyList;
//    public bool[] bodyStayBoundaryCheckList;
//}
public enum SnakeLifeStage
{
    RebornDelay,
    Start,
    Update,
    End
}
public enum SnakeMotionStage
{
    Idle,
    Hunting,
    Eating,
    BeEaten
}

public class SnakeParticleGenerator : MonoBehaviour {
    
    [Header("Prefabs Attribute setting")]
    public GameObject headPrefab;
    public GameObject bodyPrefab;

    [Header("Apearence Attribbute")]
    public Color headColor;
    public Color bodyColor;

    [Tooltip("only affect at fisrt time(start)")]public int numberOfBody = 5;
    [Tooltip("change by realtime")]public float bodyDistance;

    [Header("Motion Related Attribbute")]
    public Vector2 rebornDelay = new Vector2(2, 5);
    [Tooltip("How long will it reborn after leaving boundary")]
    public float endingDelay = 1.5f;
    [Tooltip("The moving speed")]
    public float speed = 1;
    public float maxMoveSpeed = 3.4f;

    [Header("Sensor Related Attribute")]
    [Tooltip("defalt value : o.5 so at least bigger than 0.5f")]
    public float SensorTriggerRadius = 1f;
    public SortingLayer HuntingTarget;
    public int HowMuchNumberForEatingToGrowUp = 5;
    [Tooltip("the max number that you can add to snake")]
    public int snakeAddingMaxNumber = 10;

    [HideInInspector] public bool ifAllStayInBoundary;
    [HideInInspector] public int direction = 1;

    int index;
    float timer;
    float timerAtEnd;
    float rebbornDelayTime;
    int eatNumber;

    //stage control
    public SnakeLifeStage m_stage = SnakeLifeStage.Update;
    public SnakeMotionStage m_updateStage = SnakeMotionStage.Idle;

    [HideInInspector]public List<GameObject> bodyList = new List<GameObject>();
    ScreenSpaceBoundary m_Boundary;
    SnakeHead m_head
    {
        get {
            return bodyList[0].GetComponent<SnakeHead>();
        }
    }

    //[Header("setting Boundary Attribute ")]
    //public bool isUseBoundarySystem;
    //public bool isUseRandomAttributeToReborn;
	// Use this for initialization
	void Start () {
        PositionInitialize();
        SnakeGenerator();
        AttributeIniate();
        //StartCoroutine(StageController());
	}
	
	// Update is called once per frame
	void Update () {
        LifeCycleStageSelector();
        //SnakeMove();
	}

    IEnumerator StageController()
    {
        while(true)
        {
            LifeCycleStageSelector();
            yield return new WaitForEndOfFrame();
        }
    }

    void LifeCycleStageSelector()
    {
        timer += Time.deltaTime;
        //switch
        switch(m_stage)
        {
            case SnakeLifeStage.RebornDelay:
                

                if (timer > rebbornDelayTime)
                {
                    
                    m_stage = SnakeLifeStage.Start;
                    UpdateCycleStageSelector();
                }
                break;

            case SnakeLifeStage.Start:
                timer = 0;
                SnakeInitializer();
                m_stage = SnakeLifeStage.Update;
                break;

            case SnakeLifeStage.Update:
                UpdateCycleStageSelector();
                break;

            case SnakeLifeStage.End:
                m_stage = SnakeLifeStage.RebornDelay;
                break;
        }
    }

    void UpdateCycleStageSelector()
    {

        //Debug.Log(m_updateStage);
        EventManagement();
        switch (m_updateStage)
        {
            case SnakeMotionStage.Idle:
                SnakeMove();
                break;
            case SnakeMotionStage.Hunting:
                Hunter();
                break;
            case SnakeMotionStage.Eating:
                //GrowUp();
                m_updateStage = SnakeMotionStage.Idle;
                break;
            case SnakeMotionStage.BeEaten:
                break;
        }
        if (timer > 5) CheckEachBodyIffStayInBoundary();
    }
    void GrowUp()
    {
        
        eatNumber += 1;


        //Debug.Log("HowMuchNumberForEatingToGrowUp" + HowMuchNumberForEatingToGrowUp + "||Eat Number" + eatNumber);
        if (eatNumber > HowMuchNumberForEatingToGrowUp)
        {
            if (snakeAddingMaxNumber == 0)
            {
                m_updateStage = SnakeMotionStage.Idle;
                return;
            }
            snakeAddingMaxNumber -= 1;
            eatNumber = 0;
            Debug.Log("grow");
            AddBody();
        }
        m_updateStage = SnakeMotionStage.Idle;
    }

    //define what stage should be 
    void EventManagement()
    {
        SnakeHead m_head = bodyList[0].GetComponent<SnakeHead>();
        if(m_head.HuntingList.Count > 1 && !m_head.beEaten)
        {
            m_updateStage = SnakeMotionStage.Hunting;
        }
        else if (m_head.HuntingList.Count == 0 && !m_head.beEaten)
        {
            m_updateStage = SnakeMotionStage.Idle;
        }
        else if (m_head.beEaten)
        {
            m_updateStage = SnakeMotionStage.BeEaten;
        }
        else if(m_head.getIfEating)
        {
            m_updateStage = SnakeMotionStage.Eating;
        }

    }
    public bool ifHaveTarget = false;
    public GameObject mostCloseTarget = null;
    public GameObject currentTarget
    {
        get
        {
            return m_head._currentTarget;
        }
        set
        {
            m_head._currentTarget = value;
        }
    }
    void Hunter()
    {
        if (!ifHaveTarget)
        {
            ChooseTarget();
        }
        TracingTarget();
    }
    void ChooseTarget()
    {
        if (m_head.HuntingList.Count > 0)
        {
            float mostCloseDistance = 0;
            for (int i = 0; i < m_head.HuntingList.Count; i++)
            {
                float distanceToTarget = Vector3.Distance(m_head.transform.position, m_head.HuntingList[i].transform.position);
                if (mostCloseTarget == null)
                {
                    currentTarget = m_head.HuntingList[i];
                    mostCloseDistance = Vector3.Distance(m_head.transform.position, currentTarget.transform.position);
                }
                else if (distanceToTarget < mostCloseDistance)
                {
                    currentTarget = m_head.HuntingList[i];
                    mostCloseDistance = distanceToTarget;
                }
            }
        }
        else if(m_head.HuntingList.Count == 0)
        {
            //Debug.Log("lost target");
            m_updateStage = SnakeMotionStage.Idle;
        }
    }

    void TracingTarget()
    {
        float distanceToTarget = Vector3.Distance(m_head.transform.position, currentTarget.transform.position);
        for (int i = 0; i < bodyList.Count; i++)
            {
            
                if (i == 0)
                    TracingMove(bodyList[i]);
                else
                    CheckDistanceWithForward(bodyList[i - 1], bodyList[i]);


            } 
        if(distanceToTarget < 1.39f)
        {
            m_updateStage = SnakeMotionStage.Eating;
            GrowUp();
        }

    }
    void TracingMove(GameObject _head)
    {
        //Vector3 velocity = (currentTarget.transform.position - _head.transform.position ).normalized* speed * 1.5f * Time.deltaTime;
        //_head.GetComponent<Rigidbody>().MovePosition(_head.transform.position + velocity);

        float _distance = Vector3.Distance(currentTarget.transform.position, _head.transform.position);
        Vector3 dir = (currentTarget.transform.position - _head.transform.position).normalized;
        float t = Time.deltaTime * _distance / 2 * speed * 0.35f;
        if (t > 0.5) t = 0.5f;
        _head.GetComponent<Rigidbody>().MovePosition(
            Vector3.Slerp(_head.transform.position, currentTarget.transform.position, t));
    }

    void PositionInitialize()
    {
        this.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
    }

    void AttributeIniate()
    {
        index = UnityEngine.Random.Range(12345, 823231);

        //stage initialize
        m_stage = SnakeLifeStage.RebornDelay;
        m_updateStage = SnakeMotionStage.Idle;

        m_Boundary = transform.parent.GetComponent<ScreenSpaceBoundary>();
        rebbornDelayTime = UnityEngine.Random.Range(rebornDelay.x, rebornDelay.y);
    }

    void SnakeInitializer()
    {
        //Head and body position random setting
        direction = UnityEngine.Random.Range(0, 100) > 50 ? 1 : -1;
        transform.localPosition = RegenerateStartPosition(direction);
        PositionInitialize();
        if(direction == 1)
        {
            
            for (int i=0; i < bodyList.Count; i++)
            {
                //head setting 
                if(i ==0)
                {
                    bodyList[0].transform.localPosition = Vector3.zero;
                    bodyList[0].GetComponent<Rigidbody>().Sleep();

                }
                //body setting
                else{
                    bodyList[i].transform.localPosition = bodyList[0].transform.localPosition;
                    bodyList[i].GetComponent<Rigidbody>().Sleep();
                }
            }
        }
        else
        {

            for (int i = 0; i < bodyList.Count; i++)
            {
                //head setting 
                if (i == 0)
                {
                    bodyList[0].transform.localPosition = Vector3.zero;
                }
                //body setting
                else
                {
                    bodyList[i].transform.localPosition = bodyList[0].transform.localPosition;;
                }
            }
        }
    }


    void CheckEachBodyIffStayInBoundary()
    {
        //bool[] checklist = new bool[bodyList.Count];
        for (int i=0; i < bodyList.Count; i++)
        {
            if(m_Boundary.isPointInside(bodyList[i].transform))
            {
                //Debug.Log("stay");
                return; 
            }
        }
        timerAtEnd += Time.deltaTime;
        if (timerAtEnd>endingDelay)
        {
            timer = 0;
            timerAtEnd = 0;
            m_stage = SnakeLifeStage.End;
            //Debug.Log("exit");
        }
        return;

    }

    Vector3 RegenerateStartPosition(int direction)
    {
        
        float newPositionX = (direction == 1? m_Boundary.minX : m_Boundary.maxX ) - 5f * direction;
        float newPositionY = (m_Boundary.maxY + m_Boundary.minY)/2 + ((m_Boundary.maxY  - m_Boundary.minY) / 4 ) * Mathf.Sin(Time.time * 0.5f);
        Vector3 newPosition = new Vector3(newPositionX ,newPositionY,0);
        return newPosition;
    }

    void SnakeGenerator()
    {
        HeadGenerator();
        BodyGenerator();
    }

    void HeadGenerator()
    {
        GameObject Head = (Instantiate(headPrefab) as GameObject);
        Head.transform.parent = this.transform;
        Head.transform.localPosition = new Vector3(0, 0, 0);

        Head.GetComponent<Renderer>().material.SetColor("_TintColor", headColor);
        Head.GetComponent<ParticleBase>().maxMoveSpeed = maxMoveSpeed;
        Head.GetComponent<SphereCollider>().radius = SensorTriggerRadius;

        //Snake layer
        Head.layer = 13;
        bodyList.Add(Head);

    }
    void BodyGenerator()
    {
        
        for (int i = 1; i <= numberOfBody;i++)
        {
            GameObject body = (Instantiate(bodyPrefab) as GameObject);
            body.transform.parent = this.transform;
            body.transform.position = bodyList[0].transform.position;
            body.GetComponent<Renderer>().material.SetColor("_TintColor", bodyColor);
            body.layer = 13;
            body.GetComponent<Rigidbody>().isKinematic = true;
            if(i%3 == 0)
            {
                body.GetComponent<Rigidbody>().isKinematic = false;
            }

            if(bodyList[i-1] == null)
            {
                HeadGenerator();
            }
            bodyList.Add(body);
        }
    }

    void SnakeMove()
    {
        //Debug.Log("StageControl");
        for (int i = 0; i < bodyList.Count;i++)
        {
            if(i == 0)
            {
                HeadMove(bodyList[i]);

            }else{
                
                CheckDistanceWithForward(bodyList[i - 1], bodyList[i]);
            }
        }
    }
    void CheckDistanceWithForward(GameObject forward, GameObject current)
    {
        current.GetComponent<Rigidbody>().Sleep();
        float _distance = Vector3.Distance(forward.transform.position, current.transform.position);
        Vector3 dir = (forward.transform.position - current.transform.position).normalized;
        float t = Time.deltaTime * _distance / bodyDistance * speed;
        if (t > 0.5) t = 0.5f;
        current.GetComponent<Rigidbody>().MovePosition(
                Vector3.Slerp(current.transform.position, forward.transform.position, t));
        
    }

    void HeadMove(GameObject _head)
    {
        _head.GetComponent<Rigidbody>().Sleep();
        if (direction == 0) direction = 1;
        float moveX = (Mathf.PerlinNoise(Time.time + index , index) - 0.1f) * 2 * speed * Time.deltaTime* direction;
        float moveY = Mathf.Sin(Time.time + index) * speed * Time.deltaTime;
        Vector3 velocity = new Vector3(moveX, moveY,0);
        _head.GetComponent<Rigidbody>().MovePosition(_head.transform.position + velocity);
        //Debug.Log(velocity);
        //if(!m_Boundary.isPointInside(_head.transform) && timer > 5)
        //{
        //    _head.GetComponent<Rigidbody>().MovePosition(_head.transform.position - velocity * 2);
        //}

    }

    void AddBody()
    {
        GameObject body = (Instantiate(bodyPrefab) as GameObject);
        body.transform.parent = this.transform;
        body.transform.position = bodyList[bodyList.Count-1].transform.position;
        body.layer = 13;
        body.GetComponent<Renderer>().material.SetColor("_TintColor", bodyColor);
        bodyList.Add(body);
    }



}

public class SnakeParticleBoundaryEvent : BoundaryEvent
{
   
    public override void StayBoundary()
    {
        throw new NotImplementedException();
    }
    public override void ExitBoundary()
    {
        throw new NotImplementedException();
    }
}
