using System;
using System.Linq; //for Enumerable.Repeat().... used for packet message padding.
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPPacketSender : MonoBehaviour
{
    
    // This script controls behavior of packets used to start and stop capture in Nexus.
    [Tooltip("Place your Packet Manager object from the hierarchy in this slot.")]
    public GameObject packetManagerObject;
    private PacketLogic packetLogic; 
    public GameObject buttonManagerObject;
    private ButtonManager button;
    string tName;
    UdpClient Client = new UdpClient(); //Client will always be on. will call SendPacket() method when needed
   
    void Awake() //put all calls to other GameObjects here so they are valid by the Start() method
    {
        button = buttonManagerObject.GetComponent<ButtonManager>();
        packetLogic = packetManagerObject.GetComponent<PacketLogic>(); //makes components of script in other GameObject callable
    }

    void Start()
    {
        Debug.Log("The UDPPacketSender script has initialized");
        ClientConnect();
    }

    void Update()
    {
        //test
        dgramDatabasePath = String.Format( @"C:\Users\Public\Documents\Ataxia_Vicon_M1NIBSandRL\{0}\{1}\", button.subjectName,button.session);
       
        if (packetLogic.sendStartPacket)
        {
            SendPacket("Start");
            packetLogic.sendStartPacket = false; //reset to false after sending packet so only one packet is sent
        }
        else if (packetLogic.sendStopPacket)
        {
            SendPacket("Stop");
            packetLogic.sendStopPacket = false; //reset to false after sending packet so only one packet is sent
        }
        if (button.inPractice)
        {
            tName = String.Format("P_{0}_B{1}_S{2}_T{3}", button.subjectName, TrialDataGUI.blockCounter, TrialDataGUI.setCounter, Marker.pNum);
        }
        else tName = String.Format("{0}_T{1}",button.subjectName,TrialDataGUI.trialCounter);
        
    }
    void ClientConnect()
    {
        IPEndPoint ep = new IPEndPoint(IPAddress.Parse("169.254.50.139"), 30); // IP is for direct ethernet connection. Can change this if needed
        try 
        {
            Client.Connect(ep);
            Debug.Log("UDP connected");
        }
        catch (Exception e)
        {
            print("Exception thrown " + e.Message);
        }
    }

    /// <summary>
    ///  can add other public variables to manipulate in Unity. note below that the "dgramDatabasePath will need to be changed
    ///  to the same database path as shown in Nexus.
    /// </summary>


    //public static int trialNum = 1; if new trial counter doesnt work replace TrialdataGUI.trialCounter with trial num in line 91 and 102
    //input ability to change file path
    string dgramDatabasePath = @"C:\Users\Public\Documents\Ataxia_Vicon_M1NIBSandRL\" ; //double backslash enters as string: "\"
    int packetID = 10000;   
    int targetLength = 500; //used for padding and checksums
    void SendPacket(string packetFlavour)
    {
        if (packetFlavour == "Start")
        {
            string dgramBeginning = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?><CaptureStart>"; //backslah" enters as string: """
            string dgramEnd = "\"/><Delay VALUE=\"250\"/><PacketID VALUE=\"" + packetID.ToString() + "\"/></CaptureStart>";

            ///THESE ARE THE VALUES YOU MAY NEED TO UPDATE BASED ON THE NEXUS HEADINGS FOR YOUR CURRENT PROJECT/TESTING SESSION

            string dgramNotes = "";
            string dgramDescription = "";

            ///TOTAL MESSAGE
            string message = dgramBeginning + "<Name VALUE=\"" + tName + "\"/><Notes VALUE=\"" + dgramNotes + "\"/><Description VALUE=\"" + dgramDescription + "\"/><DatabasePath VALUE=\"" + dgramDatabasePath + dgramEnd;
            message += String.Concat(Enumerable.Repeat(" ",(targetLength - message.Length)));
            byte[] datagram = Encoding.ASCII.GetBytes(message);
            Client.Send(datagram, datagram.Length); 
        }
        else if (packetFlavour == "Stop") //for the case where we want to send a Stop Packet
        {
            string dgramBeginning = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?><CaptureStop RESULT=\"SUCCESS\">"; //backslah" enters as string: """
            string dgramEnd = "\"/><Delay VALUE=\"250\"/><PacketID VALUE=\"" + (packetID+1).ToString() + "\"/></CaptureStop>";

            ///TOTAL MESSAGE
            string message = dgramBeginning + "<Name VALUE=\"" + tName + "\"/><DatabasePath VALUE=\"" + dgramDatabasePath + dgramEnd;
            message += String.Concat(Enumerable.Repeat(" ", (targetLength - message.Length)));
            byte[] datagram = Encoding.ASCII.GetBytes(message);
            Client.Send(datagram, datagram.Length);
            packetID += 2; //everytime a Stop packet is sent increment for the next trial i.e Trial 02 Start packetID = 16668.
           
        }

       
    }
}

//  FIRST LINE  0000 ....E..Sy}......
//  SECOND LINE 0010 .............?..

/* TARGET STRING
 * <?xml version="1.0" encoding="UTF-8" standalone="no"?><CaptureStart><Name VALUE="PatientTestNew Packet Send 01"/><Notes VALUE=""/><Description VALUE=""/><DatabasePath VALUE="C:\Users\Public\Documents\Unity Testing\Golf Club Test\PatientTest\May10\"/><Delay VALUE="250"/><PacketID VALUE="16666"/></CaptureStart>.
 */