using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography.X509Certificates;

[RequireComponent(typeof(SphereCollider))]
public class TrackingTargetBehaviour : MonoBehaviour
{
    [Header("Motion setting")]
    public float circleAroundRadius=5;
    public float trackingSpeed = 1;
    public float circcleAroundSpeed = 1;

    [Header("Trigger setting")]
    public float triggerRadius;
    public bool isUseVisualizer = true;
    public Color nowTargetColor = Color.white;
    public Color otherTargetColor = Color.red;

    [Header("Target list")]
    public List<GameObject> allTargets;
    public GameObject NowTarget;

    Rigidbody m_rigidbody;
    MotionState m_state = MotionState.Idle;
    enum MotionState
    {
        Idle,
        Tracking,
        CircleArdound
    }
    // Start is called before the first frame update
    void Start()
    {
        triggerRadius = GetComponent<SphereCollider>().radius;
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //checkEveryObject();
        Moving();
    }
    void Moving()
    {
        MovingEventSelector();
        SwitchState();
    }
    void MovingEventSelector()
    {
        if(NowTarget == null)
        {
            m_state = MotionState.Idle;
        }
        else
        {
            float distanceToTarget = Vector3.Distance(NowTarget.transform.position, transform.position);
            if(distanceToTarget > circleAroundRadius)
            {
                m_state = MotionState.Tracking;
            }
            else if(distanceToTarget <= circleAroundRadius)
            {
                m_state = MotionState.CircleArdound;
            }
        }
    }
    void SwitchState()
    {
        switch(m_state)
        {
            case MotionState.Idle:
                IdleMove();
                break;
            case MotionState.Tracking:
                TrackingMove();
                break;
            case MotionState.CircleArdound:
                CircleAroundMove();
                break;
        }
    }
    void IdleMove()
    {
        Debug.Log("Idle Move");

    }
    void TrackingMove()
    {
        Debug.Log("TrackingMove");
        //Vector3 moveStep = (NowTarget.transform.position - transform.position).normalized;
        //m_rigidbody.MovePosition(transform.position + moveStep * trackingSpeed * Time.deltaTime);
        LerpMove(NowTarget.transform.position, transform.position, 5f);

    }
    void CircleAroundMove()
    {
        Debug.Log("CircleAround");
        Vector3 towardDir = Vector3.Cross(transform.position - NowTarget.transform.position, Vector3.forward).normalized;
        m_rigidbody.MovePosition(transform.position + towardDir * circcleAroundSpeed * Time.deltaTime);
    }

    void LerpMove(Vector3 forwardPosition, Vector3 currentPosition, float minCloseDistance)
    {
        float _distance = Vector3.Distance(forwardPosition, currentPosition);
        Vector3 dir = (forwardPosition - currentPosition).normalized;
        float t = Time.deltaTime * _distance / minCloseDistance * trackingSpeed;
        if (t > 0.5) t = 0.5f;
        m_rigidbody.MovePosition(
        Vector3.Slerp(currentPosition, forwardPosition, t));
    }

    void checkEveryObject()
    {
        if (allTargets.Count == 0) return;
        float mostCloseDistance = 10;
        foreach(GameObject gb in allTargets.ToArray())
        {
            
            bool active = gb.active;
            float distanceToTarget = Vector3.Distance(gb.transform.position, transform.position);
            bool ifOutOfRange = ( distanceToTarget > triggerRadius * transform.localScale.x);
            Debug.Log("Distance to target : " + distanceToTarget + " most close distance : " + mostCloseDistance);

            //if (NowTarget == null && mostCloseDistance > distanceToTarget)
            //{
            //    mostCloseDistance = distanceToTarget;
            //    NowTarget = gb;
            //}
            
            if (!active || ifOutOfRange)
            {
                if (NowTarget == gb)
                    NowTarget = null;
                allTargets.Remove(gb);

                    
            }

            Color color = gb == NowTarget ? nowTargetColor : otherTargetColor;
            if(isUseVisualizer) 
                Debug.DrawRay(transform.position, gb.transform.position - transform.position, color);  
                
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        addListObject(other.gameObject);
    }
    //use to check every object is existing
    private void OnTriggerStay(Collider other)
    {
        addListObject(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        removeListObject(other.gameObject);
    }

    void addListObject(GameObject _gb)
    {
        GameObject enterGameObject = _gb;

        if (enterGameObject.tag != "TrackingTarget") return;
        if (!allTargets.Contains(enterGameObject))
        {
            allTargets.Add(enterGameObject);
            NowTarget = enterGameObject;
        }
            
    }
    void removeListObject(GameObject _gb)
    {
        GameObject enterGameObject = _gb;
        if (enterGameObject.tag != "TrackingTarget") return;

        if (allTargets.Contains(enterGameObject))
            allTargets.Remove(enterGameObject);

        if (NowTarget == enterGameObject)
        {
            NowTarget = null;
        }
    }
}
