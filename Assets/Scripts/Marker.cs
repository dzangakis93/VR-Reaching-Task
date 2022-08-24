using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViconDataStreamSDK.CSharp;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using System;

public class Marker : MonoBehaviour
{
    
    //Datastream setup
    //public ViconDataStreamClient Client;
    ViconDataStreamSDK.CSharp.Client MyClient = new ViconDataStreamSDK.CSharp.Client();
    public float pathLength = 0;
    Vector3 lastPosition;
    //get subject name from input field
    public GameObject buttonManager;
    ButtonManager button;
    public GameObject CursorObject;
    CursorFunction cursor;

    static string end = ".csv";

    public static string filePath = "";

    //Positional data setup
    [HideInInspector] public float xCam;
    [HideInInspector] public float yCam;
    [HideInInspector] public float zCam;
    //Fingertip marker position
    [HideInInspector] public float xPos;
    [HideInInspector] public float yPos;
    [HideInInspector] public float zPos;
    //segment info
    [HideInInspector] public float xMID;
    [HideInInspector] public float yMID;
    [HideInInspector] public float zMID;

    [HideInInspector] public float xMWRI;
    [HideInInspector] public float yMWRI;
    [HideInInspector] public float zMWRI;

    [HideInInspector] public float xLWRI;
    [HideInInspector] public float yLWRI;
    [HideInInspector] public float zLWRI;

    [HideInInspector] public float xPFORE;
    [HideInInspector] public float yPFORE;
    [HideInInspector] public float zPFORE;

    [HideInInspector] public float xDFORE;
    [HideInInspector] public float yDFORE;
    [HideInInspector] public float zDFORE;

    [HideInInspector] public float xLELB;
    [HideInInspector] public float yLELB;
    [HideInInspector] public float zLELB;

    [HideInInspector] public float xPUP;
    [HideInInspector] public float yPUP;
    [HideInInspector] public float zPUP;

    [HideInInspector] public float xDUP;
    [HideInInspector] public float yDUP;
    [HideInInspector] public float zDUP;

    [HideInInspector] public float xRSHO;
    [HideInInspector] public float yRSHO;
    [HideInInspector] public float zRSHO;

    [HideInInspector] public float xLSHO;
    [HideInInspector] public float yLSHO;
    [HideInInspector] public float zLSHO;

    [HideInInspector] public float xCLAV;
    [HideInInspector] public float yCLAV;
    [HideInInspector] public float zCLAV;

    [HideInInspector] public float xSTERN;
    [HideInInspector] public float ySTERN;
    [HideInInspector] public float zSTERN;

    [HideInInspector] public float xLASIS;
    [HideInInspector] public float yLASIS;
    [HideInInspector] public float zLASIS;

    [HideInInspector] public float xRASIS;
    [HideInInspector] public float yRASIS;
    [HideInInspector] public float zRASIS;


    //Redundancy
    [HideInInspector] public float xmidDist;
    [HideInInspector] public float ymidDist;
    [HideInInspector]public float zmidDist;
    [HideInInspector] public float xmwriDist;
    [HideInInspector] public float ymwriDist;
    [HideInInspector] public float zmwriDist;
    [HideInInspector] public float xHand;
    [HideInInspector] public float yHand;
    [HideInInspector] public float zHand;
    [HideInInspector] public float xFore;
    [HideInInspector] public float yFore;
    [HideInInspector] public float zFore;
    [HideInInspector] public float xArm;
    [HideInInspector] public float yArm;
    [HideInInspector] public float zArm;
    [HideInInspector] public float xTorso;
    [HideInInspector] public float yTorso;
    [HideInInspector] public float zTorso;
    
    //vicon frame number
    [HideInInspector] public int frame;
    public Vector3 markerPosition;





    void Awake()
    {
        button = buttonManager.GetComponent<ButtonManager>();
        cursor =CursorObject.GetComponent<CursorFunction>();
        Thread datastream = new Thread(ConnectClient);
        datastream.Start();
    }
  public void ConnectClient()
    {
        MyClient.Connect("Vicon-OEM"); 
        MyClient.EnableMarkerData();
        MyClient.DisableUnlabeledMarkerData();
        MyClient.DisableSegmentData();
    }
  

