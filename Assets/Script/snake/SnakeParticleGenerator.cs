using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/// <summary>
/// tip : need change the project setting collision let same tag object wont happen collision
/// </summary>
public class SnakeParticleGenerator : MonoBehaviour {
    
    public GameObject headPrefab;
    public GameObject bodyPrefab;

    [Tooltip("only affect at fisrt time(start)")]public int numberOfBody = 5;
    [Tooltip("change by realtime")]public float bodyDistance;

    public float speed = 1;
    public Color headColor;
    public Color bodyColor;



    public List<GameObject> bodyList = new List<GameObject>();

	// Use this for initialization
	void Start () {
        SnakeGenerator();
	}
	
	// Update is called once per frame
	void Update () {
        SnakeMove();
	}

    void SnakeGenerator()
    {
        HeadGenerator();
        BodyGenerator();
    }

    void HeadGenerator()
    {
        GameObject Head = (Instantiate(headPrefab) as GameObject);
        Head.transform.parent = this.transform;
        Head.transform.position = this.transform.position;
        Head.GetComponent<Renderer>().material.SetColor("_TintColor", headColor);
        //Snake layer
        Head.layer = 13;
        bodyList.Add(Head);

    }
    void BodyGenerator()
    {
        
        for (int i = 1; i <= numberOfBody;i++)
        {
            GameObject body = (Instantiate(bodyPrefab) as GameObject);
            body.transform.parent = this.transform;
            body.transform.position = bodyList[0].transform.position;
            body.GetComponent<Renderer>().material.SetColor("_TintColor", bodyColor);
            body.layer = 13;

            if(bodyList[i-1] == null)
            {
                HeadGenerator();
            }
            bodyList.Add(body);
        }
    }

    void SnakeMove()
    {
        for (int i = 0; i < bodyList.Count;i++)
        {
            if(i == 0)
            {
                HeadMove(bodyList[i]);
            }else{
                CheckDistanceWithForward(bodyList[i - 1], bodyList[i]);
            }
        }
    }
    void CheckDistanceWithForward(GameObject forward, GameObject current)
    {
        float _distance = Vector3.Distance(forward.transform.position, current.transform.position);
        Vector3 dir = (forward.transform.position - current.transform.position).normalized;
        float t = Time.deltaTime * _distance / bodyDistance * speed;
        if (t > 0.5) t = 0.5f;
        current.GetComponent<Rigidbody>().MovePosition(
                Vector3.Slerp(current.transform.position, forward.transform.position, t));
        
    }

    void HeadMove(GameObject _head)
    {
        float moveX = speed * Time.deltaTime;
        float moveY = Mathf.Sin(Time.time ) * speed * Time.deltaTime;
        Vector3 velocity = new Vector3(moveX, moveY,0);
        _head.GetComponent<Rigidbody>().MovePosition(_head.transform.position + velocity);
    }

    void AddBody()
    {
        GameObject body = (Instantiate(bodyPrefab) as GameObject);
        body.transform.parent = this.transform;
        body.transform.position = bodyList[0].transform.position;
        body.layer = 13;
        bodyList.Add(body);
    }



}
