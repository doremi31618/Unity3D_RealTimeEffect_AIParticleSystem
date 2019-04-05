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
    bool UseDebugVisualizer = false;

    [Header("Radius")]
    [Range(0.5f, 20f)] public float attractiveRadius = 10f;
    [Range(0.5f, 20f)] public float triggerRadius = 2f;
    [Range(0.5f, 20f)] public float explosionRadius = 5f;
    [Range(0.5f, 20f)]public float deathRadius = 0.5f;

    [Header("Time Setting")]
    public float explosionTime = 0.3f;
    public float attractiveTime = 10f;
    private float timer;

    //計算吸收數量
    public int substractNumber = 0;
    public int HowManyToGrow = 20;

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
        GetComponent<Star>().frequency += 1;
        StartCoroutine(StageControl());
	}
	
	// Update is called once per frame
	void Update () {
        if(substractNumber>HowManyToGrow)
        {
            GetComponent<Star>().frequency += 1;
            GetComponent<Star>().Reset();
            //substractNumber = 0;
        }
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

        if(!UseDebugVisualizer)
        {
            TirggerVidsualizer.SetActive(false);
            ExplosionVisualizer.SetActive(false);
        }
    }

    void AttributeInitialize()
    {
        switch (m_stage)
        {
            //explosion stage
            case PlayerStage.Beginning:
                m_collider.radius = explosionRadius/2;

                if(UseDebugVisualizer)
                {
                    ExplosionVisualizer.SetActive(true);
                    TirggerVidsualizer.SetActive(false);
                    ExplosionVisualizer.transform.localScale = new Vector3(explosionRadius, explosionRadius, explosionRadius);
                }

               
                break;

            //Attractive stage
            case PlayerStage.update:
                m_collider.radius = attractiveRadius/2;
                substractNumber = 0;

                if(UseDebugVisualizer)
                {
                    ExplosionVisualizer.SetActive(false);
                    TirggerVidsualizer.SetActive(true);
                    TirggerVidsualizer.transform.localScale = new Vector3(attractiveRadius, attractiveRadius, attractiveRadius);
                }


                break;

            case PlayerStage.end:
                break;
                            
        }
    }

    IEnumerator StageControl()
    {
        Initialize();
        while(true)
        {
            timer = 0;
            switch(m_stage)
            {
                case PlayerStage.Beginning:
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
                    while (timer / attractiveTime < 1)
                    {
                        timer += Time.deltaTime;
                        yield return new WaitForEndOfFrame();
                    }
                    m_stage = PlayerStage.Beginning;
                    break;

                case PlayerStage.end:
                    break;
            }
        }

    }

    //debug use function 
    private void OnMouseEnter()
    {
        m_stage = PlayerStage.Beginning;
        Debug.Log("On Mouse Enter");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "LittleParticle" && other.GetComponent<ParticleLifeCycle>().m_stage == mainParticleStage.update)
        {
            Rigidbody pRigbody = other.GetComponent<Rigidbody>();
            Vector3 Force;
            float distance = Vector3.Distance(other.transform.position, this.transform.position);

            switch (m_stage)
            {
                case PlayerStage.Beginning:
                    
                    Force = (other.transform.position - this.transform.position).normalized * 
                        Mathf.Lerp(0,RepulsiveForce,distance/triggerRadius);
                    
                    pRigbody.AddForce(Force);
                    break;

                case PlayerStage.update:
                    if(distance < deathRadius){
                        substractNumber += 1;
                        other.GetComponent<ParticleLifeCycle>().ExitBoundary();

                        break;
                    }

                    Force = (this.transform.position - other.transform.position).normalized *
                            Mathf.Lerp(AttrativeForce,AttrativeForce/2 ,distance / triggerRadius);
                    
                    Color firstColor = other.GetComponent<ParticleLifeCycle>().fisrtColor;
                    Color finalColor = other.GetComponent<ParticleLifeCycle>().finalColor;
                    Color newColor = Color.Lerp(firstColor, finalColor, 1-distance / triggerRadius);

                    //Debug.Log(1 - distance / triggerRadius);
                    other.GetComponent<MeshRenderer>().material.SetColor("_TintColor", newColor);
                    pRigbody.AddForce(Force);
                    break;

                case PlayerStage.end:
                    break;
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "LittleParticle")
        {
            other.GetComponent<MeshRenderer>().material.SetColor("_TintColor", other.GetComponent<ParticleLifeCycle>().fisrtColor);
        }
    }

}
