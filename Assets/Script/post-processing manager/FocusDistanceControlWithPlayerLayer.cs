using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.PostProcessing;
public class FocusDistanceControlWithPlayerLayer : MonoBehaviour {
    public ScreenSpaceBoundary playerLayerBoundary;
    public float changingFrequency = 5f;
    PostProcessVolume volume;
	// Use this for initialization
	void Start () {
        StartCoroutine(changeFocusDistance());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    IEnumerator changeFocusDistance()
    {
        while(true)
        {
            ControlFocusDistance();
            yield return new WaitForSeconds(changingFrequency);
        }
    }
    void ControlFocusDistance()
    {
        volume = gameObject.GetComponent<PostProcessVolume>();
        DepthOfField depthOfField = null;
        volume.profile.TryGetSettings(out depthOfField);
        depthOfField.focusDistance.value = ReadPlayerLayerDistance();
    }
    float ReadPlayerLayerDistance()
    {
        float distanceOfPlayerLayer = 0;
        distanceOfPlayerLayer = playerLayerBoundary.distanceToCamera;
        return distanceOfPlayerLayer;
    }
}
