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
    public Color color;

    [Header("Single Worm Physic Attribute Setting")]
    public float mass = 50;
    public float drag = 1.5f;
    public float spring = 50f;
    public RigidbodyConstraints constraints = RigidbodyConstraints.FreezeRotation;

    [Header("Moving Attribute")]
    public float speedOfSpinAround;

    bool alreadyBuild = false;
    GameObject center;
    //List<GameObject> particles;

	// Use this for initialization
	void Start () {
        GameObjectGenerator();
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

        center.GetComponent<SpinAround>().speed = speedOfSpinAround;

    }
    void GameObjectGenerator()
    {
        GeneratorCenter();
        for (int i = 0; i < frequency;i++)
        {
            float angle = (360 * i / (frequency)) * Mathf.Deg2Rad;
            float x = transform.position.x + radius * Mathf.Sin(angle);
            float y = transform.position.y + radius * Mathf.Cos(angle);

            GameObject corner = new GameObject();
            corner.name = "corner" + i;
            corner.transform.parent = this.transform;
            corner.transform.position = new Vector3(x,y,transform.position.z);

            //add composnent
            corner.AddComponent<MeshFilter>();
            corner.AddComponent<MeshRenderer>();
            corner.AddComponent<Rigidbody>();
            corner.AddComponent<SphereCollider>();
            corner.AddComponent<SpringJoint>();

            //setting stage
            material = Instantiate(material);
            material.SetColor("_TintColor", color);
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
