using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SpringJointWormHunter : Hunter
{
}
public class SpringJointWormBehaviour : ParticleBehaiour
{
    [Header("Move motion setting")]
    [Tooltip("multiply")] public float globalMoveSpeed = 1;
    public float moveSpeed = 1;
    public int direction = 1;

    private int index;
    private float timer;
    private float timerAtFirst;
    float nextTimeToEatOrBeEaten;
    private void Start()
    {
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
            case ParticleLifeState.RebornDelay:
                break;
            case ParticleLifeState.Start:
                Initialize();
                break;
            case ParticleLifeState.Update:
                IdldeEventHandler();
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
        //GetComponent<SpriteRenderer>().color = firstColor;
        stateNow = ParticleLifeState.Update;
        timer = 0;
    }
    public void EndBehaviour()
    {
        stateNow = ParticleLifeState.Start;
    }
    void FirstUpdateEventHandler()
    {
        timerAtFirst += Time.deltaTime;
    }

    void IdldeEventHandler() { 
        if (!m_Boundary.isPointInside(transform) && timer > 5)
        {
            stateNow = ParticleLifeState.End;
        }
        Run(); 
    }
    void EndOfUpdateEventHandler(){}

    #endregion

    #region behaviour function

    //behaviour function
    void Run()
    {
        float velocity = Mathf.PerlinNoise(Time.time + index, index) * direction * moveSpeed * Time.deltaTime;
        m_rigidbody.MovePosition(transform.position + velocity * Vector3.right);

    }
    #endregion
        
}
