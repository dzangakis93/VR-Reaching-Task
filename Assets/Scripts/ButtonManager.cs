using UnityEngine;
using UnityEngine.UI;
using System;

public class ButtonManager : MonoBehaviour
{

    //Object setup
    public GameObject BLV;
    public GameObject BLNV;
    public GameObject CursorObject;
    public GameObject cam;
    public GameObject HomeObject;
    public GameObject EndObject;
    public GameObject MarkerObject;
    public GameObject LogicObject;
    CursorFunction cursor;
    PacketLogic packetLogic;
    Marker marker;

    [HideInInspector] public  float xCam;
    [HideInInspector] public  float yCam;
    [HideInInspector] public  float zCam;

    [HideInInspector] public  bool homeSet = false;
    [HideInInspector] public  bool endSet = false;
    public bool inPractice = true;
  
    private void Start()
    {
        marker = MarkerObject.GetComponent<Marker>();
        packetLogic = LogicObject.GetComponent<PacketLogic>();
        cursor = CursorObject.GetComponent<CursorFunction>();
    }
   
    public Text camButtonText;
    public float zOff;
    public void ZcamOffset(string z)
    {
        zOff = float.Parse(z);
    }
    public float yOff;
    public void YcamOffset(string y)
    {
        yOff = float.Parse(y);
    }
    public void SetCamPosition()
    {
        cam.transform.position = new Vector3(-marker.xCam, (marker.zCam-zOff), ((-marker.yCam)+yOff));
        camButtonText.text = "Camera Set!";  
    }
    //Functions for setting home and end positions 
    public Text homeButtonText;
    public Vector3 Home;
    public Vector3 End;
    public float optPath;
    public float homeX;
    public float homeY;
    public float homeZ;
    public void SetHomeFunction()
     {
        HomeObject.transform.position = new Vector3(-marker.xPos, marker.zPos, -marker.yPos);
        Home = new Vector3(-marker.xPos, marker.zPos, -marker.yPos);
        homeX = -marker.xPos;
        homeY = marker.zPos;
        homeZ = -marker.yPos;
        
        HomeObject.SetActive(true);
        homeSet = true;
        homeButtonText.text = "Home set!";
    }
    public Text endButtonText;
    public float endX;
    public float endY;
    public float endZ;
    public void SetEndFunction()
     {
        EndObject.transform.position = new Vector3(-marker.xPos, marker.zPos, -marker.yMID);
        End = new Vector3(-marker.xMID, marker.zPos, -marker.yMID);
        endX = -marker.xPos;
        endY = marker.zPos;
        endZ = -marker.yMID;
        endButtonText.text = "End set!";
        EndObject.SetActive(true);
        endSet = true;
        //calculate optimal path
        float targetDist = Vector3.Distance(End, Home);
        optPath = (float)(targetDist + 0.0875 - 0.035);
        Debug.Log(optPath);
    }
    //Setting Pause/Resume

    [HideInInspector] public bool x = false;
    public Text pauseButtonText;
    public void ButtonFunction()
    {
       if (x == false)
            PauseGame();
       else if (x == true)
            ResumeGame();
    }
    public void PauseGame()
    {
        GameObject cursorPause = GameObject.Find("Cursor");
        cursorPause.GetComponent<CursorFunction>().enabled = false;
           
        packetLogic.sendStartPacket = false;
        packetLogic.sendStopPacket = true;
           
        Time.timeScale = 0;
        pauseButtonText.text = "Resume";  
        x = true;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        GameObject cursorPause = GameObject.Find("Cursor");
        cursorPause.GetComponent<CursorFunction>().enabled = true;
        pauseButtonText.text = "Pause";
        if (cursor.isStarted == true && cursor.isFinished == false)
        {
            cursor.TrialReset();
            cursor.isStarted = false;
            cursor.isFinished = true;
        }
        //enter practice on resume if next set requires no vision
        if((TrialDataGUI.trialCounter>=40 && TrialDataGUI.trialCounter <= 260) || TrialDataGUI.practiceCounter ==-23)
        { 
            inPractice = true;
            //if practicecounter>=0 set to 0? this way trials are named 1,2,3 properly
        }
        x = false;
            
    }

    public string subjectName;
    public Text subjectNameText;
    public void ReadSubjectName(string s)
    {
        subjectName = s;
        Debug.Log(subjectName);
        subjectNameText.text = "Subject Name Set!";
    } 
    public void PracticeVis()
    {
        TrialDataGUI.practiceCounter = -43;
        inPractice = true;
    }
    public void PracticeNoVis()
    {
        TrialDataGUI.practiceCounter = -20;
        inPractice = true;
    }
    public void BaseVis()
    {
        TrialDataGUI.trialCounter = 0;
        cursor.finishedTrials = 0;
        inPractice = false;
        TrialDataGUI.practiceCounter = 1;
    }
    public void BaseNoVis()
    {
        TrialDataGUI.trialCounter = 40;
        cursor.finishedTrials = 40;
        inPractice =false;
        TrialDataGUI.practiceCounter = 1;
    }
    public void Int()
    {
        TrialDataGUI.trialCounter = 80;
        cursor.finishedTrials = 80;
        inPractice = false;
        TrialDataGUI.practiceCounter = 1;
    }
    public void RetVis()
    {
        TrialDataGUI.trialCounter = 280;
        cursor.finishedTrials = 280;
        inPractice = false;
        TrialDataGUI.practiceCounter = 1;
    }
    public void RetNoVis()
    {
        TrialDataGUI.trialCounter = 240;
        cursor.finishedTrials = 240;
        inPractice = false;
        TrialDataGUI.practiceCounter = 1;
    }
    public string session;
    public void InputSession(string z)
    {
        session = String.Format("Session{0}", z);
        Debug.Log(session);
    }

}

