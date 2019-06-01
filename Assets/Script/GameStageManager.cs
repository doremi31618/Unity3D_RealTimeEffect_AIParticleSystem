using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{
    Start,
    Update,
    end
}
public class GameStageManager : MonoBehaviour
{
    public GameState m_GameState; 
    [Header("Time setting")]
    [Tooltip("it means update time length")]
    public float stageTimeLength = 60f;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    IEnumerator GameStageControl()
    {
        yield return new WaitForEndOfFrame();
    }
}
