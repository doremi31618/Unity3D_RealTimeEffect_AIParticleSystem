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
//using UnityEditor.ShaderGraph;
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

public class SnakeParticleGenerator : MonoBehaviour
{

    [Header("Prefabs Attribute setting")]
    public GameObject headPrefab;
    public GameObject bodyPrefab;

    [Header("Apearence Attribbute")]
    public Color headColor;
    public Color bodyColor;

    [Tooltip("only affect at fisrt time(start)")] public int numberOfBody = 5;
    public int minBodyNunber = 3;
    [Tooltip("change by realtime")] public float bodyDistance;

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

    
    [Header("Snake body animation")]
    public bool isUseSnakeBodyAnimation = true;
    public Color color1 = Color.white;
    public Color color2 = Color.white;

    public Color beEatenColor = Color.red;
    public float animationClipTime = 1;
    public float animationFreshRate = 20f;
    
    public float AnimationSpeed = 1;
    bool isUseAntiOrder=false;
    public int maxIntervalNumber = 3;

    
    [Header("state visualizer")]
    public AnimationState animationState;
    public enum AnimationState
    {
        start,
        update,
        end
    }
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

    [HideInInspector] public List<GameObject> bodyList = new List<GameObject>();
    GameObject head;
    ScreenSpaceBoundary m_Boundary;

    public ParticleSystem deathParticleEffect;
    SnakeHead m_head
    {
        get
        {
            return bodyList[0].GetComponent<SnakeHead>();
        }
    }

    bool isBeEaten
    {
        get{
            foreach(GameObject body in bodyList)
            {
                bool _isBeEaten = body.GetComponent<ParticleBase>().isBeaten;
                if(_isBeEaten)return true;
            }
            return false;
        }
    }
    //[Header("setting Boundary Attribute ")]
    //public bool isUseBoundarySystem;
    //public bool isUseRandomAttributeToReborn;
    // Use this for initialization
    void Start()
    {
        PositionInitialize();
        SnakeGenerator();
        AttributeIniate();
        StartCoroutine(SnakeColorLerpAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        LifeCycleStageSelector();
        //SnakeMove();
    }

    IEnumerator StageController()
    {
        while (true)
        {
            LifeCycleStageSelector();
            yield return new WaitForEndOfFrame();
        }
    }

    void LifeCycleStageSelector()
    {
        timer += Time.deltaTime;
        //switch
        switch (m_stage)
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
    bool isTimerEnd = true;
    IEnumerator animationSpeedUp(float _timeLength,float _speed)
    {
        if(!isTimerEnd)yield break;
        isTimerEnd = false;
        AnimationSpeed = _speed;
        speed *=2;
        yield return new WaitForSeconds(_timeLength);
        AnimationSpeed = 1;
        speed /= 2;
        isTimerEnd = true;
    }
    IEnumerator animationSpeedDown(float _timeLength,float _speed)
    {
        if(!isTimerEnd)yield break;
        Color temp = color2;
        color2 = beEatenColor;
        isTimerEnd = false;

        updownSpeed = 5;
        if(bodyList.Count > minBodyNunber )
            RemoveBody();
        animationState = AnimationState.start;
        AnimationSpeed = _speed;
        //speed *= 0.5f;
        yield return new WaitForSeconds(_timeLength);
        AnimationSpeed = 1;
        //speed *= 2;
        updownSpeed = 1;
        isTimerEnd = true;
        m_head.beEaten = false;
        color2 = temp;
    }
    void UpdateCycleStageSelector()
    {
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
                GrowUp();
                m_updateStage = SnakeMotionStage.Idle;
                break;
            case SnakeMotionStage.BeEaten:
                SnakeMove();
                StartCoroutine(animationSpeedDown(1,0.5f));
                break;
        }
        if (timer > 5) CheckEachBodyIffStayInBoundary();
    }
    void GrowUp()
    {
        StartCoroutine(animationSpeedUp(1f,3));
        //StartCoroutine(animationSpeedDown(1,0.5f));
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
            //Debug.Log("grow");
            AddBody();
        }
        m_updateStage = SnakeMotionStage.Idle;
    }

    //define what stage should be 
    void EventManagement()
    {
        SnakeHead m_head = head.GetComponent<SnakeHead>();
        if (m_head.HuntingList.Count > 1 && !m_head.beEaten)
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
        else if (m_head.getIfEating)
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
        else if (m_head.HuntingList.Count == 0)
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
        if (distanceToTarget < 1.39f)
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
        deathParticleEffect.Stop();
    }

