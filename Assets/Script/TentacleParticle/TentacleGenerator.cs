using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class TentacleGenerator : MonoBehaviour
{
    //PUBLIC
    [Header("Line Renderer Setting")]
    public float lineSegmentSize = 0.15f;
    public float lineWidth = 0.1f;
    public float lineLength = 5;

    public Material lineMaterial;
    public Color lineStartColor;
    public Color lineEndColor;

    [Header("Line Motion")]
    [Range(2,10)]public int childLinePoint = 5;

    [Header("LinePhysic")]
    public bool isUsePhysic = false;
    public RigidbodyConstraints constraints;



    [Header("Gizmos")]
    public bool showGizmos = true;
    public float gizmoSize = 0.1f;
    public Color gizmoColor = new Color(1, 0, 0, 0.5f);

    //PRIVATE
    private CurvedLinePoint[] linePoints = new CurvedLinePoint[0];
    private Vector3[] linePositions = new Vector3[0];
    private Vector3[] linePositionsOld = new Vector3[0];

    private void Start()
    {
        LineInitialize();
        GanerateChildGameObject();
    }
    // Update is called once per frame
    public void Update()
    {
        GetPoints();
        SetPointsToLine();
    }
    void GanerateChildGameObject()
    {
        for (int i = 1; i <= childLinePoint;i++)
        {
            GameObject LinePoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            LinePoint.name = "LinePoint" + i;
            LinePoint.transform.parent = this.transform;
            LinePoint.transform.localPosition = (i/ lineLength) * Vector3.up;
            LinePoint.AddComponent<CurvedLinePoint>();
            LinePoint.GetComponent<MeshRenderer>().enabled = false;

            if (isUsePhysic)
            {
                LinePoint.AddComponent<Rigidbody>();
                SphereCollider _collider = LinePoint.GetComponent<SphereCollider>();
                _collider.radius = lineWidth;

                Rigidbody _rigdbody = LinePoint.GetComponent<Rigidbody>();
                _rigdbody.useGravity = false;
                _rigdbody.constraints = constraints;
            }
        }

    }
    void LineInitialize()
    {
        LineRenderer line = this.GetComponent<LineRenderer>();

        line.SetWidth(lineWidth, lineWidth);
        line.material = lineMaterial;
        line.SetColors(lineStartColor, lineEndColor);

    
    }
    void GetPoints()
    {
        //find curved points in children
        linePoints = this.GetComponentsInChildren<CurvedLinePoint>();

        //add positions
        linePositions = new Vector3[linePoints.Length];
        for (int i = 0; i < linePoints.Length; i++)
        {
            linePositions[i] = linePoints[i].transform.position;
        }
    }

    void SetPointsToLine()
    {
        //create old positions if they dont match
        if (linePositionsOld.Length != linePositions.Length)
        {
            linePositionsOld = new Vector3[linePositions.Length];
        }

        //check if line points have moved
        bool moved = false;
        for (int i = 0; i < linePositions.Length; i++)
        {
            //compare
            if (linePositions[i] != linePositionsOld[i])
            {
                moved = true;
            }
        }

        //update if moved
        if (moved == true)
        {
            LineRenderer line = this.GetComponent<LineRenderer>();

            //get smoothed values
            Vector3[] smoothedPoints = LineSmoother.SmoothLine(linePositions, lineSegmentSize);

            //set line settings
            line.SetVertexCount(smoothedPoints.Length);
            line.SetPositions(smoothedPoints);
            line.SetWidth(lineWidth, lineWidth);
            //line.SetColors(lineStartColor, lineEndColor);
        }
    }

    void OnDrawGizmosSelected()
    {
        Update();
    }

    void OnDrawGizmos()
    {
        if (linePoints.Length == 0)
        {
            GetPoints();
        }

        //settings for gizmos
        foreach (CurvedLinePoint linePoint in linePoints)
        {
            linePoint.showGizmo = showGizmos;
            linePoint.gizmoSize = gizmoSize;
            linePoint.gizmoColor = gizmoColor;
        }
    }
}

