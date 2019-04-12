using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BoundaryBase : MonoBehaviour {
    
    public void CollisionBoundary(Transform particleTransform,ref BoundaryEvent @event){

        if(isPointInside(particleTransform))
        {
            @event.StayBoundary();
        }
        else
        {
            @event.ExitBoundary();
        }
    }

    public virtual bool isPointInside(Transform _point){
        return false;
    }

}
