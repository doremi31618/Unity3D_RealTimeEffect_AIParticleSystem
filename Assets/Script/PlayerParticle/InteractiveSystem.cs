using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveSystem : MonoBehaviour {

    [Header("Force")]
    public float RepulsiveForce = 100f;
    public float AttrativeForce = 10f;

    [Header("VisualizerObject")]
    public GameObject ExplosionVisualizer;
    public GameObject TirggerVidsualizer;
    public GameObject ColliderObject;

    [Header("Radius")]
    [Range(0.5f, 5f)] public float attractiveRadius = 1.5f;
    [Range(0.5f, 5f)] public float triggerRadius = 2f;
    [Range(0.5f, 5f)] public float explosionRadius = 5f;
    private float m_triggerRadius;

    [Header("Time Setting")]
    public float explosionTime = 0.3f;
    private float timer;

    Rigidbody m_rigidbody;
    SphereCollider m_collider;

    public PlayerStage m_stage = PlayerStage.Beginning;
    public enum PlayerStage
    {
        Beginning,
        update,
        end
    }

	// Use this for initialization
	void Start () {
        Initialize();
        //m_stage = PlayerStage.Beginning;
        //AttributeInitialize();
        StartCoroutine(StageControl());
	}
	
	// Update is called once per frame
	void Update () {
        
	}
    private void Initialize()
    {
        //component setting
        m_collider = this.GetComponent<SphereCollider>();
        m_rigidbody = this.GetComponent<Rigidbody>();

        //game object setting
        if (ExplosionVisualizer == null)
        {
            GameObject _ExplosionVisualizer = new GameObject();
            _ExplosionVisualizer.name = "_ExplosionVisualizer";
            _ExplosionVisualizer.AddComponent<MeshFilter>();
            _ExplosionVisualizer.AddComponent<MeshRenderer>();
            _ExplosionVisualizer.transform.localScale = Vector3.one * explosionRadius;

            ExplosionVisualizer = _ExplosionVisualizer;
        }
        else
        {
            ExplosionVisualizer.transform.localScale = Vector3.one * explosionRadius;

        }

        if (TirggerVidsualizer == null)
        {
            GameObject _TirggerVidsualizer = new GameObject();
            _TirggerVidsualizer.name = "_ExplosionVisualizer";
            _TirggerVidsualizer.AddComponent<MeshFilter>();
            _TirggerVidsualizer.AddComponent<MeshRenderer>();
            _TirggerVidsualizer.transform.localScale = Vector3.one * triggerRadius / 2;

            TirggerVidsualizer = _TirggerVidsualizer;
        }
        else
        {
            TirggerVidsualizer.transform.localScale = Vector3.one * triggerRadius;
        }

        if (ColliderObject == null)
        {
            GameObject _ColliderObject = new GameObject();
            _ColliderObject.name = "_ExplosionVisualizer";
            _ColliderObject.AddComponent<MeshFilter>();
            _ColliderObject.AddComponent<MeshRenderer>();
            _ColliderObject.transform.localScale = Vector3.one * triggerRadius;

            ColliderObject = _ColliderObject;
        }
        else
        {
            ColliderObject.transform.localScale = this.transform.localScale;
        }
    }

    void AttributeInitialize()
    {
        switch (m_stage)
        {
            case PlayerStage.Beginning:
                m_collider.radius = explosionRadius/2;
                ExplosionVisualizer.SetActive(true);
                TirggerVidsualizer.SetActive(false);
                break;

            case PlayerStage.update:
                m_collider.radius = triggerRadius/2;
                ExplosionVisualizer.SetActive(false);
                TirggerVidsualizer.SetActive(true);
                break;

            case PlayerStage.end:
                break;
                            
        }
    }

    void AttributeUpdate()
    {
        
    }


    void TriggerRadiusSetting()
    {
        m_triggerRadius = triggerRadius * transform.localScale.x;

    }

    IEnumerator StageControl()
    {
        while(true)
        {
            
            switch(m_stage)
            {
                case PlayerStage.Beginning:
                    timer = 0;
                    AttributeInitialize();
                    while(timer/explosionTime<1)
                    {
                        timer += Time.deltaTime;

                        yield return new WaitForEndOfFrame(); 
                    }
                    m_stage = PlayerStage.update;
                    break;

                case PlayerStage.update:
                    AttributeInitialize();
                    break;
                case PlayerStage.end:
                    break;
            }

            //m_rigidbody.AddExplosionForce(1000, this.transform.position, 2, 0);

            yield return new WaitForEndOfFrame(); 
        }

        //m_stage = PlayerStage.update;
        //while(true)
        //{
            
        //    yield return new WaitForEndOfFrame(); 
        //}

    }

    private void OnMouseDown()
    {
        m_stage = PlayerStage.Beginning;
    }
    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "LittleParticle")
        {
            switch (m_stage)
            {
                case PlayerStage.Beginning:
                    Rigidbody pRigbody = other.GetComponent<Rigidbody>();
                    Vector3 Force;

                    Force = (other.transform.position - this.transform.position).normalized * 
                            Mathf.Lerp(0,RepulsiveForce,
                                       Vector3.Distance(other.transform.position,this.transform.position)/m_triggerRadius);
                    
                    pRigbody.AddForce(Force);
                    break;
                case PlayerStage.update:
                    break;
                case PlayerStage.end:
                    break;
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        
    }

}
