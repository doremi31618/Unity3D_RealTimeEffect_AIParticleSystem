using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class ScreenSpaceBoundary : MonoBehaviour {
    [Header("塞入相機物件")]
    public GameObject cameraObject ;

    [Header("到相機的距離")]
    [Range(0f,100f)]public float distanceToCamera = 10;

    public Color debugColor = Color.white;

    [HideInInspector]public float minX, maxX, minY, maxY = 0;
    [HideInInspector]public Vector3 worldCenter;

	

    private void Update()
    {
        SetDistanceToCamera();
        GetBoundaryConrner();
        BoundaryVisualizer();
    }
    void SetDistanceToCamera()
    {
        //check camera gameObject
        if(cameraObject == null)
            cameraObject = GameObject.FindWithTag("MainCamera");
        
        this.transform.position = new Vector3(0, cameraObject.transform.position.y, distanceToCamera);
        worldCenter = this.transform.position;
    }

    void GetBoundaryConrner()
    {
        
        Vector3 bottomCorner = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distanceToCamera));
        Vector3 TopCorner = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distanceToCamera));

        maxX = TopCorner.x;
        minX = bottomCorner.x;
        maxY = TopCorner.y;
        minY = bottomCorner.y;

    }
    void BoundaryVisualizer()
    {

        Vector3 TopCornerRight = new Vector3(maxX,maxY,transform.position.z);
        Vector3 TopCornerLeft = new Vector3(minX, maxY, transform.position.z);
        Vector3 BottomCornerRight = new Vector3(maxX, minY, transform.position.z);
        Vector3 BottomCornerLeft = new Vector3(minX, minY, transform.position.z);


        Debug.DrawRay(TopCornerRight, TopCornerLeft - TopCornerRight , debugColor);
        Debug.DrawRay(TopCornerRight, BottomCornerRight - TopCornerRight , debugColor);
        Debug.DrawRay(BottomCornerLeft, TopCornerLeft - BottomCornerLeft  , debugColor);
        Debug.DrawRay(BottomCornerLeft, BottomCornerRight - BottomCornerLeft , debugColor);

    }

}

