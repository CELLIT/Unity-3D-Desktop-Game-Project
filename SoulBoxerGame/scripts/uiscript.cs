using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uiscript : MonoBehaviour
{
    private Text health;
    public GameObject obj;
    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
        health.text = "Health : " + obj.GetComponent<newPlayerMovement>().playerHealth.ToString();
    }
}
