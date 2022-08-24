using UnityEngine;
using UnityEngine.UI;

public class ScoreboardLogic : MonoBehaviour
{
    // Start is called before the first frame update
    public Text ScoreValue;
    public Text Score;
    public Text Paused;
    public GameObject Button;
    ButtonManager button;
    [HideInInspector]public static int scoreNum;
    void Start()
    {
        button = Button.GetComponent<ButtonManager>();
        ScoreValue = GetComponent<Text>();
        scoreNum = 0;
        scoreBoardOff();
    }

    // Update is called once per frame
    void Update()
    {
        ScoreValue.text = scoreNum.ToString();
        if(button.x == true)
        {
            Paused.enabled = true;
        }
        else if(button.x == false)
        {
            Paused.enabled = false;
        }
        if (TrialDataGUI.trialCounter > 80 && TrialDataGUI.trialCounter <= 240)
        {
            scoreBoardOn();
            
        }
        else if(TrialDataGUI.trialCounter >240)
        {
            scoreBoardOff();
        }
       
    }
    void scoreBoardOn()
    {
        Score.enabled=true;
        ScoreValue.enabled = true;
    }
    void scoreBoardOff()
    {
        Score.enabled = false;
        ScoreValue.enabled = false;
    }
}
