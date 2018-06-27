///*
// * Copyright (C) 2016, Jaguar Land Rover
// * This program is licensed under the terms and conditions of the
// * Mozilla Public License, version 2.0.  The full text of the
// * Mozilla Public License is at https://www.mozilla.org/MPL/2.0/
// */
//
//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//
//public struct FullDataFrame2
//{
//	public float time;
//	public float rpm;
//	public float gearPosActual;
//	public float gearPosTarget;
//	public float accelleratorPos;
//	public float deceleratorPos;
//	public float steeringWheelAngle;
//	public float vehicleSpeed;
//	public float position_x;
//	public float position_y;
//	public float position_z;
//	public string mode;
//
//
//
//
//	public string ToCSV()
//	{
//		string data = string.Format("{0},"+ "{1:F4}," +
//				"{2:N}," +
//				"{3:N}, " +
//				"{4:F4}," +
//				"{5:F4}," +
//				"{6:F4}," +
//				"{7:F4}," +
//				"{8:F4}," +
//				"{9:F4}," +
//			"{10:F4}," + "{11}"
//			, time, rpm, gearPosActual, gearPosTarget, accelleratorPos, deceleratorPos, steeringWheelAngle, vehicleSpeed, position_x, position_y, position_z, mode);
//			
//					return data;
//
////		//TODO: handle this better
////		if (triggeredEvent1TimeStamp > 0f)
////		{
////			data += string.Format("TriggeredEvent1, 0.0, {0}\n", triggeredEvent1TimeStamp);
////		}
////
////		if (triggeredEvent2TimeStamp > 0f)
////		{
////			data += string.Format("TriggeredEvent2, 0.0, {0}\n", triggeredEvent2TimeStamp);
////		}
////
////		if (triggeredEvent3TimeStamp > 0f)
////		{
////			data += string.Format("TriggeredEvent3, 0.0, {0}\n", triggeredEvent3TimeStamp);
////		}
//
//		//return data;
//
//	}
//
//
//}
//
//
//
//[StructLayout(LayoutKind.Sequential)]
//public class CANDataWrite : MonoBehaviour {
//	private Rigidbody rb;
//	private VehicleController vehicleController;
//
//	public const float sendRate = 0.1f;
//	private float lastSend = 0f;
//
//
//	private float triggerTimeStamp1 = 0f;
//	private float triggerTimeStamp2 = 0f;
//	private float triggerTimeStamp3 = 0f;
//
//	private string filePath = "";
//
//	void Awake() {
//		rb = GetComponent<Rigidbody>();
//		vehicleController = GetComponent<VehicleController>();
//
//		filePath=CreateFileData ();
//	}
//
//	private void OnEnable()
//	{
//		AppController.Instance.AdminInput.DataStreamEvent1 += OnTriggerEvent1;
//		AppController.Instance.AdminInput.DataStreamEvent2 += OnTriggerEvent2;
//		AppController.Instance.AdminInput.DataStreamEvent3 += OnTriggerEvent3;
//	}
//
//	private void OnDisable()
//	{
//		if(AppController.IsInstantiated && AppController.Instance.AdminInput != null)
//		{
//			AppController.Instance.AdminInput.DataStreamEvent1 -= OnTriggerEvent1;
//			AppController.Instance.AdminInput.DataStreamEvent2 -= OnTriggerEvent2;
//			AppController.Instance.AdminInput.DataStreamEvent3 -= OnTriggerEvent3;
//		}
//	}
//
//	void OnTriggerEvent1()
//	{
//		triggerTimeStamp1 = Time.time;
//	}
//
//	void OnTriggerEvent2()
//	{
//		triggerTimeStamp2 = Time.time;
//	}
//
//	void OnTriggerEvent3()
//	{
//		triggerTimeStamp3 = Time.time;
//	}
//
//	void Update() {
//
//
//		if (Time.time - lastSend < sendRate)
//			return;
//
//		float time = Time.time;
//
//		//GearPosActual
//		int gear = vehicleController.IsShifting ? -3 : vehicleController.Gear;
//
//		//AcceleratorPedalPos
//		int pedalPos = Mathf.RoundToInt(Mathf.Clamp01(vehicleController.accellInput) * 100);
//
//		//DeceleratorPedalPos
//		int brakePos = Mathf.RoundToInt(Mathf.Clamp(-1, 0, vehicleController.accellInput) * 100);
//
//		//SteeringWheelAngle
//		int wheelAngle = Mathf.RoundToInt(vehicleController.steerInput * 720);
//
//		//VehicleSpeed
//		float kmh = rb.velocity.magnitude * 3.6f;
//		//TODO: calculate this from wheel rpm
//
//		FullDataFrame2 frame = new FullDataFrame2()
//		{
//			time = time,
//			rpm = vehicleController.RPM,
//			gearPosActual = gear,
//			gearPosTarget = vehicleController.Gear,
//			accelleratorPos = pedalPos,
//			deceleratorPos = brakePos,
//			steeringWheelAngle = wheelAngle,
//			vehicleSpeed = kmh,
//			position_x = rb.position.x,
//			position_y = rb.position.y,
//			position_z = rb.position.z,
//			mode =  TrackController.Instance.IsInAutoPath().ToString()
//		};
//
//		if(triggerTimeStamp1 > 0f || triggerTimeStamp2 > 0f || triggerTimeStamp3 > 0f)
//		{
//			TriggeredEventFrame eventFrame = new TriggeredEventFrame()
//			{
//				triggeredEvent1TimeStamp = triggerTimeStamp1,
//				triggeredEvent2TimeStamp = triggerTimeStamp2,
//				triggeredEvent3TimeStamp = triggerTimeStamp3
//			};
//
//
//		}
//
////		//add triggered events if required
////		if(triggerTimeStamp1 > 0f)
////		{
////			frame.triggeredEvent1TimeStamp = triggerTimeStamp1;
////			triggerTimeStamp1 = 0f;
////		}
////		if (triggerTimeStamp2 > 0f)
////		{
////			frame.triggeredEvent2TimeStamp = triggerTimeStamp2;
////			triggerTimeStamp2 = 0f;
////		}
////		if (triggerTimeStamp3 > 0f)
////		{
////			frame.triggeredEvent3TimeStamp = triggerTimeStamp3;
////			triggerTimeStamp3 = 0f;
////		}
//
//		WriteData (filePath, frame.ToCSV());
//	
//	}
//
//
//		public string CreateFileData()
//		{	
//			string filePath = Application.dataPath + "/drivingData/" + System.DateTime.Now.ToString("dd-MMM-yyyy_HH-mm-ss") + ".txt";	
//			//editor writes to one level above to avoid storing in source control
//			if(Application.isEditor)
//				filePath = Application.dataPath + "/../drivingData/" + System.DateTime.Now.ToString("dd-MMM-yyyy_HH-mm-ss") + ".txt";
//	
//			//create the log dir if it doesn't already exist
//			var fi = new System.IO.FileInfo(filePath);
//			fi.Directory.Create();
//			System.IO.File.WriteAllText(filePath, "Time,"+
//				"EngineSpeed," +
//				"GearPosActual," +
//				"GearPosTarget," +
//				"AcceleratorPedalPos," +
//				"DeceleratorPedalPos," +
//				"SteeringWheelAngle," +
//				"VehicleSpeed, " +
//				"Position X," +
//				"Position Y, " +
//			"Position Z, "+ "Autonomous Mode (T/F)"+"\r\n");
//	
//			return filePath;
//		}
//	
//		public void WriteData(string filePath, string frame)
//			{		
//			System.IO.File.AppendAllText (filePath, frame + System.Environment.NewLine);
//	    }
//}