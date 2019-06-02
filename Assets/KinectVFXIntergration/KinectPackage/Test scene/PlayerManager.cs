using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, KinectGestures.GestureListenerInterface
{
    KinectManager manager;
    public List<GameObject> PlayerList;
    public List<KinectSkeletonTracker> PlayerParticleEffect;
    public int usersSaved = 0;
    void Update()
    {
        if(manager == null)manager = KinectManager.Instance;
        switch( manager.GetUsersCount())
        {
            case 0:
                // for(int i = 1;i<PlayerList.Count;i++)
                // {
                //     PlayerList[i].SetActive(false);
                //     //PlayerParticleEffect[i].enabled = false;
                // }
                break;
            case 1:
                // for(int i = 1;i<PlayerList.Count;i++)
                // {
                //     PlayerList[i].SetActive(false);
                //     //PlayerParticleEffect[i].enabled = false;
                // }
                break;
            case 2:
                break;
            case 3:
                break;
        }
        // if(manager && manager.IsInitialized())
        // {
        //     int usersNow = manager.GetUsersCount();

        //     if(usersNow > usersSaved)
        //     {
        //         OndetectUser();
        //     }
        //     if(usersNow < usersSaved)
        //     {
        //         OnLostUser();
        //     }

        //     usersSaved = usersNow;
        // }
    }
    void OndetectUser(){
        
    }
    void OnLostUser(){

    }
    
    public void UserDetected(long userId, int userIndex){
        Debug.Log("User detect");
        if(userIndex != 0)
        {
             PlayerList[userIndex].SetActive(true);
        }
        PlayerParticleEffect[userIndex].enabled = (true);
        PlayerParticleEffect[userIndex].PlayerIndex = userIndex;
        PlayerList[userIndex].GetComponent<PlayerData>().userID = userId;
        PlayerList[userIndex].GetComponent<PlayerData>().userIndex = userIndex;
        PlayerList[userIndex].GetComponent<PlayerData>().ResetPalayerParticlePosition();
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
                                  float progress, KinectInterop.JointType joint, Vector3 screenPos){}
    public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture,
                                  KinectInterop.JointType joint, Vector3 screenPos)
    {
        return true;
    }
    public bool GestureCancelled(long userId, int userIndex, KinectGestures.Gestures gesture,
                                  KinectInterop.JointType joint)
    {
        return true;
    }
}
