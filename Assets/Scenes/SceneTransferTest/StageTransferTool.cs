using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class StageTransferTool : MonoBehaviour
{

    [Header("Game Object setting")]
    public Camera MainCamera;
    public PostProcessVolume postProcessing;

    public Text TimerText;
    public GameObject Stage1;
    public GameObject Stage2;

    public bool isUseKinect;
    public GameObject noKinectPLayer;
    public GameObject kinectPlayer;
    private float timer;

    [Header("Time setting")]
    public float fadeInTime = 0.3f;
    public float fadeOutTime = 1f;
    public float Stage1TimeLength = 60;
    public float Stage2TimeLength = 60;
    public float EndingStageTimeLength = 30;
    public gameStage gameStageNow = gameStage.Stage1;

    [Header("[Start]Vignette Animation setting (Blink)")]
    public Color vignetteColor = new Color(0,19,67,255);
    public Color vignetteColor2 = new Color(0,19,67,255);
    public AnimationCurve vignetteIntensity = AnimationCurve.EaseInOut(0,0.5f,1,0.3f);
    public AnimationCurve vignetteRoundness = AnimationCurve.EaseInOut(0,1f,1,0.9f);

    [Header("[Start]Color grading Attribte setting (Blink)")]
    public AnimationCurve ColorGrading_ColorFilter_Intensity = AnimationCurve.EaseInOut(0,0,1,1);

    [Header("[Transfer]Vignette Animation setting")]
    public float cameraMotionTimeLength = 2f;
    public AnimationCurve cameraMotionTime1 = AnimationCurve.EaseInOut(0,0.5f,1,-0.1f);
    public AnimationCurve cameraMotionTime2 = AnimationCurve.EaseInOut(0,1.1f,1,0.5f);

    public GameObject EndingCanvas;
    
    public enum gameStage{
        Stage1,
        Stage2
    }
    
    // Start is called before the first frame update
    void Start()
    {
        //if(isFullScreen) Screen.fullScreen = true;
        SwitchPlayerControll();
        attributeInitate();
        StartCoroutine( GameStageManager());
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        TimerText.text = "Time : " + (int)timer;
<<<<<<< HEAD
<<<<<<< HEAD

<<<<<<< HEAD
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadSceneAsync(0);
        }
=======
>>>>>>> parent of 1c5d5b50... update
=======
        
>>>>>>> parent of f69532e1... Merge branch 'master' of https://github.com/doremi31618/AIParticleSystem
=======
>>>>>>> parent of 1c5d5b50... update
    }

    IEnumerator GameStageManager()
    {
        BlinkEffect();
        yield return new WaitForSeconds(Stage1TimeLength);
        if(gameStageNow == gameStage.Stage1)
            TransferEffect();
        yield return new WaitForSeconds(Stage2TimeLength);
        EndingCanvas.SetActive(true);
        Stage1.SetActive(false);
        Stage2.SetActive(false);
        yield return new WaitForSeconds(EndingStageTimeLength);
        
       
        SceneManager.LoadSceneAsync(0);
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

        timer = 0;
    }

    public void SwitchPlayerControll()
    {
        isUseKinect = !isUseKinect;
        if(!isUseKinect)
        {
            //switch to mouse
            noKinectPLayer.SetActive(true);
            kinectPlayer.SetActive(false);
            MainCamera = noKinectPLayer.GetComponent<PlayerObjectManager>().MainCamera;
            postProcessing = MainCamera.GetComponent<PostProcessVolume>();
        }
        else
        {
            //switch to kinect
            noKinectPLayer.SetActive(false);
            kinectPlayer.SetActive(true);
            MainCamera = kinectPlayer.GetComponent<PlayerObjectManager>().MainCamera;
            postProcessing = MainCamera.GetComponent<PostProcessVolume>();
        }
        ChangeBoundaryLayerCamera();
    }

    public void ChangeBoundaryLayerCamera()
    {
        switch(gameStageNow)
        {
            case gameStage.Stage1:
                foreach(ScreenSpaceBoundary layerBoundary in Stage1.GetComponentsInChildren<ScreenSpaceBoundary>())
                {
                    layerBoundary.cameraObject = MainCamera.gameObject;
                }
                break;
            case gameStage.Stage2:
             foreach(ScreenSpaceBoundary layerBoundary in Stage2.GetComponentsInChildren<ScreenSpaceBoundary>())
                {
                    layerBoundary.cameraObject = MainCamera.gameObject;
                }
                break;

        }
    }
    public void BlinkEffect()
    {
        gameStageNow = gameStage.Stage1;
        StartCoroutine(Blink());
        
    }
    public void TransferEffect()
    {
        gameStageNow = gameStage.Stage2;
        StartCoroutine(camerMotion());
    }
    public void TimeMachine()
    {
        Time.timeScale = 10;
    }

    void stageObjectManager()
    {
        switch(gameStageNow)
        {
            case gameStage.Stage1:
                Stage1.SetActive(true);
                Stage2.SetActive(false);
                break;

            case gameStage.Stage2:
                Stage1.SetActive(false);
                Stage2.SetActive(true);
                break;
        }
    }
    IEnumerator Blink()
    {
        Vignette vignette = null;
        ColorGrading colorGrading = null;
        postProcessing.profile.TryGetSettings(out vignette);
        postProcessing.profile.TryGetSettings(out colorGrading);
        vignette.color.value = vignetteColor;
        stageObjectManager();
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


    
    IEnumerator camerMotion()
    {
        Vignette vignette = null;
        postProcessing.profile.TryGetSettings(out vignette);
        fadeInTime = 0.15f;
        // for (float i = 0; i < fadeInTime; i+=Time.deltaTime)
        // {
        //     vignette.intensity.value = vignetteIntensity.Evaluate(1 - i/fadeInTime);
        //     vignette.roundness.value = vignetteRoundness.Evaluate(1- i/fadeInTime);
        //     yield return new WaitForEndOfFrame();
        // }
        // vignette.rounded.value = true;
         vignette.color.value = vignetteColor2;
        for (float i = 0; i <cameraMotionTimeLength; i+=Time.deltaTime)
        {
            float x =  cameraMotionTime1.Evaluate(i/cameraMotionTimeLength);
            vignette.center.value =new Vector2(x,vignette.center.value.y);
            yield return new WaitForEndOfFrame();
        }

        stageObjectManager();
        for (float i = 0; i < cameraMotionTimeLength; i+=Time.deltaTime)
        {
            float x =  cameraMotionTime2.Evaluate(i/cameraMotionTimeLength);
            vignette.center.value =new Vector2(x,vignette.center.value.y);
            yield return new WaitForEndOfFrame();
        }
        // vignette.rounded.value = false;
        // for (float i = 0; i < fadeInTime; i+=Time.deltaTime)
        // {
        //     vignette.intensity.value = vignetteIntensity.Evaluate(i/fadeInTime);
        //     vignette.roundness.value = vignetteRoundness.Evaluate(i/fadeInTime);
        //     yield return new WaitForEndOfFrame();
        // }
    }

}
