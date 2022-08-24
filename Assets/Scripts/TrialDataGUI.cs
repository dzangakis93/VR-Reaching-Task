using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrialDataGUI : MonoBehaviour
{

    private Text trialNum;
    public Text practiceText;
    public GameObject ButtonManager;
    ButtonManager button;

    public static int trialCounter=0;
    public static int blockCounter=0;
    public static int setCounter =0;
    public static int practiceCounter = -43;
    public static int warmupCounter = 0;
    void Start()
    {
        button= ButtonManager.GetComponent<ButtonManager>();
        trialNum = GetComponent<Text>();
        
    }
    // Update is called once per frame
    void Update()
    {
        UIText();
        BlockSetCounter();
    }
    //Method to set Text on UI with trial data
    void UIText()
    {
        if(button.inPractice)
        {
            practiceText.enabled = true;
            if (practiceCounter <= 0)
            {
                practiceText.text = " IN PRACTICE \n Practice Trial Number: " + (practiceCounter + 43).ToString();
            } 
            else practiceText.text = " IN PRACTICE \n Practice Trial Number: " + (practiceCounter-1).ToString();

        }
        else if(!button.inPractice)
        {
            practiceText.enabled= false;
        }

        if (trialCounter <= 40)
        {

            trialNum.text = " BL-V Trial Number " + trialCounter.ToString() + "\n Last Path: " + CursorFunction.lastPath + "\n Ten Path Avg: " + CursorFunction.avg + "\n Trial Timer: " + Math.Round(CursorFunction.currentTime, 3) + "\n" + CursorFunction.targetTimer;

        }
        if (trialCounter > 40 && trialCounter <= 80)
        {

            int trial = trialCounter - 40;
            trialNum.text = " BL-NV Trial Number " + trial.ToString() + "\n Last Path: " + CursorFunction.lastPath + "\n Ten Path Avg: " + CursorFunction.avg + "\n Trial Timer: " + Math.Round(CursorFunction.currentTime, 3);
        }
        if (trialCounter > 80 && trialCounter <= 240)
        {

            int trialInt = trialCounter - 80;
            trialNum.text = " Int Trial Number " + trialInt.ToString() + "\n Last Path: " + CursorFunction.lastPath + "\n Ten Path Avg: " + CursorFunction.avg + "\n Trial Timer: " + Math.Round(CursorFunction.currentTime, 3);
        }
        if (trialCounter > 240 && trialCounter <= 280)
        {
            blockCounter = 4; int trialRetA = trialCounter - 240;
            trialNum.text = " Ret-NV Trial Number " + trialRetA.ToString() + "\n Last Path: " + CursorFunction.lastPath + "\n Ten Path Avg: " + CursorFunction.avg + "\n Trial Timer: " + Math.Round(CursorFunction.currentTime, 3);

        }
        if (trialCounter > 280 && trialCounter <= 320)
        {

            int trialRetB = trialCounter - 280;
            trialNum.text = " Ret-V Trial Number " + trialRetB.ToString() + "\n Last Path: " + CursorFunction.lastPath + "\n Ten Path Avg: " + CursorFunction.avg + "\n Trial Timer: " + Math.Round(CursorFunction.currentTime, 3);

        }
        
    }
   //Method to set Block and Set Numbers 
    void BlockSetCounter()
    {
        if(trialCounter == 0 && practiceCounter ==0) 
        {
            blockCounter = 1;
            setCounter = 1;
        }
        if(trialCounter ==40)
        {
            blockCounter = 2;
            setCounter = 1;
        }
        if(trialCounter == 80)
        {
            blockCounter = 3;
            setCounter = 1;
        }
        if(trialCounter ==240)
        {
            blockCounter=4;
            setCounter=1;
        }
        if(trialCounter==280)
        {
            blockCounter = 5;
            setCounter = 1;
        }
    }
}
