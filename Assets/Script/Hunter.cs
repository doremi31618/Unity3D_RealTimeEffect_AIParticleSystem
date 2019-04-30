using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Hunter
{
    public LayerManager m_Layer;
    public LayerManager[] HuntingTargets;
    public ParticleMouse mouse;
    [HideInInspector] public bool ifHasTarget
    {
        get{
            return HuntingList.Count > 0;
        }
    }
    public List<GameObject> HuntingList;
    public bool getIsEating{get { return mouse.isEating; }}

    public virtual void SelectTracingTarget()
    {

    }
    public virtual void TracingTarget()
    {

    }

}

