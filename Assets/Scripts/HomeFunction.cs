using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeFunction : MonoBehaviour
{

    //Initialize trial counter
    public Color baseColor;
    public Color triggerColor;
    

    //calls to button manager to check if in practice mode
    public GameObject ButtonManagerObject;
    ButtonManager button;

    public void Start()
    {
        gameObject.SetActive(false);
        button = ButtonManagerObject.GetComponent<ButtonManager>();


    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            transform.GetComponent<Renderer>().material.color = triggerColor;

        }
    }
    private void OnTriggerExit(Collider other)
    {
        transform.GetComponent <Renderer>().material.color = baseColor;
    }
}