    void SnakeInitializer()
    {
        //Head and body position random setting
        direction = UnityEngine.Random.Range(0, 100) > 50 ? 1 : -1;
        transform.localPosition = RegenerateStartPosition(direction);
        PositionInitialize();
        if (direction == 1)
        {

            for (int i = 0; i < bodyList.Count; i++)
            {
                if(i>=bodyList.Count)return;
                //head setting 
                if (i == 0)
                {
                    bodyList[0].transform.localPosition = Vector3.zero;
                    bodyList[0].GetComponent<Rigidbody>().Sleep();
                    bodyList[0].GetComponent<SnakeHead>().beEaten = false;

                }
                //body setting
                else
                {
                    bodyList[i].transform.localPosition = bodyList[0].transform.localPosition;
                    bodyList[i].GetComponent<Rigidbody>().Sleep();
                }

                //bodyList[i].GetComponent<ParticleBase>().isBeaten = false;
            }
        }
        else
        {

            for (int i = 0; i < bodyList.Count; i++)
            {
                if(i>=bodyList.Count)return;
                //head setting 
                if (i == 0)
                {
                    bodyList[0].transform.localPosition = Vector3.zero;
                }
                //body setting
                else
                {
                    bodyList[i].transform.localPosition = bodyList[0].transform.localPosition; ;
                }
            }
        }
    }


    void CheckEachBodyIffStayInBoundary()
    {
        //bool[] checklist = new bool[bodyList.Count];
        for (int i = 0; i < bodyList.Count; i++)
        {
            if(i>=bodyList.Count)return;
            if (m_Boundary.isPointInside(bodyList[i].transform))
            {
                //Debug.Log("stay");
                return;
            }
        }
        timerAtEnd += Time.deltaTime;
        if (timerAtEnd > endingDelay)
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
        float newPositionX = (direction == 1 ? m_Boundary.minX : m_Boundary.maxX) - 5f * direction;
        float newPositionY = (m_Boundary.maxY + m_Boundary.minY) / 2 + ((m_Boundary.maxY - m_Boundary.minY) / 4) * Mathf.Sin(Time.time * 0.5f);
        Vector3 newPosition = new Vector3(newPositionX, newPositionY, 0);
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
        head = Head;
    }
    void BodyGenerator()
    {

        for (int i = 1; i <= numberOfBody; i++)
        {
            GameObject body = (Instantiate(bodyPrefab) as GameObject);
            body.transform.parent = this.transform;
            body.transform.position = bodyList[0].transform.position;
            body.GetComponent<Renderer>().material.SetColor("_TintColor", bodyColor);
            body.layer = 13;
            body.GetComponent<Rigidbody>().isKinematic = true;
            if (i % 3 == 0)
            {
                body.GetComponent<Rigidbody>().isKinematic = false;
            }

            if (bodyList[i - 1] == null)
            {
                HeadGenerator();
            }
            bodyList.Add(body);
        }
    }

