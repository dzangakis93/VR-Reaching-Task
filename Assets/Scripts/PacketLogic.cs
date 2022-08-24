using UnityEngine;


public class PacketLogic : MonoBehaviour
{
    //public GameObject triggerObject;
    // private TriggerLogic triggerLogic; //reference to other script so we can use GetComponent<>
    // Logic for sending packets
    //sends start packet on entering home target (in onTriggerEnter in homefunction script)
    //Sends stop packet on entering end target (in CursorFunction script)
    
    public static bool triggerActivated = false; 
    
    public static bool triggerDeactivated = false;


    public bool sendStartPacket = false; // for passthrough to UDPPacketSender Script
    public bool sendStopPacket = false; // for passthrough to UDPPacketSender Script

    private int count = 0;

    void Start()
    {
        Debug.Log("The Packet Manager Object has initialized.");
    }

    //MAIN LOOP
    void Update()
    {
        SendPacketFromTrigger();    // if trigger == true send desired packet 
    }

    void SendPacketFromTrigger() 
    {  
        if (triggerActivated == true && count < 1) // don't send more than one packet at a time
        {
            //send Start/Stop Recording packet here
            sendStartPacket = true;
            
            count++;
        }
        
        if (triggerDeactivated == true && count == 1)
        {
            sendStopPacket = true;
            
            count = 0;
        }
    }
    

}
