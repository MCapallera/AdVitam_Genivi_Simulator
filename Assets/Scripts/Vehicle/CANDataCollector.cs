/*
 * Copyright (C) 2016, Jaguar Land Rover
 * This program is licensed under the terms and conditions of the
 * Mozilla Public License, version 2.0.  The full text of the
 * Mozilla Public License is at https://www.mozilla.org/MPL/2.0/
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public struct FullDataFrame
{
    public float time;
    public float cruiseSpeed;
    public float rpm;
    public float gearPosActual;
    public float gearPosTarget;
    public float accelleratorPos;
    public float deceleratorPos;
    public float rollRate;
    public float steeringWheelAngle;
    public float vehicleSpeed;
    public float vehicleSpeedOverGround;
    public float wheelSpeedFL;
    public float wheelSpeedFR;
    public float wheelSpeedRL;
    public float wheelSpeedRR;
    public float yawRate;
    public float triggeredEvent1TimeStamp;
    public float triggeredEvent2TimeStamp;
    public float triggeredEvent3TimeStamp;
	public bool autoPilot; //Marine

    public string ToCSV()
    {
        string data = string.Format("EMSSetSpeed, {1:F4}, {0}\n" +
                "EngineSpeed, {2:F4}, {0}\n" +
                "GearPosActual, {3:N}, {0}\n" +
                "GearPosTarget, {4:N}, {0}\n" +
                "AcceleratorPedalPos, {5:F4}, {0}\n" +
                "DeceleratorPedalPos, {6:F4}, {0}\n" +
                "RollRate, {7:F4}, {0}\n" +
                "SteeringWheelAngle, {8:F4}, {0}\n" +
                "VehicleSpeed, {9:F4}, {0}\n" +
                "VehicleSpeedOverGnd, {10:F4}, {0}\n" +
                "WheelSpeedFrL, {11:F4}, {0}\n" +
                "WheelSpeedFrR, {12:F4}, {0}\n" +
                "WheelSpeedReL, {13:F4}, {0}\n" +
                "WheelSpeedReR, {14:F4}, {0}\n" +
                "YawRate, {15:F4}, {0}\n", time, cruiseSpeed, rpm, gearPosActual, gearPosTarget, accelleratorPos, deceleratorPos, rollRate, steeringWheelAngle, vehicleSpeed, vehicleSpeedOverGround, wheelSpeedFL, wheelSpeedFR, wheelSpeedRL, wheelSpeedRR, yawRate);

        //TODO: handle this better
        if (triggeredEvent1TimeStamp > 0f)
        {
            data += string.Format("TriggeredEvent1, 0.0, {0}\n", triggeredEvent1TimeStamp);
        }

        if (triggeredEvent2TimeStamp > 0f)
        {
            data += string.Format("TriggeredEvent2, 0.0, {0}\n", triggeredEvent2TimeStamp);
        }

        if (triggeredEvent3TimeStamp > 0f)
        {
            data += string.Format("TriggeredEvent3, 0.0, {0}\n", triggeredEvent3TimeStamp);
        }

        return data;
          
    }

    public string ToICCSV()
    {
        return string.Format(
            "EngineSpeed, {1:F4}, {0}\n" +
			"VehicleSpeed, {2:F4}, {0}\n"+ "Mode, {3}\n", time, rpm, vehicleSpeed, autoPilot); //Marine ajout ligne pour mode autoPilot
    }
}

public struct TriggeredEventFrame
{

    public float triggeredEvent1TimeStamp;
    public float triggeredEvent2TimeStamp;
    public float triggeredEvent3TimeStamp;

    public string ToCSV()
    {
        string data = "";
        if (triggeredEvent1TimeStamp > 0f)
        {
            data += string.Format("TriggeredEvent1, 0.0, {0}\n", triggeredEvent1TimeStamp);
        }

        if (triggeredEvent2TimeStamp > 0f)
        {
            data += string.Format("TriggeredEvent2, 0.0, {0}\n", triggeredEvent2TimeStamp);
        }

        if (triggeredEvent3TimeStamp > 0f)
        {
            data += string.Format("TriggeredEvent3, 0.0, {0}\n", triggeredEvent3TimeStamp);
        }
        return data;
    }
}


//Marine
public struct FullDataFrame2
{
	public float time;
	public float rpm;
	public float gearPosActual;
	public float gearPosTarget;
	public float accelleratorPos;
	public float deceleratorPos;
	public float steeringWheelAngle;
	public float vehicleSpeed;
	public bool mode;
	public float position_x;
	public float position_y;
	public float position_z;


	//Marine
	public string ToCSV2()
	{
		string data = string.Format("{0},"+ "{1:F4}," +
			"{2:N}," +
			"{3:N}, " +
			"{4:F4}," +
			"{5:F4}," +
			"{6:F4}," +
			"{7:F4}," +
			"{8:F4}," +
			"{9:F4}," +
			"{10:F4}," +
			"{11}," , time, rpm, gearPosActual, gearPosTarget, accelleratorPos, deceleratorPos, steeringWheelAngle, vehicleSpeed, position_x, position_y, position_z, mode);

		return data;

	}
}
	

[StructLayout(LayoutKind.Sequential)]
public class CANDataCollector : MonoBehaviour {
	private Rigidbody rb;
	private VehicleController vehicleController;
	private TrackController trackController; // Marine

    private DataStreamServer dataStream;

    public const float sendRate = 0.1f;
    private float lastSend = 0f;


    private float lastYaw = 0f;
    private float lastRoll = 0f;

    private float triggerTimeStamp1 = 0f;
    private float triggerTimeStamp2 = 0f;
    private float triggerTimeStamp3 = 0f;

	//Marine
	private string filePath="";

	void Awake() {
		rb = GetComponent<Rigidbody>();
		vehicleController = GetComponent<VehicleController>();
        dataStream = DataStreamServer.Instance;
        lastYaw = transform.localRotation.eulerAngles.y;
        lastRoll = transform.localRotation.eulerAngles.z;
		//Marine
		filePath = CreateFileData ();
	}

    private void OnEnable()
    {
        AppController.Instance.AdminInput.DataStreamEvent1 += OnTriggerEvent1;
        AppController.Instance.AdminInput.DataStreamEvent2 += OnTriggerEvent2;
        AppController.Instance.AdminInput.DataStreamEvent3 += OnTriggerEvent3;
    }

    private void OnDisable()
    {
        if(AppController.IsInstantiated && AppController.Instance.AdminInput != null)
        {
            AppController.Instance.AdminInput.DataStreamEvent1 -= OnTriggerEvent1;
            AppController.Instance.AdminInput.DataStreamEvent2 -= OnTriggerEvent2;
            AppController.Instance.AdminInput.DataStreamEvent3 -= OnTriggerEvent3;
        }
    }

    void OnTriggerEvent1()
    {
        triggerTimeStamp1 = Time.time;
    }

    void OnTriggerEvent2()
    {
        triggerTimeStamp2 = Time.time;
    }

    void OnTriggerEvent3()
    {
        triggerTimeStamp3 = Time.time;
    }
		

    void Update() {
		float yaw = (transform.localRotation.eulerAngles.y - lastYaw) / Time.deltaTime;
		float roll = (transform.localRotation.eulerAngles.z - lastRoll) / Time.deltaTime;
		lastRoll = transform.localRotation.eulerAngles.z;
		lastYaw = transform.localRotation.eulerAngles.y;

		if (Time.time - lastSend < sendRate)
			return;

		float time = Time.time;

		//GearPosActual
		int gear = vehicleController.IsShifting ? -3 : vehicleController.Gear;

		//AcceleratorPedalPos
		int pedalPos = Mathf.RoundToInt (Mathf.Clamp01 (vehicleController.accellInput) * 100);

		//DeceleratorPedalPos
		int brakePos = Mathf.RoundToInt (Mathf.Clamp (-1, 0, vehicleController.accellInput) * 100);

		//SteeringWheelAngle
		int wheelAngle = Mathf.RoundToInt (vehicleController.steerInput * 720);

		//VehicleSpeed
		float kmh = rb.velocity.magnitude * 3.6f;
		//TODO: calculate this from wheel rpm

		FullDataFrame frame = new FullDataFrame () {
			time = time,
			cruiseSpeed = 0f,
			rpm = vehicleController.RPM,
			gearPosActual = gear,
			gearPosTarget = vehicleController.Gear,
			accelleratorPos = pedalPos,
			deceleratorPos = brakePos,
			rollRate = roll,
			steeringWheelAngle = wheelAngle,
			vehicleSpeed = kmh,
			vehicleSpeedOverGround = kmh,
			wheelSpeedFL = vehicleController.WheelFL.rpm * 60,
			wheelSpeedFR = vehicleController.WheelFR.rpm * 60,
			wheelSpeedRL = vehicleController.WheelRL.rpm * 60,
			wheelSpeedRR = vehicleController.WheelRR.rpm * 60,
			yawRate = yaw,
			triggeredEvent1TimeStamp = 0f, 
			autoPilot = TrackController.Instance.IsInAutoPath() //Marine Etat AutoPilot
		};
		dataStream.SendAsText (frame);
				
		if (triggerTimeStamp1 > 0f || triggerTimeStamp2 > 0f || triggerTimeStamp3 > 0f) {
			TriggeredEventFrame eventFrame = new TriggeredEventFrame () {
				triggeredEvent1TimeStamp = triggerTimeStamp1,
				triggeredEvent2TimeStamp = triggerTimeStamp2,
				triggeredEvent3TimeStamp = triggerTimeStamp3
			};

			dataStream.SendAsText (eventFrame);
		}

		//add triggered events if required
		if (triggerTimeStamp1 > 0f) {
			frame.triggeredEvent1TimeStamp = triggerTimeStamp1;
			triggerTimeStamp1 = 0f;
		}
		if (triggerTimeStamp2 > 0f) {
			frame.triggeredEvent2TimeStamp = triggerTimeStamp2;
			triggerTimeStamp2 = 0f;
		}
		if (triggerTimeStamp3 > 0f) {
			frame.triggeredEvent3TimeStamp = triggerTimeStamp3;
			triggerTimeStamp3 = 0f;
		}

		//Marine
		FullDataFrame2 frame2 = new FullDataFrame2 () {
			time = time,
			rpm = vehicleController.RPM,
			gearPosActual = gear,
			gearPosTarget = vehicleController.Gear,
			accelleratorPos = pedalPos,
			deceleratorPos = brakePos,
			steeringWheelAngle = wheelAngle,
			vehicleSpeed = kmh,
			position_x = rb.position.x,
			position_y = rb.position.y,
			position_z = rb.position.z,
			mode = TrackController.Instance.IsInAutoPath()
		};


		dataStream.SendAsText (frame);
		//Marine
		WriteData (filePath, frame.ToCSV());

	}

	//Marine
	public string CreateFileData()
	{	
		string filePath = Application.dataPath + "/drivingData/" + System.DateTime.Now.ToString("dd-MMM-yyyy_HH-mm-ss") + ".txt";	
		//editor writes to one level above to avoid storing in source control
		if(Application.isEditor)
			filePath = Application.dataPath + "/../drivingData/" + System.DateTime.Now.ToString("dd-MMM-yyyy_HH-mm-ss") + ".txt";

		//create the log dir if it doesn't already exist
		var fi = new System.IO.FileInfo(filePath);
		fi.Directory.Create();
		System.IO.File.WriteAllText(filePath, "Time,"+
			"EngineSpeed," +
			"GearPosActual," +
			"GearPosTarget," +
			"AcceleratorPedalPos," +
			"DeceleratorPedalPos," +
			"SteeringWheelAngle," +
			"VehicleSpeed, " +
			"Position X," +
			"Position Y, " +
			"Position Z,  "+
			"Autonomous Mode (T/F)" + "\r\n");

		return filePath;
	}

	public void WriteData(string filePath, string frame)
		{		
		//string filePath = Application.dataPath + "/drivingData/" + System.DateTime.Now.ToString("dd-MMM-yyyy_HH-mm-ss") + ".txt";
		System.IO.File.AppendAllText (filePath, frame + System.Environment.NewLine);
    }
}
