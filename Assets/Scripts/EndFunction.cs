using UnityEngine;
//This script controls what happens in the game when the cursor interacts with the end target
public class EndFunction : MonoBehaviour
{
    //color properties
    public Color baseColor;
    public Color triggerColor;

    //renderer and collider properties
    public static Renderer endRend;
    public static Collider endCollider;
    //calls to button manager to check if in practice mode
    public GameObject ButtonManagerObject;
    ButtonManager button;

    //Bool for determining if End reached in 5s
    public static bool endReached;

    // Start is called before the first frame update
    public void Start()
    {
        button = ButtonManagerObject.GetComponent<ButtonManager>();
        gameObject.SetActive(false);

            endRend = GetComponent<Renderer>();
            endRend.enabled = false;
            endCollider = GetComponent<Collider>();
            endCollider.enabled = false;
        
        
    }
    private void Update()
    {
        if(button.inPractice)
        {
            endRend.enabled = true;
            endCollider.enabled = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            transform.GetComponent<Renderer>().material.color = triggerColor;
            endReached = true;
            
            
        }
    }
   private void OnTriggerExit(Collider other) //when object exits target changes back to base color
    { 
        transform.GetComponent<Renderer>().material.color = baseColor; 
    }
    //Methods to control end target from other scripts
   /* public static void EndControl() //Triggers on staying in home for .5s
    {
        
        endRend.enabled = true;
        endCollider.enabled = true;
        Debug.Log("End Control run");
    }*/
    
}
