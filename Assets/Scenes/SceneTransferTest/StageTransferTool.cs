using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
public class StageTransferTool : MonoBehaviour
{
    public Camera MainCamera;
    public PostProcessVolume postProcessing;

    public string nextSceneName;

    [Header("Time setting")]
    public float fadeInTime = 0.3f;
    public float fadeOutTime = 1f;
    public float gameStageTimeLength = 60;

    [Header("[Start]Vignette Animation setting (Blink)")]
    public Color vignetteColor = new Color(0,19,67,255);
    public AnimationCurve vignetteIntensity = AnimationCurve.EaseInOut(0,0.5f,1,0.3f);
    public AnimationCurve vignetteRoundness = AnimationCurve.EaseInOut(0,1f,1,0.9f);

    [Header("[Start]Color grading Attribte setting (Blink)")]
    public AnimationCurve ColorGrading_ColorFilter_Intensity = AnimationCurve.EaseInOut(0,0,1,1);
    public gameStage gameStageNow = gameStage.Stage1;
    public enum gameStage{
        Stage1,
        Stage2
    }
    public EndingScreenEffectAnimationKey EndingAnimationState = EndingScreenEffectAnimationKey.Start;
    public enum EndingScreenEffectAnimationKey
    {
        Start,
        Key1,
        Key2,
        End
    }
    // Start is called before the first frame update
    void Start()
    {
        attributeInitate();
        BlinkEffect();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void attributeInitate()
    {
        if(MainCamera == null)
        {
            MainCamera = Camera.main;
        }

        if(postProcessing == null)
        {
            postProcessing = MainCamera.GetComponent<PostProcessVolume>();
        }


    }

    public void SwitchScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
    
    public void BlinkEffect()
    {
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        Vignette vignette = null;
        ColorGrading colorGrading = null;
        postProcessing.profile.TryGetSettings(out vignette);
        postProcessing.profile.TryGetSettings(out colorGrading);

        for (float i = 0; i < fadeInTime; i+=Time.deltaTime)
        {
            vignette.intensity.value = vignetteIntensity.Evaluate(i/fadeInTime);
            vignette.roundness.value = vignetteRoundness.Evaluate(i/fadeInTime);

            colorGrading.colorFilter.value = new Color(
                ColorGrading_ColorFilter_Intensity.Evaluate(i/fadeInTime),
                ColorGrading_ColorFilter_Intensity.Evaluate(i/fadeInTime),
                ColorGrading_ColorFilter_Intensity.Evaluate(i/fadeInTime),0);
            yield return new WaitForEndOfFrame();
        }

    }
    
    IEnumerator EndingAnimation()
    {
        Vignette vignette = null;
        ColorGrading colorGrading = null;
        postProcessing.profile.TryGetSettings(out vignette);
        postProcessing.profile.TryGetSettings(out colorGrading);

        for (float i = 0; i < fadeInTime; i+=Time.deltaTime)
        {
            vignette.intensity.value = vignetteIntensity.Evaluate(1 - i/fadeInTime);
            vignette.roundness.value = vignetteRoundness.Evaluate(1- i/fadeInTime);

            colorGrading.colorFilter.value = new Color(
                ColorGrading_ColorFilter_Intensity.Evaluate(1 - i/fadeInTime),
                ColorGrading_ColorFilter_Intensity.Evaluate(1 - i/fadeInTime),
                ColorGrading_ColorFilter_Intensity.Evaluate(1 - i/fadeInTime),0);
            yield return new WaitForEndOfFrame();
        }

        

    }

    float cameraMotionTime = 2f;
    IEnumerator camerMotion()
    {
        Vignette vignette = null;
        postProcessing.profile.TryGetSettings(out vignette);
        //vignette.center.value = 
        for (float i = 0; i < cameraMotionTime; i+=Time.deltaTime)
        {
            float x =  Mathf.Lerp(0.5f,-0.5f,i/cameraMotionTime);
            vignette.center.value =new Vector2(x,vignette.center.value.y);
            yield return new WaitForEndOfFrame();
        }
        for (float i = 0; i < cameraMotionTime; i+=Time.deltaTime)
        {
            float x =  Mathf.Lerp(1.5f,0.5f,i/cameraMotionTime);
            vignette.center.value =new Vector2(x,vignette.center.value.y);
            yield return new WaitForEndOfFrame();
        }
        // yield return new WaitForEndOfFrame();
        // yield return new WaitForEndOfFrame();
        // yield return new WaitForEndOfFrame();
    }

}
