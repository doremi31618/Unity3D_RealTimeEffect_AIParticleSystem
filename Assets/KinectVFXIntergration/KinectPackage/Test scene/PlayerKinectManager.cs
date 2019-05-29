using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using Windows.Kinect;


public class PlayerKinectManager : MonoBehaviour 
{
	[Tooltip("GUI-texture used to display the color camera feed on the scene background.")]
	public GUITexture backgroundImage;

	[Tooltip("Camera that will be used to overlay the 3D-objects over the background.")]
	public Camera foregroundCamera;
	
	[Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
	public int playerIndex = 0;
	
	[Tooltip("Game object used to overlay the left hand.")]

    public List<GameObject> PlayerGameObject;
    
	public Transform leftHandOverlay;

	[Tooltip("Game object used to overlay the left hand.")]
	public Transform rightHandOverlay;
	
	//public float smoothFactor = 10f;

	// reference to KinectManager
	private KinectManager manager;
	

	void Update () 
	{
		if(manager == null)
		{
			manager = KinectManager.Instance;
		}

		if(manager && manager.IsInitialized() && foregroundCamera)
		{
			//backgroundImage.renderer.material.mainTexture = manager.GetUsersClrTex();
			if(backgroundImage && (backgroundImage.texture == null))
			{
				backgroundImage.texture = manager.GetUsersClrTex();
			}

			// get the background rectangle (use the portrait background, if available)
			Rect backgroundRect = foregroundCamera.pixelRect;
			PortraitBackground portraitBack = PortraitBackground.Instance;

			if(portraitBack && portraitBack.enabled)
			{
				backgroundRect = portraitBack.GetBackgroundRect();
			}

			// overlay the joints
			if(manager.IsUserDetected())
			{
				long userId1P = manager.GetUserIdByIndex(playerIndex);
                long userId2P = manager.GetUserIdByIndex(1);


                //1P
                OverlayJoint(userId1P, (int)KinectInterop.JointType.HandLeft, leftHandOverlay, backgroundRect);
                OverlayJoint(userId1P, (int)KinectInterop.JointType.HandRight, rightHandOverlay, backgroundRect);
                


            }
			
		}
        if (Input.GetKeyDown(KeyCode.D)) 
        {
            //Debug.Log("playerIndex:" + playerIndex);
            Debug.Log("manager.GetUserIdByIndex(playerIndex):" + manager.GetUserIdByIndex(playerIndex));
            Debug.Log("BodyCount:"+manager.GetUserIndexById(manager.GetUserIdByIndex(playerIndex)));

        }
    }


	private void OverlayJoint(long userId, int jointIndex, Transform overlayObj, Rect backgroundRect)
	{
		if(manager.IsJointTracked(userId, jointIndex))
		{
			Vector3 posJoint = manager.GetJointKinectPosition(userId, jointIndex);
			
			if(posJoint != Vector3.zero)
			{
				// 3d position to depth
				Vector2 posDepth = manager.MapSpacePointToDepthCoords(posJoint);
				ushort depthValue = manager.GetDepthForPixel((int)posDepth.x, (int)posDepth.y);
				
				if(depthValue > 0)
				{
					// depth pos to color pos
					Vector2 posColor = manager.MapDepthPointToColorCoords(posDepth, depthValue);
					
					float xNorm = (float)posColor.x / manager.GetColorImageWidth();
					float yNorm = 1.0f - (float)posColor.y / manager.GetColorImageHeight();
					
					if(overlayObj && foregroundCamera)
					{
						float distanceToCamera = overlayObj.position.z - foregroundCamera.transform.position.z;
						posJoint = foregroundCamera.ViewportToWorldPoint(new Vector3(xNorm, yNorm, distanceToCamera));

						overlayObj.position = posJoint;
					}
				}
			}
		}
	}
}