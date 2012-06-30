using System;
using UnityEngine;
using OpenNI;

namespace ImolaNI
{
	/// <summary>
	/// Detect a gesture from NIInput.
	/// </summary>
	public class InputGestureDetector
	{
		// The gesture name defined in input controller,
		// such as NIGUI_WAVE
		private String m_gesture;
		private NIInput m_input;
		private float m_lastDetectedTime = -1f;
		private bool m_currentDetected; //is currently detected?
		private float m_inputValue;
		public float DetectionThreshHold = 0.8f; // 1 second between two gestures
		// Readonly input value
		public float InputValue
	    {
	        get { return m_inputValue; }
	    }
		public InputGestureDetector (NIInput input, String gesture)
		{
			m_input = input;
			m_gesture = gesture;
		}
		
		public bool Detect ()
		{
			
			m_inputValue = m_input.GetAxis (m_gesture);
			//Debug.Log ("current gesture " + m_gesture + " value is " + val + " and checked is " + currentDetected);
			
			if (m_inputValue >= 1.0f) 
			{
				if (m_currentDetected) 
				{
					//Debug.Log ("ignore duplicated " + m_gesture);
					return false;
				} 
				if (Time.time - m_lastDetectedTime >= DetectionThreshHold)
				{
					//buttonPressedMessage = gesture + " has been detected value =" + val;
					Debug.Log ("Fired:" + m_gesture);
					m_currentDetected = true;
					m_lastDetectedTime = Time.time;
					return true;
				}
			
			}
			//if (val > 0) {
				// The value is between 0 and 1 so we have detected the gesture/pose and are waiting
				// for it to continue long enough. 
				// Just pop up a message telling the user to continue holding the pose...
				//buttonPressedMessage = "Hold the " + gesture + " pose until " + val + " reaches 1";
				//currentGestureChecked = false;
			//} 
			m_currentDetected = false;
			return false;
		}
	}
}

