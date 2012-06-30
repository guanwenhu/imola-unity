using UnityEngine;
using System.Collections;

public class TPoseGestureFactory : NIGestureFactory {

    /// this is the time between the first detection of an exit pose gesture and the
    /// time it is considered to have "clicked". This is used to make timed exit poses
    /// gestures. A value of 0 (or smaller) ignores the timing element.
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
        return "TPoseGesture";
    }

    /// this creates the correct object implementation of the tracker
    /// @return the tracker object. 
    protected override NIGestureTracker GetNewTrackerObject()
    {
        TPoseDetector gestureTracker = new TPoseDetector(m_timeToHoldPose,m_maxMoveSpeed,m_angleTolerance,m_timeToSavePoints);
        return gestureTracker;
    }
}
