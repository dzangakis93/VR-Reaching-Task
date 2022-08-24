using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class DualDisplay : MonoBehaviour
{

    int w = 1920;
    int h = 1080;
    int x = 0;
    int y = 0;


    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 9999;
       
        if (Display.displays.Length >1)
        {
            Display.displays[1].SetParams(w, h, x, y);
            Display.displays[1].Activate(w,h, 9999);
            
            
        }
    }

    
}
