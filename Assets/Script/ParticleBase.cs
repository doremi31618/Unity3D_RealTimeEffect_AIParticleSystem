using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ParticleBase : MonoBehaviour{
    [HideInInspector]public float mass;
    public float lifeTime;

    [HideInInspector]public Vector3 direction;
    //[HideInInspector]public Vector3 position;
    [HideInInspector]public Vector3 velocity;
    [Range(0, 100)] public float maxMoveSpeed = 100;
    public float _lerpTime = 2;
    public bool isBeaten = false;
    [HideInInspector]public Vector3 angVelocity;
    //public float lerpTime = 1f;
    private void Update()
    {
        if(GetComponent<Rigidbody>().velocity.magnitude > maxMoveSpeed)
        {
            GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * maxMoveSpeed;
        }
    }

    public void BeEaten()
    {
        isBeaten = true;
        //Color now = GetComponent<MeshRenderer>().material.GetColor("_TintColor");
        //GetComponent<Collider>().enabled = false;
        //GetComponent<MeshRenderer>().material.SetColor("_TintColor", new Vector4(now.r, now.g, now.b, 0));
    }
    public void FadeIn()
    {
        isBeaten = false;
        //Debug.Log("start fade in ");
        StartCoroutine(fadein());
    }
    public IEnumerator fadein()
    {
        Color nowTransparent = GetComponent<Renderer>().material.GetColor("_TintColor");
        Color now = new Vector4(nowTransparent.r, nowTransparent.g, nowTransparent.b, 1f);

        GetComponent<MeshRenderer>().material.SetColor("_TintColor", nowTransparent);
        for (float i = 0; i < _lerpTime; i+=Time.deltaTime)
        {
            Color _c = Color.Lerp(nowTransparent,now , i / _lerpTime);
            //Debug.Log("fade in" + _c);
            GetComponent<MeshRenderer>().material.SetColor("_TintColor", _c);
            yield return new WaitForFixedUpdate();
        }
        GetComponent<Collider>().enabled = true;

    }
}
