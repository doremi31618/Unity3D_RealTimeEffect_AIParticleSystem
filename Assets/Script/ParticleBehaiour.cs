using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public struct ApearenceStructure
{
    public Gradient randomRebbornColor;
}
[System.Serializable]
public struct PhysicMotionSetting
{
    public float speed;
    public float maxMoveSpeed;
}
public enum ParticleLifeState
{
    RebornDelay,
    Start,
    Update,
    End
}
public enum ParticleMotionState
{
    Idle,
    Hunting,
    Eating,
    BeEaten,
    interactive,
    grow,
    FirstUpdate,
    EndOfUpdate
}
public enum LayerManager
{
    Object = 9,
    Player = 10,
    SpringJointWorm = 12,
    Snake = 13,
    EmitterParticle = 14,
    Wave = 15,
    Trash = 16


}

public class ParticleBehaiour : MonoBehaviour {

    //public Hunter hunter;
    [Header("State  setting")]
    public bool isHunter = false;
    public ParticleLifeState stateNow;
    public ParticleMotionState motionStateNow;

    [Header("Growing Up attribute")]
    public bool isUseRandomReborn;
    public bool canItGrowUp = false;
    public int HowMuchNumberForEatingToGrowUp;
    [HideInInspector] public bool beEaten = false;

    [Header("struct data setting")]
    public PhysicMotionSetting physicMotion;
    //int index;
    [HideInInspector]public ScreenSpaceBoundary m_Boundary;
    [HideInInspector]public Rigidbody m_rigidbody;
    [HideInInspector]public SphereCollider m_collider;
    public void InitializeComponent()
    {
        m_Boundary = transform.parent.GetComponent<ScreenSpaceBoundary>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_collider = GetComponent<SphereCollider>();
    }

    public virtual void RandomReborn(){}
    public virtual void Initialize(){}
    public virtual void LifeCycleStateSelector(){}
    public virtual void UpdateCycleStateSelector(){}
    //public virtual void EventManagement(){}
    public virtual void GrowUp()
    {
        if(canItGrowUp)
        {
            
        }
    }

    public Vector3 RegenerateStartPosition(int direction)
    {
        float newPositionX = (direction == 1 ? m_Boundary.minX : m_Boundary.maxX) - 5f * direction;
        float newPositionY = (m_Boundary.maxY + m_Boundary.minY) / 2 + ((m_Boundary.maxY - m_Boundary.minY) / 4) * Mathf.Sin(Time.time * 0.5f);
        Vector3 newPosition = new Vector3(newPositionX, newPositionY, 0);
        return newPosition;
    }

    public void PositionInitialize()
    {
        this.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
    }
}
