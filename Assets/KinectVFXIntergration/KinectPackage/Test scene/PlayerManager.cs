using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, KinectGestures.GestureListenerInterface
{
    KinectManager manager;
    private static ModelGestureListener instance = null;

    public static ModelGestureListener Instance
	{
		get
		{
			return instance;
		}
	}
    public List<GameObject> PlayerList;
    public List<KinectSkeletonTracker> PlayerParticleEffect;
    public int usersSaved = 0;
    void Update()
    {
        if(manager == null)manager = KinectManager.Instance;
        // switch( manager.GetUsersCount())
        // {
        //     case 0:
        //         for(int i = 1;i<PlayerList.Count;i++)
        //         {
        //             PlayerList[i].SetActive(false);
        //             //PlayerParticleEffect[i].enabled = false;
        //         }
        //         break;
        //     case 1:
        //         for(int i = 1;i<PlayerList.Count;i++)
        //         {
        //             PlayerList[i].SetActive(false);
        //             //PlayerParticleEffect[i].enabled = false;
        //         }
        //         break;
        //     case 2:
        //         break;
        //     case 3:
        //         break;
        // }
        if(manager && manager.IsInitialized())
        {
            int usersNow = manager.GetUsersCount();

            if(usersNow > usersSaved)
            {
                OndetectUser();
            }
            if(usersNow < usersSaved)
            {
                OnLostUser();
            }

            usersSaved = usersNow;
        }
    }
    void OndetectUser(){
        
    }
    void OnLostUser(){

    }
    
    public void UserDetected(long userId, int userIndex){
        Debug.Log("User detect");
        PlayerParticleEffect[userIndex].enabled = (true);
        PlayerParticleEffect[userIndex].PlayerIndex = userIndex;
        PlayerParticleEffect[userIndex].Idle = false;
        StartCoroutine(PlayerParticleEffect[userIndex].Timer());
        
        PlayerList[userIndex].SetActive(true);
        PlayerList[userIndex].GetComponent<PlayerData>().userID = userId;
        PlayerList[userIndex].GetComponent<PlayerData>().userIndex = userIndex;
        PlayerList[userIndex].GetComponent<PlayerData>().ResetPalayerParticlePosition();

        manager.DetectGesture(userId, KinectGestures.Gestures.Jump);
		manager.DetectGesture(userId,KinectGestures.Gestures.Tpose);
        manager.DetectGesture(userId,KinectGestures.Gestures.SwipeRight);
        manager.DetectGesture(userId,KinectGestures.Gestures.SwipeLeft);

    }
    public void UserLost(long userId, int userIndex){
        Debug.Log("user lost");
        foreach(GameObject user in PlayerList)
        {
            long _userId = user.GetComponent<PlayerData>().userID;
            int _userIndex = user.GetComponent<PlayerData>().userIndex;

            if(_userId == userId)
            {
                //Debug.Log(userIndex);
                if((manager.GetUsersCount() == 1 ))
                {
                    user.GetComponent<PlayerData>().userID = -1;
                    PlayerParticleEffect[userIndex].enabled = false;
                    PlayerList.Insert(0,user);
                    return;
                }
                else if((manager.GetUsersCount() > 1 ))
                {
                    PlayerList.Remove(user);
                    PlayerList.Add(user);

                    user.GetComponent<PlayerData>().userID = -1;
                    user.GetComponent<PlayerData>().userIndex = PlayerList.Count - 1;
                    PlayerParticleEffect[userIndex].enabled = false;
                    user.SetActive(false);

                    break;
                }
                
            }
        }
    }
    public void PlayerGestureDetect(int userIndex, KinectGestures.Gestures gesture){}
    public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture,
                                  float progress, KinectInterop.JointType joint, Vector3 screenPos){

           if(progress >0.5f &&(gesture == KinectGestures.Gestures.Tpose || 
              gesture == KinectGestures.Gestures.Jump
           ||gesture == KinectGestures.Gestures.SwipeRight ||
           gesture == KinectGestures.Gestures.SwipeLeft ||
           gesture == KinectGestures.Gestures.RaiseRightHand
         ))
		{
            
            KinectSkeletonTracker vfx = PlayerParticleEffect[userIndex];
            Color m_color = vfx.m_Color;
            Color _color = new Color(m_color.r, m_color.g, m_color.b, 1);
            vfx.changeColor(_color);
            vfx.Idle = false;
            StartCoroutine(vfx.Timer());
             PlayerList[userIndex].GetComponent<PlayerData>().ResetPalayerParticlePosition();
            Debug.Log("Get complete gesture id : " + userId);
            return;
        }
                                  }
    public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture,
                                  KinectInterop.JointType joint, Vector3 screenPos)
    {
        
       if((gesture == KinectGestures.Gestures.Tpose || 
              gesture == KinectGestures.Gestures.Jump
           ||gesture == KinectGestures.Gestures.SwipeRight ||
           gesture == KinectGestures.Gestures.SwipeLeft ||
           gesture == KinectGestures.Gestures.RaiseRightHand
         ))
		{
            
            KinectSkeletonTracker vfx = PlayerParticleEffect[userIndex];
            Color m_color = vfx.m_Color;
            Color _color = new Color(m_color.r, m_color.g, m_color.b, 1);
            vfx.changeColor(_color);
            vfx.Idle = false;
            StartCoroutine(vfx.Timer());
            Debug.Log("Get complete gesture id : " + userId);   
             PlayerList[userIndex].GetComponent<PlayerData>().ResetPalayerParticlePosition();
            return true;
        }
        return false;
        
    }
    public bool GestureCancelled(long userId, int userIndex, KinectGestures.Gestures gesture,
                                  KinectInterop.JointType joint)
    {
        return true;
    }
}
