using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControllMove : MonoBehaviour
{
    public GameObject controllObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MouseControlMove();
    }
    public void MouseControlMove()
    {
        //Vector3 point = new Vector3();
        Event currentEvent = Event.current;
        //Vector2 mousePos = new Vector2();
        ScreenSpaceBoundary m_Boundary = GetComponent<ScreenSpaceBoundary>();
        Vector3 mousePointToPlayerLayer = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x,
                        Input.mousePosition.y,
                        m_Boundary.distanceToCamera));
        controllObject.transform.position = mousePointToPlayerLayer;
        //m_rigidbody.MovePosition(mousePointToPlayerLayer);
    }
}
