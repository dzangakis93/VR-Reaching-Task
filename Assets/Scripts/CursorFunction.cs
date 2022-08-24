using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System;

public class CursorFunction : MonoBehaviour
{
    //get subject name from input field
    public GameObject buttonManager;
    ButtonManager button;
    public GameObject MarkerObject;
    Marker marker;
    public GameObject HomeObject;
    public GameObject EndObject;
    public GameObject TextObject;

    public Color hBaseColor;
    public Color hTriggerColor;

    public Color eBaseColor;
    public Color eTriggerColor;

    Rigidbody index;
   

    public Vector3 markerPosition;

    //sets path length 
    public static float pathLength = 0;
    public static float avg = 0;
    public static float lastPath;
    
   
    public bool isStarted = false;
    public bool isFinished = false;  

    public int initialFrame = 0;
    public int startFrame = 0;
    public int endFrame = 0;
    public int finishedTrials = 0;

    //sets cursor visibility
    public static Renderer cursorRend;

    // Queue to store pathLengths
    public static Queue<float> LastTenPaths = new Queue<float>();


    //intialize audio sources
    public AudioSource score;
    public AudioSource noScore;
    public AudioSource bubblePop;

    //CSV output
    static string end = ".csv";

    public static string filePath = "";

    public static string feedback;
    
    //Timer
    [HideInInspector] public bool timerActive = false;
    [HideInInspector] public static float currentTime;

    //Set up practice mode
    public bool inPractice = true;
    // Start is called before the first frame update
    //Get components of button manager, marker
    void Start()
    {
        button = buttonManager.GetComponent<ButtonManager>();
        marker = MarkerObject.GetComponent<Marker>();
        index = GetComponent<Rigidbody>();
        cursorRend = GetComponent<Renderer>();
        TextObject.SetActive(false);
        HomeObject.SetActive(false);
        EndObject.SetActive(false);
        
    }


    // Update is called once per frame
    //Gets marker position for marker on index finger and creates a vector3 which is used to move the index object
    //If timer is active, updates current time by adding change in time between each frame
    //if current time is >5 runs the trial reset method and resets the timer
    //Runs the targetTimer method which checks the time the cursor is inside either the home or end targets
   
