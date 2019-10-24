using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Phidget22;
using Phidget22.Events;

public class NetworkLAController : MonoBehaviour
{
    private DCMotor dcmotor = new DCMotor();
    private VoltageRatioInput vol = new VoltageRatioInput();
	public float move = 0;

	// Use this for initialization
	void Start () 
	{
		

		dcmotor.Attach += dcmotor_attach;
		dcmotor.Error += dcmotor_error;
		dcmotor.VelocityUpdate += onVelocityUpdate;
        vol.VoltageRatioChange += onVoltageRatioChange;


        try
		{
            Net.EnableServerDiscovery(ServerType.DeviceRemote);
			dcmotor.Channel = 0;
			dcmotor.DeviceSerialNumber = 513940; //SerialNumber of SBC4
			dcmotor.HubPort = 0;
            dcmotor.IsRemote = true;
			dcmotor.Open();
            vol.Channel = 0;
            vol.DeviceSerialNumber = 529025; //SerialNumber of VINT Hub
            vol.HubPort = 0;
            vol.Open();
		}
		catch(PhidgetException ex){Debug.Log ("Error opening device: " + ex.Message);}
	}

	void dcmotor_attach(object sender, Phidget22.Events.AttachEventArgs e)
	{
		Debug.Log ("Phidget Attached!");
	}

	void dcmotor_error(object sender, Phidget22.Events.ErrorEventArgs e)
	{
		Debug.Log ("Code: " + e.Code.ToString());
		Debug.Log ("Description: " + e.Description);
	}

	void onVelocityUpdate(object sender, DCMotorVelocityUpdateEventArgs e)
	{
		try{dcmotor.TargetVelocity = move;}
		catch (PhidgetException ex) {Debug.Log ("Error setting duty cycle: " + ex.Message);}
	}

    void onVoltageRatioChange(object sender, VoltageRatioInputVoltageRatioChangeEventArgs e)
    {
        Debug.Log("VoltageRatio : " + e.VoltageRatio);
    }

    // Update is called once per frame
    void Update () 
	{
		if(Input.GetKeyDown (KeyCode.Escape))
		{
			dcmotor.VelocityUpdate -= onVelocityUpdate;
			dcmotor.Close();
			dcmotor = null;
            vol.VoltageRatioChange -= onVoltageRatioChange;
            vol.Close();
            vol = null;

            Application.Quit ();
		}
	}

	void FixedUpdate()
	{
		 move = Input.GetAxis("Vertical");
	}

	void OnApplicationQuit()
	{
		if (Application.isEditor)
			Phidget.ResetLibrary();
		else
			Phidget.FinalizeLibrary(0);
	}

}
