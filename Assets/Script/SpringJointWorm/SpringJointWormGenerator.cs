using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.Remoting;
using System.Security.Policy;

public class SpringJointWormGenerator : MonoBehaviour {
    
    [Header("Single Shape Setting")]
    [Range(5,12)]public int frequency = 5;
    [Range(1, 10)] public float radius;
    public Material material;
    public Mesh mesh;

    public Color centerColor;
    public Color cornerColor;

    [Header("Single Worm Physic Attribute Setting")]
    public float mass = 50;
    public float drag = 1.5f;
    public float spring = 50f;
    public RigidbodyConstraints constraints = RigidbodyConstraints.FreezeRotation;

    [Header("Moving Attribute")]
    public float minSpeedOfSpinAround = 50;
    public float maxSpeedOfSpinArdoun = 150;

    [Header("Random Attribute range")]
    public bool isUseRandomAttribute = true;

    public Gradient randomCenterColor;
    public Gradient randomCornerColor;

    public Vector2 randomFrequencyBetweenTwoConstant = new Vector2(5,12);
    public Vector2 randomRadiusBetweenTwoConstant = new Vector2(1, 15);
    public Vector2 minSpinAroundBetweenTwoConstant = new Vector2(75, 100);
    public Vector2 maxSpinAroundBetweenTwoConstant = new Vector2(100, 200);
    #region private attribute
    bool alreadyBuild = false;
    float index;
    GameObject center;
    ScreenSpaceBoundary m_Boundary;
    #endregion

    //List<GameObject> particles;

    // Use this for initialization
    void Start () 
    {
        PositionInitialize();
        RandomBornSetting();

        GameObjectGenerator();
	}
    void PositionInitialize()
    {
        this.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
    }

    void RandomBornSetting()
    {
        if (!isUseRandomAttribute) return;
        m_Boundary = transform.parent.GetComponent<ScreenSpaceBoundary>();

        this.transform.localPosition = new Vector3(
            Random.Range(m_Boundary.minX, m_Boundary.maxX),
            Random.Range(m_Boundary.minY, m_Boundary.maxY), 0);
        
        index = Random.Range(12452, 59291241);
        frequency = (int)Random.Range(randomFrequencyBetweenTwoConstant.x, randomFrequencyBetweenTwoConstant.y);
        radius = (int)Random.Range(randomRadiusBetweenTwoConstant.x, randomRadiusBetweenTwoConstant.y);

        minSpeedOfSpinAround = Random.Range(minSpinAroundBetweenTwoConstant.x, minSpinAroundBetweenTwoConstant.y);
        maxSpeedOfSpinArdoun = Random.Range(maxSpinAroundBetweenTwoConstant.x, maxSpinAroundBetweenTwoConstant.y);
        //color setting 
        float randomSeed = Mathf.Sin(minSpeedOfSpinAround + maxSpeedOfSpinArdoun) *
                           Mathf.Cos(maxSpeedOfSpinArdoun - minSpeedOfSpinAround);
        
        centerColor = randomCenterColor.Evaluate(Mathf.PerlinNoise(index, randomSeed));
        cornerColor = randomCornerColor.Evaluate(Mathf.PerlinNoise(index * randomSeed, index));

        //Debug.Log(Mathf.PerlinNoise(index * Mathf.Sin(index), index * Mathf.Sin(index)));

        
    }

    void GeneratorCenter()
    {
        center = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        center.name = "Center";
        center.tag = "SpringJointWorm";

        center.GetComponent<MeshFilter>().mesh = mesh;
        center.GetComponent<MeshRenderer>().material = material;
        center.GetComponent<SphereCollider>().enabled = false;
        center.AddComponent<Rigidbody>();
        center.AddComponent<SpinAround>();


        Rigidbody rigidbody = center.GetComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        rigidbody.useGravity = false;

        center.transform.parent = this.transform;
        center.transform.localPosition = Vector3.zero;

        center.GetComponent<SpinAround>().minSpeed = minSpeedOfSpinAround;
        center.GetComponent<SpinAround>().maxSpeed = maxSpeedOfSpinArdoun;
        center.GetComponent<SpinAround>().direction = ((int)index % 2 == 0) ? 1:-1;
        center.GetComponent<Renderer>().material.SetColor("_TintColor", centerColor);

    }

    void GameObjectGenerator()
    {
        if (center == null) GeneratorCenter();
        for (int i = 0; i < frequency;i++)
        {
            float angle = (360 * i / (frequency)) * Mathf.Deg2Rad;
            float x = transform.position.x + radius * Mathf.Sin(angle);
            float y = transform.position.y + radius * Mathf.Cos(angle);

            GameObject corner = new GameObject();
            corner.name = "corner" + i;
            corner.transform.parent = this.transform;
            corner.transform.localScale *= transform.localScale.x;
            corner.transform.position = new Vector3(x,y,transform.position.z);

            //add composnent
            corner.AddComponent<MeshFilter>();
            corner.AddComponent<MeshRenderer>();
            corner.AddComponent<Rigidbody>();
            corner.AddComponent<SphereCollider>();
            corner.AddComponent<SpringJoint>();

            //setting stage
            material = Instantiate(material);
            material.SetColor("_TintColor", cornerColor);
            corner.GetComponent<MeshFilter>().mesh = mesh;
            corner.GetComponent<MeshRenderer>().material = material;

            Rigidbody rigidbody = corner.GetComponent<Rigidbody>();
            rigidbody.mass = mass;
            rigidbody.drag = drag;
            rigidbody.constraints = constraints;
            rigidbody.useGravity = false;

            SpringJoint joint = corner.GetComponent<SpringJoint>();
            joint.connectedBody = center.GetComponent<Rigidbody>();
            joint.autoConfigureConnectedAnchor = true;
            joint.spring = 50;
            joint.damper = 5;

            //particles.Add(corner);
        }
        alreadyBuild = true;
    }

}
