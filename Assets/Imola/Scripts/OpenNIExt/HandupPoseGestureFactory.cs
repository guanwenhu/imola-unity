using UnityEngine;
using System.Collections;

public class HandupPoseGestureFactory : NIGestureFactory
{
	
	public bool m_useRightHand;

    public float m_timeToHoldPose;
    /// @param maxMoveSpeed the maximum speed (in mm/sec) allowed for each of the relevant joints.
    public float m_maxMoveSpeed;
    /// @param timeToSavePoints the time we use to average points
    public float m_timeToSavePoints;
	    
	public float m_angleTolerance;

    /// returns a unique name for the gesture type.
    /// @note this is what is used to identify the factory
    /// @return the unique name.
    public override string GetGestureType()
	{
		return "HandupPoseGesture";
	}
	
    /// this creates the correct object implementation of the tracker
    /// @return the tracker object. 
    protected override NIGestureTracker GetNewTrackerObject()
	{
		HandupPoseDetector gestureTracker = new HandupPoseDetector(m_useRightHand, m_timeToHoldPose, m_maxMoveSpeed, m_angleTolerance, m_timeToSavePoints);
		return gestureTracker;
	}
}