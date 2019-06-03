using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using System.Timers;
//using NUnit.Framework.Internal.Commands;
[System.Serializable]
public class WaveParticleHunter : Hunter
{

    public ParticleMouse[] mice;
    public void setHuntingTargetToMice()
    {
        for (int i = 0; i < mice.Length;i++)
        {
            mice[i].HuntingTargets = HuntingTargets;
        }
    }

    public bool getIsMiceEating {
        get {
            for (int i = 0; i < mice.Length;i++)
            {
                if (mice[i].isEating) return true;
            }
            return false; 
        } 
    }
    public bool getIsMiceBeEaten{
        get {
            for (int i = 0; i < mice.Length;i++)
            {
                if (mice[i].isBeEaten) return true;
            }
            return false; 
        } 
    }
}

public class WaveParticleBehaviour : ParticleBehaiour
{
    public WaveParticleHunter m_hunter;

    [Header("Move motion setting")]
    [Tooltip("multiply")] public float globalMoveSpeed = 1;
    public float moveSpeed = 1;
    public int direction = 1;

    [Header("Color setting")]
    public Color firstColor;
    public Color secondColor;

    [Header("Time setting")]
    public float shiningIntervalTime = 0.15f;
    public float shiningTotalTime = 2f;
    public float eatingInterval = 2f;
    public float lerpTime = 2f;

    [Header("Animation speed")]
    public float defaultAnimationSpeed = 1;
    public float EatingAnimationSpeed = 3;
    public float BeEatenAnimationSpeed = 5;
    private int index;
    private float timer;
    private float timerAtFirst;
    float nextTimeToEatOrBeEaten;
    private void Start()
    {
        m_hunter.setHuntingTargetToMice();
        InitializeComponent();

    }

    private void FixedUpdate()
    {
        LifeCycleStateSelector();
    }

    #region state setting function
    //state setting function

    //life state funtion region
    public override void LifeCycleStateSelector()
    {
        timer += Time.deltaTime;
        switch (stateNow)
        {
            case ParticleLifeState.Start:
                Initialize();
                break;
            case ParticleLifeState.Update:
                UpdateCycleStateSelector();
                break;
            case ParticleLifeState.End:
                EndBehaviour();
                break;

        }
    }
    public override void Initialize()
    {

        direction = m_Boundary.worldCenter.x - transform.position.x > 0 ? 1 : -1;
        RegenerateStartPosition(direction);
        GetComponent<SpriteRenderer>().color = firstColor;
        stateNow = ParticleLifeState.Update;
        timer = 0;
    }
    
    public void EndBehaviour()
    {
        stateNow = ParticleLifeState.Start;
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
                GetComponent<Animator>().speed = defaultAnimationSpeed;
                IdldeEventHandler();
                break;
            case ParticleMotionState.Eating:
                break;
            case ParticleMotionState.BeEaten:
                break;
            case ParticleMotionState.EndOfUpdate:
                EndOfUpdateEventHandler();
                break;
        }
    }

    public void EventManager()
    {
        //Debug.Log("isEating" + m_hunter.getIsMiceEating);
        if (!m_Boundary.isPointInside(transform) && timer > 5)
        {
            stateNow = ParticleLifeState.End;
        }
        else if(!inAnimation &&
                Time.time > nextTimeToEatOrBeEaten &&
                m_hunter.getIsMiceBeEaten &&
                motionStateNow != ParticleMotionState.BeEaten)
        {
            
            GetComponent<Animator>().speed = BeEatenAnimationSpeed;
            StartCoroutine(ColorShining(firstColor, secondColor, shiningTotalTime, shiningIntervalTime));
            motionStateNow = ParticleMotionState.BeEaten;
        }
        else if (!inAnimation &&
                Time.time > nextTimeToEatOrBeEaten && 
                 m_hunter.getIsMiceEating &&
                 motionStateNow != ParticleMotionState.Eating)
        {
            GetComponent<Animator>().speed = EatingAnimationSpeed;
            StartCoroutine(ColorLerpChanging(firstColor, secondColor, lerpTime));
            motionStateNow = ParticleMotionState.Eating;
        }
        //else 
        //{
        //    motionStateNow = ParticleMotionState.Idle;
        //}
    }

    void FirstUpdateEventHandler()
    {
        timerAtFirst += Time.deltaTime;
    }

    void IdldeEventHandler() { Run(); }
    void EndOfUpdateEventHandler(){}

    #endregion

    #region behaviour function

    //behaviour function
    void Run()
    {
        float velocity = Mathf.PerlinNoise(Time.time + index, index) * direction * moveSpeed * Time.deltaTime;
        m_rigidbody.MovePosition(transform.position + velocity * Vector3.right);

    }
    bool inAnimation = false;
    IEnumerator ColorShining(Color _firstColor, Color _secondColor, float _shiningTotalTime, float _shiningIntervalTime)
    {
        inAnimation = true;
        int counter = 0;
        float endingTime = Time.time + _shiningTotalTime;
        Color nowColor = GetComponent<SpriteRenderer>().color;

        while (Time.time < endingTime)
        {
            counter += 1;
            switch (counter % 2)
            {
                case 1:
                    nowColor = _firstColor;
                    break;
                case 0:
                    nowColor = _secondColor;
                    break;
            }

            GetComponent<SpriteRenderer>().color = nowColor;
            yield return new WaitForSeconds(_shiningIntervalTime);

        }
        inAnimation = false;
        GetComponent<SpriteRenderer>().color = _firstColor;
        motionStateNow = ParticleMotionState.Idle;
        nextTimeToEatOrBeEaten = eatingInterval+ Time.time;
        foreach (ParticleMouse mouse in m_hunter.mice) mouse.isBeEaten = false;
    }

    IEnumerator ColorLerpChanging(Color _firstColor, Color _finalColor, float _lerpTotalTime)
    {
        inAnimation = true;
        float endingTime = Time.time + _lerpTotalTime;
        Color nowColor = GetComponent<SpriteRenderer>().color;

        //avoid blink
        if(nowColor != _firstColor || nowColor == _finalColor)
        {
            Color temp = _firstColor;
            _firstColor = _finalColor;
            _finalColor = temp;
        }

        while (Time.time < endingTime)
        {
            nowColor = Color.Lerp(_finalColor, _firstColor, (endingTime - Time.time) / _lerpTotalTime);
            GetComponent<SpriteRenderer>().color = nowColor;
            yield return new WaitForEndOfFrame();
        }
        inAnimation = false;
        GetComponent<SpriteRenderer>().color = _finalColor;
        motionStateNow = ParticleMotionState.Idle;
        nextTimeToEatOrBeEaten = eatingInterval + Time.time;
        foreach (ParticleMouse mouse in m_hunter.mice) mouse.isBeEaten = false;
        //GetComponent<SpriteRenderer>().material.SetColor("_TintColor", _firstColor);

    }

    #endregion
        
}
