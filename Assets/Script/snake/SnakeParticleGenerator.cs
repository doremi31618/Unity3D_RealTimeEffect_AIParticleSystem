using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Security.Cryptography;
using System;
/// <summary>
/// tip : need change the project setting collision let same tag object wont happen collision
/// </summary>
[System.Serializable]
public struct Snake
{
    
    public GameObject headPrefab;
    public GameObject bodyPrefab;

    public Color headColor;
    public Color bodyColor;

    public List<GameObject> bodyList;
    public bool[] bodyStayBoundaryCheckList;
}
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
    

    public GameObject headPrefab;
    public GameObject bodyPrefab;
    [Tooltip("only affect at fisrt time(start)")]public int numberOfBody = 5;
    [Tooltip("change by realtime")]public float bodyDistance;

    public float speed = 1;
    public Color headColor;
    public Color bodyColor;
    public Snake m_snake;
    public int direction = 1;
    [Header("setting Boundary Attribute ")]
    public bool isUseBoundarySystem;
    private bool isStayInBoundary;

    int index;

    //stage control
    SnakeLifeStage m_stage;
    SnakeMotionStage m_updateStage;

    [HideInInspector]public List<GameObject> bodyList = new List<GameObject>();

    ScreenSpaceBoundary m_Boundary;
	// Use this for initialization
	void Start () {
        PositionInitialize();
        SnakeGenerator();
        StartCoroutine(StageController());
	}
	
	// Update is called once per frame
	void Update () {
        SnakeMove();
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
        //switch
        switch(m_stage)
        {
            case SnakeLifeStage.RebornDelay:
                break;
            case SnakeLifeStage.Start:
                break;
            case SnakeLifeStage.Update:
                UpdateCycleStageSelector();
                break;
            case SnakeLifeStage.End:
                break;
        }
    }
    void UpdateCycleStageSelector()
    {
        switch (m_updateStage)
        {
            case SnakeMotionStage.Idle:
                SnakeMove();
                break;
            case SnakeMotionStage.Hunting:
                break;
            case SnakeMotionStage.Eating:
                break;
            case SnakeMotionStage.BeEaten:
                break;
        }
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
    }
    void SnakeInitializer()
    {
        //Head and body position random setting
        direction = UnityEngine.Random.Range(0, 100) > 50 ? 1 : -1;
        if(direction == 1)
        {
            
            for (int i=0; i < bodyList.Count; i++)
            {
                //head setting 
                if(i ==0)
                {
                    bodyList[0].transform.position = Vector3.zero;

                }
                //body setting
                else{
                    
                }
            }
        }
        else
        {
            
        }
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

            if(bodyList[i-1] == null)
            {
                HeadGenerator();
            }
            bodyList.Add(body);
        }
    }

    void SnakeMove()
    {
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
        float _distance = Vector3.Distance(forward.transform.position, current.transform.position);
        Vector3 dir = (forward.transform.position - current.transform.position).normalized;
        float t = Time.deltaTime * _distance / bodyDistance * speed;
        if (t > 0.5) t = 0.5f;
        current.GetComponent<Rigidbody>().MovePosition(
                Vector3.Slerp(current.transform.position, forward.transform.position, t));
        
    }

    void HeadMove(GameObject _head)
    {
        if (direction == 0) direction = 1;
        float moveX = (Mathf.PerlinNoise(Time.time + index , index) - 0.1f*direction) * 2 * speed * Time.deltaTime;
        float moveY = Mathf.Sin(Time.time + index) * speed * Time.deltaTime;
        Vector3 velocity = new Vector3(moveX, moveY,0);
        //Debug.Log(velocity);
        _head.GetComponent<Rigidbody>().MovePosition(_head.transform.position + velocity);
    }

    void AddBody()
    {
        GameObject body = (Instantiate(bodyPrefab) as GameObject);
        body.transform.parent = this.transform;
        body.transform.position = bodyList[0].transform.position;
        body.layer = 13;
        bodyList.Add(body);
    }



}

public class SnakeParticleBoundaryEvent : BoundaryEvent
{
    public bool isAllStayInBoundary;
    public override void StayBoundary()
    {
        throw new NotImplementedException();
    }
    public override void ExitBoundary()
    {
        throw new NotImplementedException();
    }
}
