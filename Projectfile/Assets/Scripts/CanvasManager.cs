using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject startbutt;
    public GameObject endbutt;
    public GameObject optionbutt;
    public GameObject mutemusic;
    public GameObject muteeffect;
    public GameObject inbattlecv;
    public GameObject backbutt;
    public GameObject menucv;
    public GameObject Levelcv;
    public GameObject playerhwcv;
    public GameObject BGCanvas;
    public int LevelChecker = 0;

    public void Start()
    {
        inbattlecv.SetActive(false);
        menucv.SetActive(true);
        startbutt.SetActive(true);
        endbutt.SetActive(true);
        optionbutt.SetActive(true);
    }
    public void StartGame()
    {
        menucv.SetActive(false);
        Levelcv.SetActive(true);
    }
    public void LoadScene()
    {
        LevelChecker = 1;
        SceneManager.LoadScene(1);
    }
    public void SelectStageFinish()
    {
        Levelcv.SetActive(false);
        inbattlecv.SetActive(true);
    }
    public void optionmanager()
    {
        muteeffect.SetActive(true);
        mutemusic.SetActive(true);
        backbutt.SetActive(true);
    } 
    public void Quit() 
    {
        Application.Quit();
    }
}
