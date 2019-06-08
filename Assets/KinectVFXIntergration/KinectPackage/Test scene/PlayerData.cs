using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public long userID;
    public int userIndex;
    public GameObject playerHand;
    public GameObject playerParticle;

    private KinectManager manager;
    
    
    public void ResetPalayerParticlePosition(KinectInterop.JointType jointType)
    {
        if(manager == null)
		{
			manager = KinectManager.Instance;
		}
        if(manager && manager.IsInitialized())
		{
            playerHand.GetComponent<PlayerKinectManager>().OverlayJoint(
                userID,
                (int)KinectInterop.JointType.Head,
                playerParticle.transform);
            
        }
    }
}
