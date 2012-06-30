using UnityEngine;
using System.Collections;

public class SwipeGestureFactory : NIGestureFactory
{
	public SwipeDirection m_swipeDirection;
	public bool m_useRightHand;

    //public float m_timeToHoldPose;
    /// @param maxMoveSpeed the maximum speed (in mm/sec) allowed for each of the relevant joints.
    //public float m_maxMoveSpeed;
    /// @param timeToSavePoints the time we use to average points
   // public float m_timeToSavePoints;
	
	public float m_swipeMinimalLength = 100.0f; // 10 cm
    public float m_swipeMaximalHeight = 200.0f; // 20 cm
    public int m_swipeMininalDuration = 100; // 100 ms
    public int m_swipeMaximalDuration = 1500; //1500 ms
	public float m_detectionThreshHold = 1.0f; // 1 second between gestures

    /// returns a unique name for the gesture type.
    /// @note this is what is used to identify the factory
    /// @return the unique name.
    public override string GetGestureType()
	{
		if (m_swipeDirection == SwipeDirection.SwipeLeft)
			return "SwipeLeftGesture";
		if (m_swipeDirection == SwipeDirection.SwipeRight)
			return "SwipeRightGesture";
		return "SwipeGesture";
	}
	
    /// this creates the correct object implementation of the tracker
    /// @return the tracker object. 
    protected override NIGestureTracker GetNewTrackerObject()
	{
		SwipeGestureDetector gestureTracker = new SwipeGestureDetector(m_swipeDirection, m_useRightHand, 
			m_swipeMinimalLength, m_swipeMaximalHeight, m_swipeMininalDuration, m_swipeMaximalDuration,
			m_detectionThreshHold);
		
		return gestureTracker;
	}
}