    void SnakeMove()
    {
        //Debug.Log("StageControl");
        for (int i = 0; i < bodyList.Count; i++)
        {
            if (i == 0)
            {
                HeadMove(bodyList[i]);

            }
            else
            {
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

    float updownSpeed = 1;
    void HeadMove(GameObject _head)
    {
        _head.GetComponent<Rigidbody>().Sleep();
        if (direction == 0) direction = 1;
        float moveX = (Mathf.PerlinNoise(Time.time + index, index) - 0.1f) * 2 * speed * Time.deltaTime * direction;
        float moveY = Mathf.Sin((Time.time)+ index) * 3 * Time.deltaTime *(1/updownSpeed);
        Vector3 velocity = new Vector3(moveX, moveY, 0);
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
        body.transform.position = bodyList[bodyList.Count - 1].transform.position;
        body.layer = 13;
        body.GetComponent<Renderer>().material.SetColor("_TintColor", bodyColor);
        bodyList.Add(body);
    }
    void RemoveBody()
    {
        GameObject removeTarget = bodyList[bodyList.Count - 1];
        snakeAddingMaxNumber ++;
        bodyList.Remove(removeTarget);
        removeTarget.transform.parent = GameObject.Find("TrashLayer").transform ;
        removeTarget.tag = "LittleParticle";
        removeTarget.layer = 9;

        deathParticleEffect.transform.position = bodyList[bodyList.Count-1].transform.position;
        deathParticleEffect.Play();

        removeTarget.AddComponent<MainParticleLifeCycle>().fisrtColor = color1;
        removeTarget.GetComponent<MainParticleLifeCycle>().finalColor = color2;
        removeTarget.GetComponent<MainParticleLifeCycle>().localMoveSpeed = 0;
        removeTarget.GetComponent<MainParticleLifeCycle>().m_stage = mainParticleStage.update;
        removeTarget.GetComponent<MainParticleLifeCycle>().lifeTime = 60f;
    }
    #region  bodyAnimation
    bool isLoop = true;
    bool[] states;
    float animationTimer;

    IEnumerator SnakeColorLerpAnimation()
    {
        //if(!isUseSnakeBodyAnimation)isLoop = false;
        while (isLoop)
        {
            stateSelector();
            AnimationStateSelector();
            //Debug.Log("Refresh animation");
            yield return new WaitForSeconds(1/animationFreshRate);
        }
    }
    public bool isPlaying
    {
        get
        {
            if ((int)animationTimer / animationClipTime <= 0 || (int)animationTimer / animationClipTime >= 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    void AnimationStateSelector()
    {
        switch(animationState)
        {
            case AnimationState.start:
                InitializeAnimation(ref states);
                break;
            case AnimationState.update:
                UpdateAnimation(ref states);
                break;
            case AnimationState.end:
                EndAnimation(ref states);
                break;
        }
    }
    public virtual void InitializeAnimation(ref bool[] _states)
    {
        states = new bool[bodyList.Count];
        animationTimer += Time.deltaTime * Mathf.Abs(AnimationSpeed);
    }
    public virtual void UpdateAnimation(ref bool[] _states) {
         animationTimer += Time.deltaTime * Mathf.Abs(AnimationSpeed); 
         AnimatiionLogic_1(isUseAntiOrder,maxIntervalNumber,ref states);
    }
    public virtual void EndAnimation(ref bool[] _states)
    {
        animationTimer = 0;
    }

    public void stateSelector(){
            if (animationTimer == 0)
                animationState = AnimationState.start;
            else if (animationTimer > 0 && animationTimer < animationClipTime)
                animationState = AnimationState.update;
            else if (animationTimer > animationClipTime)
                animationState = AnimationState.end;
        }
    void AnimatiionLogic_1(bool _isUseAntiOrder, int maxDarkNumber, ref bool[] _states)
    {
        int dir = _isUseAntiOrder ? -1 : 1;
        int timeIndex = (int)Mathf.Lerp(0, _states.Length + (maxDarkNumber), animationTimer / animationClipTime) * dir;
        int min = (_isUseAntiOrder ? (_states.Length - 1) + maxDarkNumber : 0 - maxDarkNumber) + timeIndex;
        int max = (_isUseAntiOrder ? min - (maxDarkNumber - 1) : min + (maxDarkNumber - 1));
        //Debug.Log("min : " + min + " max : " + max);
        for (int i = 0; i < _states.Length; i++)
        {
            float i_min = Mathf.Abs(i - min);
            float i_max = Mathf.Abs(i - max);
            //Debug.Log("min : " + min + " max : " + max + " i : " + i + " i-max : " + i_max + " i-min : " + i_min);
            if (i_min + i_max < maxDarkNumber)
            {
                _states[i] = LogicStateSetting.State1 ;
                //Debug.Log("Lighting up");
            }
            else
            {
                _states[i] = LogicStateSetting.State2;

                //Debug.Log("Lighting off");
            }
        }
        if(!isUseSnakeBodyAnimation) return;

        Color[] listColor = new Color[_states.Length];
        float center = (max + min)/2 ;

        for (int i = 0; i < listColor.Length; i++)
        {
            //Debug.Log(_states[i]);
            //Debug.Log("min : " + min + " max : " + max + " i : " + i + " i-max : " + i_max + " i-min : " + i_min);
            if (_states[i])
            {

                float index = i >= center ? i - center : center - i;
                float length = i >= center ? max - center : center - min;
                listColor[i] = Color.Lerp(color2, color1, index / length);
            }
            else
            {
                listColor[i] = color1;
            }
            if(i<bodyList.Count)
                bodyList[i].GetComponent<MeshRenderer>().material.SetColor("_TintColor", listColor[i]);
        }


    }
    #endregion

}
public static class LogicStateSetting
{
    //default true
    public static bool State1 = true;
    //default false
    public static bool State2 = false;
    //public bool isUseAntiLogic = false;
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
