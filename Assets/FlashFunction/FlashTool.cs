using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashTool : MonoBehaviour
{
    public float flashInterval = 0.5f;
    Image m_img;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Flash());
    }
    
    IEnumerator Flash()
    {
        Image m_img = GetComponent<Image>();
        Color m_color = new Color( m_img.color.r,m_img.color.g,m_img.color.b,m_img.color.a);
        Color m_Color_transparent = new Color(m_color.r,m_color.g,m_color.b,0);
        while(true)
        {
            //alpha now to 0
            for(float i = 0;i<flashInterval;i+=Time.deltaTime)
            {
                m_img.color = Color.Lerp(m_color,m_Color_transparent,i/flashInterval);
                yield return new WaitForEndOfFrame();
            }
            //alpha 0 to now
            for(float i = 0;i<flashInterval;i+=Time.deltaTime)
            {
                m_img.color = Color.Lerp(m_Color_transparent,m_color,i/flashInterval);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