    // Update is called once per frame
    void Update()
    {

        //MarkerPosition();
        //marker global z-position in meters
        // Input data is in Vicon co-ordinate space; z-up, x-forward, rhs.
        // We need it in Unity space, y-up, z-forward lhs
        //           Vicon Unity
        // forward    x     z
        // up         z     y
        // right     -y     x
        Output_IsConnected connection = MyClient.IsConnected();
        if (connection.Connected == false)
        {
            ConnectClient();
        }
        MyClient.GetFrame();
        Output_GetFrameNumber Frame =MyClient.GetFrameNumber();
        frame = (int)Frame.FrameNumber;

        ///Marker Data
        Output_GetMarkerGlobalTranslation FINTIP = MyClient.GetMarkerGlobalTranslation(button.subjectName, "FINTIP");
        if (!FINTIP.Occluded)
        {
            xPos = (float)FINTIP.Translation[0] / 1000f; //marker global x-position in meters 

            yPos = (float)FINTIP.Translation[1] / 1000f; //marker global y-position in meters 

            zPos = (float)FINTIP.Translation[2] / 1000f; //marker global z-position in meters
            Vector3 markerPosition = new Vector3(-xPos, zPos, -yPos);
            pathLength += Vector3.Distance(markerPosition, lastPosition);
            lastPosition = markerPosition;
        }

        Output_GetMarkerGlobalTranslation HMD = MyClient.GetMarkerGlobalTranslation(button.subjectName, "HMD");
        xCam = (float)HMD.Translation[0] / 1000f;

        yCam = (float)HMD.Translation[1] / 1000f;

        zCam = (float)HMD.Translation[2] / 1000f;

        //Data Markers--not for movement

        Output_GetMarkerGlobalTranslation MID = MyClient.GetMarkerGlobalTranslation(button.subjectName, "MID");
        if (!MID.Occluded)
        {
            xMID = (float)MID.Translation[0] / 1000f; 
            
            yMID = (float)MID.Translation[1] / 1000f; //-.009f?

            zMID = (float)MID.Translation[2] / 1000f; 
        }
        Output_GetMarkerGlobalTranslation MWRI = MyClient.GetMarkerGlobalTranslation(button.subjectName, "MWRI");
        if (!MWRI.Occluded)
        {
            xMWRI = (float)MWRI.Translation[0] / 1000f; 

            yMWRI = (float)MWRI.Translation[1] / 1000f;  

            zMWRI = (float)MWRI.Translation[2] / 1000f; 
        }
        Output_GetMarkerGlobalTranslation LWRI = MyClient.GetMarkerGlobalTranslation(button.subjectName, "LWRI");
        xLWRI = (float)LWRI.Translation[0] / 1000f;  

        yLWRI = (float)LWRI.Translation[1] / 1000f;  

        zLWRI = (float)LWRI.Translation[2] / 1000f; 

        Output_GetMarkerGlobalTranslation PFORE = MyClient.GetMarkerGlobalTranslation(button.subjectName, "PFORE");
        xPFORE = (float)PFORE.Translation[0] / 1000f;  

        yPFORE = (float)PFORE.Translation[1] / 1000f; 

        zPFORE = (float)PFORE.Translation[2] / 1000f; 

        Output_GetMarkerGlobalTranslation DFORE = MyClient.GetMarkerGlobalTranslation(button.subjectName, "DFORE");
        xDFORE = (float)DFORE.Translation[0] / 1000f; 

        yDFORE = (float)DFORE.Translation[1] / 1000f; 

        zDFORE = (float)DFORE.Translation[2] / 1000f; 

        Output_GetMarkerGlobalTranslation LELB = MyClient.GetMarkerGlobalTranslation(button.subjectName, "LELB");
        xLELB = (float)LELB.Translation[0] / 1000f; 

        yLELB = (float)LELB.Translation[1] / 1000f;  

        zLELB = (float)LELB.Translation[2] / 1000f; 

        Output_GetMarkerGlobalTranslation PUP = MyClient.GetMarkerGlobalTranslation(button.subjectName, "PUP");
        xPUP = (float)PUP.Translation[0] / 1000f;  

        yPUP = (float)PUP.Translation[1] / 1000f; 

        zPUP = (float)PUP.Translation[2] / 1000f; 

        Output_GetMarkerGlobalTranslation DUP = MyClient.GetMarkerGlobalTranslation(button.subjectName, "DUP");
        xDUP = (float)DUP.Translation[0] / 1000f; 

        yDUP = (float)DUP.Translation[1] / 1000f;  

        zDUP = (float)DUP.Translation[2] / 1000f; 

        Output_GetMarkerGlobalTranslation RSHO = MyClient.GetMarkerGlobalTranslation(button.subjectName, "RSHO");
        xRSHO = (float)RSHO.Translation[0] / 1000f; 

        yRSHO = (float)RSHO.Translation[1] / 1000f;  

        zRSHO = (float)RSHO.Translation[2] / 1000f; 

        Output_GetMarkerGlobalTranslation LSHO = MyClient.GetMarkerGlobalTranslation(button.subjectName, "LSHO");
        xLSHO = (float)LSHO.Translation[0] / 1000f; 

        yLSHO = (float)LSHO.Translation[1] / 1000f; 

        zLSHO = (float)LSHO.Translation[2] / 1000f; 

        Output_GetMarkerGlobalTranslation CLAV = MyClient.GetMarkerGlobalTranslation(button.subjectName, "CLAV");
        xCLAV = (float)CLAV.Translation[0] / 1000f; 

        yCLAV = (float)CLAV.Translation[1] / 1000f; 

        zCLAV = (float)CLAV.Translation[2] / 1000f; 

        Output_GetMarkerGlobalTranslation STERN = MyClient.GetMarkerGlobalTranslation(button.subjectName, "STERN");
        xSTERN = (float)CLAV.Translation[0] / 1000f; 

        ySTERN = (float)CLAV.Translation[1] / 1000f; 

        zSTERN = (float)CLAV.Translation[2] / 1000f; 
        

        if (!FINTIP.Occluded &&!MID.Occluded)
        {
            
            xmidDist = (xPos) - (xMID);
            zmidDist = (zPos) - (zMID);
            ymidDist = (yPos) - (yMID);   
            
        }
        if(!FINTIP.Occluded && !MWRI.Occluded)
        {
            xmwriDist = xPos - xMWRI;
            zmwriDist = zPos - zMWRI;
            ymwriDist = yPos - yMWRI;
            
        }

        if(FINTIP.Occluded &&!MID.Occluded)
        {
            xPos = (xmidDist + (xMID));
            yPos= (ymidDist) +(yMID);
            zPos = (zmidDist) +(zMID);
            Vector3 markerPosition = new Vector3(-xPos, zPos, -yPos);
            pathLength += Vector3.Distance(markerPosition, lastPosition);
            lastPosition = markerPosition;

        }
        if(FINTIP.Occluded && MID.Occluded &&!MWRI.Occluded)
        {
            xPos = (xmwriDist + xMWRI);
            yPos = (ymwriDist + yMWRI);
            zPos = (zmwriDist) + zMWRI;
            Vector3 markerPosition = new Vector3(-xPos, zPos, -yPos);
            pathLength += Vector3.Distance(markerPosition, lastPosition);
            lastPosition = markerPosition;
        }

      
        
        if (!button.inPractice && cursor.isStarted)
        {
            TrialTextOutput();
        }
        //Need this to output only while in practice but need practice trials to function similar to regular trials
        if(button.inPractice && cursor.isStarted)
        {
            PracticeOutput();
        }
        

    }
    private void OnDestroy()
    {
        MyClient.Disconnect();
    }