    private void Update()
    {
      
        markerPosition = new Vector3(-marker.xPos, marker.zPos, -marker.yPos);
        index.MovePosition(markerPosition);
        if(timerActive == true)
        {
            currentTime =currentTime+Time.deltaTime;
            
        }
        if(currentTime > 5) //Trial timer to ensure if time exceeds 5 seconds trial resets
        {
            TrialReset();
            timerActive = false;
            currentTime = 0;

        }
        TargetTimer();
       if (targetTimer >=.5) //timer to make sure cursor stays in end for .5s
        {
            //make end bool true
            inEnd = true;
        }
       if (targetTimer >=2) //timer to make sure cursor stays in home for 2s
        {
            //make home bool true
            inHome = true;
        }
    }
    //Timer for home/end target
    public bool inHome = false;
    public bool inEnd = false;
    public static float targetTimer = 0;
    public bool targetTimeActive = false;
    //Methods for controlling the target timer
    void targetTimerStart()
    {
        targetTimeActive = true;
        targetTimer = 0;
        
    }
    void targetTimerStop()
    {
        targetTimeActive=false;
        targetTimer = 0;
        
    }
    void TargetTimer()
    {
        if (targetTimeActive == true)
        { targetTimer = targetTimer + Time.deltaTime; }
    }
    //booleans to prevent constant trigger on Stay
    public bool h = false;
    public bool e = false;
    public bool r = false;
    public bool pop = false;
    void OnTriggerEnter(Collider other) //makes cursor visible on entering either target
    {
        
        cursorRend.enabled = true;
        if (button.endSet && button.homeSet)
        {

            //When cursor enters home target  
            if (other.gameObject.CompareTag("Home"))
            {
                //change to htrigger color 
                HomeObject.transform.GetComponent<Renderer>().material.color = hTriggerColor;
                h = true;
                inHome = false;
                targetTimerStart();
                TextObject.SetActive(false);
                if (!button.inPractice)
                {

                    r = true; //prevents from automatically pausing again after practice trials

                }
                
            }

            //When cursor enters end target

            if (other.gameObject.CompareTag("End"))
            {
                
                EndObject.transform.GetComponent<Renderer>().material.color = eTriggerColor;
                targetTimerStart();
                e = true;
                inEnd = false;
                pop = true;
            }
        }
    }
    public int trialComplete = 0;
    //OnStay runs when cursor stays inside either home or end targets
    private void OnTriggerStay(Collider col)
    {
        if (button.endSet && button.homeSet)//Checks if the home and end positions are set
        {
            //In Home target:
            //After 1.5s increases trial counter or practice counter and triggers packet sender to send UDP packet to Nexus
            
            if (col.gameObject.CompareTag("Home"))
            {
                if (h && targetTimer > 1.5 && !PacketLogic.triggerActivated)
                {
                    if (!button.inPractice)
                    {
                        TrialDataGUI.trialCounter++;
                    }
                    if (button.inPractice)
                    {
                        TrialDataGUI.practiceCounter++;
                    }
                    //activates trigger to start recording in nexus
                    PacketLogic.triggerActivated = true;
                    PacketLogic.triggerDeactivated = false;
                    initialFrame = marker.frame;
                }
                //If in home for 2 seconds sets end target active, sets h false to prevent looping of onStay code
                if (inHome && h)
                {
                    startFrame = marker.frame - initialFrame;
                    EndObject.SetActive(true);
                    EndObject.GetComponent<Renderer>().enabled = true;
                    EndObject.GetComponent<Collider>().enabled = true;
                    //prevents repeatedly running through if statement
                    h = false;
                    isStarted = true;
                    isFinished = false;
                }
            }
            //If cursor is in end object
            if (col.gameObject.CompareTag("End"))
            {
                //Ends trial and sends stop packet over UDP to Nexus
                //Stops trial timer
                //If not in practice: stores path lengths, and pauses game if trial%20==0 
                //Outputs trial data to text
                if (inEnd && e)
                {
                    isStarted = false;
                    isFinished = true;
                    PacketLogic.triggerActivated = false;
                    PacketLogic.triggerDeactivated = true;
                    deactivateTimer();
                    if (!button.inPractice)
                    {
                        
                        finishedTrials += 1; //counter to track number of trials finished
                        StorePathLengths();
                        if (TrialDataGUI.trialCounter % 20 == 0 && TrialDataGUI.trialCounter > 0 /*&& !button.inPractice*/ && r)
                        {
                            TrialDataGUI.setCounter++;
                            button.PauseGame();
                            r = false;
                        }
                        WriteToText();
                    }
                    //If statement to check when to pause trials during initial practice block
                    if (button.inPractice)
                    {
                        if (TrialDataGUI.practiceCounter == -23 || TrialDataGUI.practiceCounter == 0)
                        {
                            TrialDataGUI.setCounter++;
                            button.PauseGame();
                        }
                    }
                    EndObject.SetActive(false);

                    endFrame = marker.frame - initialFrame;

                    if (TrialDataGUI.trialCounter > 80 && TrialDataGUI.trialCounter <= 240)
                    {
                        TrialEnd();
                    }
                    // close application after completing trials
                    if (TrialDataGUI.trialCounter == 320)
                    {
                        Application.Quit();

                    }
                    e = false;
                    Pop();
                }
                
                WarmupTrials();
            }
        }
    }
    //On Exit runs when the cursor exits either home or end targets
    private void OnTriggerExit(Collider col)
    {
        //When the cursor exits the home target
        //Stops the timer which checks the time inside the target, sets inHome false, if the trial is started it activates the trial timer
        //If not in practice, resets the pathlength to 0 and 
        //records the starting frame of each trial from nexus by subtracting the current frame from the initial frame (frame number when nexus starts to record)
        //Disables cursor renderer for trials where cursor is invisible
         if (col.gameObject.CompareTag("Home"))
         {
            //change to hbase color
            HomeObject.transform.GetComponent<Renderer>().material.color = hBaseColor;
            targetTimerStop();
            inHome = false;
            inEnd = false;
            if (isStarted)
            {
                activateTimer();
            }
            if (!button.inPractice)
            {
                //resetting x y z components for path length
                marker.pathLength = 0;
            }
            if ( (!button.inPractice && isStarted && TrialDataGUI.trialCounter > 40 && TrialDataGUI.trialCounter <= 280) || (button.inPractice && TrialDataGUI.practiceCounter >-20 && TrialDataGUI.practiceCounter<0))
            { 
                cursorRend.enabled = false; 
            }
         }
         //When the cursor exits end target
         //Stops the target timer and sets in end to false
         //Disables renderer if the trial has started but not finished and in trial where cursor should be invisible
         if (col.gameObject.CompareTag("End"))
        {
            //change to ebase color
            EndObject.transform.GetComponent<Renderer>().material.color = eBaseColor;
            targetTimerStop();
            inEnd = false;
            
            if(isStarted && !button.inPractice && TrialDataGUI.trialCounter > 40 && TrialDataGUI.trialCounter <= 280 || (button.inPractice && TrialDataGUI.practiceCounter > -20 && TrialDataGUI.practiceCounter < 0))
            {
                cursorRend.enabled = false;
            }
        }
    }
    //TrialEnd checks if the last reaching path is <=the moving average OR <=1.15*optPath
    // If true, plays positive chime, adds one point to scoreboard, and sets feedback to 1 for the total data CSV output
    // If false, plays negative chime and sets feedback to 0 for the total data CSV output
    void TrialEnd()
    {
        //add in check for % of optimal length if <=optPath*1.15 positive feedback

        if (lastPath < LastTenPaths.Average() || lastPath <= 1.15 * button.optPath)
        {
            ScoreboardLogic.scoreNum += 1;
            this.score.Play();
            feedback = "1";
        }
        else if (lastPath > LastTenPaths.Average() && lastPath > 1.15*button.optPath)
        {
            this.noScore.Play();
            feedback = "0";
        }
      
    }
    //Pop plays popping sound effect during the baseline and retention trials
    void Pop()
    {
        if (TrialDataGUI.trialCounter < 80 || TrialDataGUI.trialCounter > 240)
        {
            if (pop)
            {
                this.bubblePop.Play();
                Debug.Log("Pop");
                pop = false;
            }
        }
    }
    //Stores Last 10 path lengths to create a moving average of pathlengths
    //If there are 10 paths in the queue, dequeues to remove one path from the queue
    public void StorePathLengths() 
    {
       
        lastPath = (float)Math.Round(marker.pathLength,5);

        LastTenPaths.Enqueue(lastPath);
        if (LastTenPaths.Count == 10)
        {
            LastTenPaths.Dequeue();
        }
        avg = (float)Math.Round(LastTenPaths.Average(),5);
    }
    //Function to write outputs to text
    public void WriteToText() //creates text file output on completion of each trial called .5s after entering end target
    {
        if (TrialDataGUI.trialCounter > 0)
        {
            string dir = String.Format(@"C:\Logs\{0}\{1}\", button.subjectName, button.session);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            filePath = String.Format("{0}TD_{1}{2}", dir, button.subjectName, end);
            string txtOutput =
            "  " + TrialDataGUI.trialCounter.ToString() + "," + lastPath.ToString() + "," + avg.ToString() + "," + ScoreboardLogic.scoreNum.ToString() + "," + feedback + "," + startFrame.ToString() + "," + endFrame.ToString() 
            +","+ button.homeX.ToString() + "," + button.homeY.ToString()+"," + button.homeZ.ToString()+ "'" +button.endX.ToString() +","+ button.endY.ToString() + "," + button.endZ.ToString() + ","+ button.optPath.ToString()+ "\n";

            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "Trial:,Path:,Average:,Score:,Feedback:,Start Frame:,End Frame:,Home X, Home Y,Home Z, End X, End Y, End Z, Optimal Path:\n");
                File.AppendAllText(filePath, txtOutput);
            }
            else if (File.Exists(filePath))
                File.AppendAllText(filePath, txtOutput);
        }
    }
    //Trial Timer to control whether trial needs to reset
    //ActivateTimer sets the timer to active, allowing time to increase on update
    void activateTimer()
    {
        timerActive = true;
        
    }
    //Sets timer to inactive and resets the trial timer to 0
    void deactivateTimer()
    {
        timerActive = false;
        currentTime = 0;
    }
    //Method to reset trial; triggers after 5s of trial time or on pause
    //Makes cursor visible, displays text to reutn to home, and turns off the end target
    // If the trial counters is >=1 and the trial is started but not finished, deactives the trial timer, deletes the CSV output for the trial and reduces the trial counter by 1
    public void TrialReset() 
    {
        TextObject.SetActive(true);
        cursorRend.enabled = true;
        EndObject.SetActive(false);
        if (TrialDataGUI.trialCounter >= finishedTrials && isStarted && !isFinished  && !button.inPractice) //prevents trial count from going negative due to pause need to adjust this functionality 
        {
            deactivateTimer();
            if (File.Exists(Marker.filePath))
            { 
                File.Delete(Marker.filePath); 
            }
            TrialDataGUI.trialCounter=finishedTrials;
        }
        PacketLogic.triggerActivated = false;
        PacketLogic.triggerDeactivated = true;
        isStarted = false;
        isFinished = true;
    }
    // Method to run three warmup trials in between each set of 20 trials where there will be no vision
    //Checks if inPractice if yes, checks if practice counter is 0 and trial finished or practice counter is 4 and trial finished at the end of each practice trial
    //If either condition is met, practice mode is ended and regular trials are resumed
    void WarmupTrials()
    {
        if (button.inPractice)
        {
            if ((TrialDataGUI.practiceCounter == 0 && isFinished) || (TrialDataGUI.practiceCounter == 4 && isFinished))
            {
                
                button.inPractice = false;
                TrialDataGUI.practiceCounter = 1;
                isFinished = false;
            }
        }
    }
}

