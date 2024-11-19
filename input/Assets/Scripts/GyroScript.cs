using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroScript : MonoBehaviour
{
    private SenderScript senderFunction;
    private GameObject oscSender;

    // Start is called before the first frame update
    private void Start()
    {
        oscSender = GameObject.Find("OSCSender");
        senderFunction = oscSender.GetComponent<SenderScript>();

        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    /// <summary>
    /// This function returns true until the button isn't being pressed anymore
    /// </summary>
    /// <returns></returns>
    public void PressButton()
    {
        Debug.Log("Press button");
    }

    /// <summary>
    /// This function returns the three gyro attitude values
    /// </summary>
    public Quaternion GetGyro()
    {
        Debug.Log("Got the following gyro values: " + Input.gyro.attitude);
        return Input.gyro.attitude;
    }
}