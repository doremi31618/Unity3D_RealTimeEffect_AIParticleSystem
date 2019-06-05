using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;
using Windows.Kinect;
using KinectJoint = Windows.Kinect.Joint;

public class KinectSkeletonTracker : MonoBehaviour
{
    [SerializeField]
    private VisualEffect visualEffect;
    public Color m_Color;
    private KinectSensor sensor;
    private BodyFrameReader bodyFrameReader;
    private Body[] bodies;
    public ulong trackedId = 0;
    public int PlayerIndex;
    
    KinectManager manager;

    public bool Idle = true;
	public float howLongToTransparent = 30;
	public float transparentLerpTime = 2f;

    private Dictionary<JointType, JointType> boneMap = new Dictionary<JointType, JointType>() {

        {JointType.FootLeft, JointType.AnkleLeft },
        {JointType.AnkleLeft, JointType.KneeLeft },
        {JointType.KneeLeft, JointType.HipLeft },
        {JointType.HipLeft, JointType.SpineBase },

        {JointType.FootRight, JointType.AnkleRight },
        {JointType.AnkleRight,JointType.KneeRight     },
        {JointType.KneeRight,JointType.HipRight },
        {JointType.HipRight,JointType.SpineBase },

        {JointType.HandLeft, JointType.WristLeft },
        {JointType.WristLeft,JointType.ElbowLeft },
        {JointType.ElbowLeft,JointType.ShoulderLeft },
        {JointType.ShoulderLeft,JointType.SpineShoulder },

        {JointType.HandRight,JointType.WristRight },
        {JointType.WristRight,JointType.ElbowRight },
        {JointType.ElbowRight,JointType.ShoulderRight },
        {JointType.ShoulderRight,JointType.SpineShoulder },

        {JointType.SpineBase,JointType.SpineMid },
        {JointType.SpineMid,JointType.SpineShoulder },
        {JointType.SpineShoulder,JointType.Head }
    };
    public IEnumerator TransparentTimer()
	{
        Debug.Log("transparent [id] : " + trackedId);
		for(float i = 0;i<transparentLerpTime;i+=Time.deltaTime)
		{

			if(Idle)
			{
				m_Color = Color.Lerp(new Color(m_Color.r,m_Color.g,m_Color.b,1),
									  new Color(m_Color.r,m_Color.g,m_Color.b,0),
									  i/transparentLerpTime);
                //Debug.Log("i : "+ m_Color);
                 
			}
			else
			{
				m_Color = new Color(m_Color.r,m_Color.g,m_Color.b,1);
                Debug.Log("Break");
				break;
			}
           
            yield return new WaitForEndOfFrame();
		}
		
	}
    public IEnumerator Timer()
	{
        
		//Debug.Log("start transparent timer[id] : " + trackedId);
		yield return new WaitForSeconds(howLongToTransparent);
		Idle = true;
        StartCoroutine(TransparentTimer());

		
	}
    public void changeColor(Color _color)
    {
        m_Color = _color;
        visualEffect.SetVector4("Color",m_Color );
    }
    private Dictionary<string, JointType> jointMap = new Dictionary<string, JointType>()
    {
        { "Spine Base", JointType.SpineBase },
        { "Spine Mid", JointType.SpineMid },
        { "Spine Shoulder", JointType.SpineShoulder },
        { "Head", JointType.Head },

        { "Foot Left", JointType.FootLeft },
        { "Ankle Left", JointType.AnkleLeft },
        { "Knee Left", JointType.KneeLeft },
        { "Hip Left", JointType.HipLeft },

        { "Foot Right", JointType.FootRight },
        { "Ankle Right", JointType.AnkleRight },
        { "Knee Right", JointType.KneeRight },
        { "Hip Right", JointType.HipRight },

        { "Wrist Left", JointType.WristLeft },
        { "Elbow Left", JointType.ElbowLeft },
        { "Shoulder Left", JointType.ShoulderLeft },

        { "Wrist Right", JointType.WristRight },
        { "Elbow Right", JointType.ElbowRight },
        { "Shoulder Right", JointType.ShoulderRight }
    };

    // Start is called before the first frame update
    void Start()
    {
        sensor = KinectSensor.GetDefault();
        manager = KinectManager.Instance;
        if (sensor != null)
        {
            bodyFrameReader = sensor.BodyFrameSource.OpenReader();

            if (!sensor.IsOpen)
            {
                sensor.Open();
            }
        }
        visualEffect.SetVector4("Color",m_Color );
    }

    // Update is called once per frame
    void Update()
    {

        visualEffect.SetVector4("Color",m_Color );
        if (bodyFrameReader != null)
        {
            var frame = bodyFrameReader.AcquireLatestFrame();

            if (frame != null)
            {
                if (bodies == null)
                {
                    bodies = new Body[frame.BodyCount];
                }
                
                frame.GetAndRefreshBodyData(bodies);
                frame.Dispose();
                frame = null;

                List<ulong> trackedIds = new List<ulong>();
                foreach (var body in bodies)
                {
                    if (body == null) continue;

                    if (body.IsTracked)
                    {
                        trackedIds.Add(body.TrackingId);
                    }
                }

                if (!trackedIds.Contains(trackedId))
                {
                    if (trackedIds.Count > 0)
                    {
                        trackedId = (ulong)manager.GetUserIdByIndex(PlayerIndex);
                    }
                }

                foreach (var body in bodies)
                {
                    if (body.TrackingId == trackedId)
                    {
                        foreach (var joint in jointMap)
                        {
                            visualEffect.SetVector3(joint.Key, GetVector3FromJoint(body.Joints[joint.Value]));
                        }
                        break;
                    }
                }
                // numberOfPlayer = trackedIds.Count -1;
            }
        }
    }

    private static Vector3 GetVector3FromJoint(KinectJoint joint)
    {
        return new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
    }

    private void OnDestroy()
    {
        if (bodyFrameReader != null)
        {
            bodyFrameReader.Dispose();
            bodyFrameReader = null;
        }
    }
}
