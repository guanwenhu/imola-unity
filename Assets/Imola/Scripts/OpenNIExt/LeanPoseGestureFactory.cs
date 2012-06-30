using UnityEngine;
using System.Collections;

public class LeanPoseGestureFactory : NIGestureFactory {
	
	public float m_minLeanAngle;
	public float m_leanRangeAngle;
	
	public override string GetGestureType ()
	{
		return "LeanPoseGesture";
	}
	
	protected override NIGestureTracker GetNewTrackerObject ()
	{
		LeanPoseDetector gestureTracker = new LeanPoseDetector(m_minLeanAngle, m_leanRangeAngle);
		return gestureTracker;
	}
}