    void TrialTextOutput()
    {
        string dir = String.Format(@"C:\Logs\{0}\{1}\",button.subjectName,button.session);
        if(!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir); 
        }
        filePath = String.Format(@"{0}{1}{2}{3}", dir, button.subjectName, TrialDataGUI.trialCounter, end);

        string output = (frame - cursor.initialFrame).ToString() + "," + (-xPos).ToString() + "," + zPos.ToString() + "," + (-yPos).ToString() + "," +
               (-yCam).ToString() + "," + (-xCam).ToString() + "," + (zCam).ToString() + "," +
               (-yMID).ToString() + "," + (-xMID).ToString() + "," + (zMID).ToString() + "," +
               (-yMWRI).ToString() + "," + (-xMWRI).ToString() + "," + (zMWRI).ToString() + "," +
               (-yLWRI).ToString() + "," + (-xLWRI).ToString() + "," + (zLWRI).ToString() + "," +
               (-yPFORE).ToString() + "," + (-xPFORE).ToString() + "," + (zPFORE).ToString() + "," +
               (-yDFORE).ToString() + "," + (-xDFORE).ToString() + "," + (zDFORE).ToString() + "," +
               (-yLELB).ToString() + "," + (-xLELB).ToString() + "," + (zLELB).ToString() + "," +
               (-yPUP).ToString() + "," + (-xPUP).ToString() + "," + (zPUP).ToString() + "," +
               (-yDUP).ToString() + "," + (-xDUP).ToString() + "," + (zDUP).ToString() + "," +
               (-yRSHO).ToString() + "," + (-xRSHO).ToString() + "," + (zRSHO).ToString() + "," +
               (-yLSHO).ToString() + "," + (-xLSHO).ToString() + "," + (zLSHO).ToString() + "," +
               (-yCLAV).ToString() + "," + (-xCLAV).ToString() + "," + (zCLAV).ToString() + "," +
               (-ySTERN).ToString() + "," + (-xSTERN).ToString() + "," + (zSTERN).ToString() + "\n";
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, " , , FINTIP, , , HMD, , , MID , , , MWRI, , ,LWRI, , ,PFORE, , ,DFORE, , ,LELB, , , PUP, , ,DUP, , ,RSHO, , ,LSHO, , ,CLAV, , , STERN\n " +
                "Frame,X,Y,Z, X,Y,Z, X,Y,Z, X,Y,Z, X,Y,Z, X,Y,Z, X,Y,Z, X,Y,Z, X,Y,Z, X,Y,Z, X,Y,Z, X,Y,Z, X,Y,Z,X,Y,Z \n ");
        }
        else File.AppendAllText(filePath, output);
    }
    public static int pNum;
    void PracticeOutput()
    {
            pNum = TrialDataGUI.practiceCounter;
        if(TrialDataGUI.blockCounter == 0)
        {
            pNum = TrialDataGUI.practiceCounter + 43;
        }
        if(button.inPractice)
        {
            string dir = String.Format(@"C:\Logs\{0}\{1}\Practice\", button.subjectName, button.session);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            filePath = String.Format(@"{0}{1}_B{2}_S{3}_T{4}{5}", dir, button.subjectName, TrialDataGUI.blockCounter,TrialDataGUI.setCounter,pNum, end);
            string output = (frame - cursor.initialFrame).ToString() + "," + (-xPos).ToString() + "," + zPos.ToString() + "," + (-yPos).ToString() + "," +
               (-yCam).ToString() + "," + (-xCam).ToString() + "," + (zCam).ToString() + "," +
               (-yMID).ToString() + "," + (-xMID).ToString() + "," + (zMID).ToString() + "," +
               (-yMWRI).ToString() + "," + (-xMWRI).ToString() + "," + (zMWRI).ToString() + "," +
               (-yLWRI).ToString() + "," + (-xLWRI).ToString() + "," + (zLWRI).ToString() + "," +
               (-yPFORE).ToString() + "," + (-xPFORE).ToString() + "," + (zPFORE).ToString() + "," +
               (-yDFORE).ToString() + "," + (-xDFORE).ToString() + "," + (zDFORE).ToString() + "," +
               (-yLELB).ToString() + "," + (-xLELB).ToString() + "," + (zLELB).ToString() + "," +
               (-yPUP).ToString() + "," + (-xPUP).ToString() + "," + (zPUP).ToString() + "," +
               (-yDUP).ToString() + "," + (-xDUP).ToString() + "," + (zDUP).ToString() + "," +
               (-yRSHO).ToString() + "," + (-xRSHO).ToString() + "," + (zRSHO).ToString() + "," +
               (-yLSHO).ToString() + "," + (-xLSHO).ToString() + "," + (zLSHO).ToString() + "," +
               (-yCLAV).ToString() + "," + (-xCLAV).ToString() + "," + (zCLAV).ToString() + "," +
               (-ySTERN).ToString() + "," + (-xSTERN).ToString() + "," + (zSTERN).ToString() + "\n";
;
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, " , , FINTIP, , , HMD, , , MID , , , MWRI, , ,LWRI, , ,PFORE, , ,DFORE, , ,LELB, , , PUP, , ,DUP, , ,RSHO, , ,LSHO, , ,CLAV, , , STERN\n " +
                    "Frame,X,Y,Z, X,Y,Z, X,Y,Z, X,Y,Z, X,Y,Z, X,Y,Z, X,Y,Z, X,Y,Z, X,Y,Z, X,Y,Z, X,Y,Z, X,Y,Z, X,Y,Z,X,Y  \n ");
            }
            else File.AppendAllText(filePath, output);
        }
    }
}