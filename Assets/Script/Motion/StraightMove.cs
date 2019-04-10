using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightMove : ParticleBase {
    int index = 1;
    public float localMoveSpeed = 0.5f;
    Rigidbody m_rigidbody;
    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        Run();
        OutputValue();
    }
    void Run()
    {

        //direction += new Vector3(
        //    Mathf.Sin(Mathf.Rad2Deg * Random.Range(-Mathf.PI, Mathf.PI) * Mathf.PerlinNoise(Time.time + index, index)),
        //    Mathf.Cos(Mathf.Rad2Deg * Random.Range(-Mathf.PI, Mathf.PI) * Mathf.PerlinNoise(Time.time + index, index)), 0);

        //direction = direction.normalized;
        //direction.Set(direction.x, direction.y, 0);
        direction = Vector3.right;
        velocity = Mathf.PerlinNoise(Time.time + index, index) * direction * localMoveSpeed * Time.deltaTime;

    }
    void OutputValue()
    {

        //m_rigidbody.velocity = velocity;
        m_rigidbody.MovePosition(transform.position + velocity);
        //m_rigidbody.AddForce(velocity);
        m_rigidbody.angularVelocity = angVelocity;

    }
}
