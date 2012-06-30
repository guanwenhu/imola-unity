using UnityEngine;
using System.Collections;
using OpenNI;

public class HandupPoseDetector : NIGestureTracker
{
    
    /// Release the gesture
    public override void ReleaseGesture()
    {
        m_pointTracker = null;
    }

    /// base constructor
    /// @param timeToHoldPose the time the user is required to hold the pose.
    /// @param maxMoveSpeed the maximum speed (in mm/sec) allowed for each of the relevant joints.
    /// @param angleTolerance This is the allowed tolerance in degrees 
    /// @param timeToSavePoints the time we use to average points
    public HandupPoseDetector(bool useRightHand, float timeToHoldPose, float maxMoveSpeed, float angleTolerance, float timeToSavePoints)
    {
		m_useRightHand = useRightHand;
        m_maxMoveSpeed = maxMoveSpeed;
        m_timeToHoldPose = timeToHoldPose;
        m_timeToSavePoints = timeToSavePoints;
        m_angleTolerance = angleTolerance;
        m_holdingPose = false;
        m_timeDetectedPose = 0;
        m_firedEvent = false;
        m_pointsHand = new NITimedPointSpeedListUtility(timeToSavePoints);
        m_pointsElbow = new NITimedPointSpeedListUtility(timeToSavePoints);
        m_pointsShoulder = new NITimedPointSpeedListUtility(timeToSavePoints);
    }

    /// This is true if the gesture is in the middle of doing (i.e. it has detected but not gone out of the gesture).
    /// for our purposes this means the steady event has occurred and the unsteady has not occurred yet
    /// @return a value between 0 and 1. 0 means no pose, 1 means the pose has been detected and held 
    /// for a while. a value in the middle means the pose has been detected and has been held this
    /// portion of the time required to fire the trigger (@ref m_timeToHoldPose).
    public override float GestureInProgress()
    {
        if (m_holdingPose == false)
            return 0.0f;
        float diffTime = Time.time - m_timeDetectedPose;
        if (diffTime >= m_timeToHoldPose || m_timeToHoldPose <= 0)
            return 1.0f;
        return diffTime / m_timeToHoldPose;
    }

    /// used for updating every frame
    /// @note While we are still steady (i.e. we haven't gotten a "not steady" event) we update
    /// the time and frame every frame!
    public override void UpdateFrame()
    {
        if (FillPoints() == false)
            return;
        int numPoints;
        bool foundPos = TestHandupPose(out numPoints);
        if (numPoints < 1)
            return; // we don't have enough points to make a decision

        if (foundPos == false)
        {
            m_holdingPose = false;
            return;
        }
        // now we know we have found a pose.

        // first time of the pose since last we didn't have a pose
        if (m_holdingPose==false)
        {
            m_holdingPose = true;
            m_timeDetectedPose = Time.time;
            m_firedEvent = false;
        }


        float diffTime = Time.time - m_timeDetectedPose;
        if (diffTime >= m_timeToHoldPose || m_timeToHoldPose <= 0)
        {
            InternalFireDetectEvent();
        }
    }
    
    // protected methods

    /// this method tries to fill a new point on each of the relevant joints.
    /// It returns true if it succeed and false otherwise
    /// @note it will fail if even one of the points has a low confidence!
    /// @return true on success, false on failure.
    protected bool FillPoints()
    {
        // first we find a reference to the skeleton capability
        NISkeletonTracker hand = m_pointTracker as NISkeletonTracker;
        if (hand == null)
            return false; // no hand to track
        NISelectedPlayer player = hand.GetTrackedPlayer();
        if (player == null || player.Valid == false || player.Tracking == false)
            return false; // no player to work with...

        // We need to figure out if we have a good confidence on all joints
		if(m_useRightHand)
		{
        	SkeletonJointPosition rightHand;	
			SkeletonJointPosition rightElbow;
			SkeletonJointPosition rightShoulder;
			
			if(player.GetSkeletonJointPosition(SkeletonJoint.RightHand,out rightHand) == false || rightHand.Confidence <= 0.5f)
            	return false;
        	
			if(player.GetSkeletonJointPosition(SkeletonJoint.RightElbow,out rightElbow) == false || rightElbow.Confidence <= 0.5f)
            	return false;		
			
			if(player.GetSkeletonJointPosition(SkeletonJoint.RightShoulder,out rightShoulder) == false || rightShoulder.Confidence <= 0.5f)
            	return false;	
			
			Vector3 pos = NIConvertCoordinates.ConvertPos(rightHand.Position);
        	m_pointsHand.AddPoint(ref pos);

        	pos = NIConvertCoordinates.ConvertPos(rightElbow.Position);
        	m_pointsElbow.AddPoint(ref pos);
        	
			pos = NIConvertCoordinates.ConvertPos(rightShoulder.Position);
        	m_pointsShoulder.AddPoint(ref pos);
		}
		else
		{
	        SkeletonJointPosition leftHand;
	        SkeletonJointPosition leftElbow;
			SkeletonJointPosition leftShoulder;			
	
	        if(player.GetSkeletonJointPosition(SkeletonJoint.LeftHand,out leftHand) == false || leftHand.Confidence <= 0.5f)
	            return false;
	
	        if(player.GetSkeletonJointPosition(SkeletonJoint.LeftElbow,out leftElbow) == false || leftElbow.Confidence <= 0.5f)
	            return false;	

			if(player.GetSkeletonJointPosition(SkeletonJoint.LeftShoulder,out leftShoulder) == false || leftShoulder.Confidence <= 0.5f)
            	return false;	
			
			Vector3 pos = NIConvertCoordinates.ConvertPos(leftHand.Position);
        	m_pointsHand.AddPoint(ref pos);

        	pos = NIConvertCoordinates.ConvertPos(leftElbow.Position);
        	m_pointsElbow.AddPoint(ref pos);
			
        	pos = NIConvertCoordinates.ConvertPos(leftShoulder.Position);
        	m_pointsShoulder.AddPoint(ref pos);
		}
		
        return true;
    }

    /// this method tests if we are relatively steady (i.e. our speed is low).
    /// In order to be steady, all relevant joints must move very slowly.
    /// @param numPoints the number of points tested
    /// @return true if we are steady and false otherwise
    /// @note we assume the number of points is the same for all joints
    protected virtual bool IsSteady(out int numPoints)
    {
        Vector3 curSpeed = m_pointsHand.GetAvgSpeed(m_timeToHoldPose, out numPoints);
		
        if (curSpeed.magnitude > m_maxMoveSpeed)
            return false; // we are moving
		
        if (numPoints < 1)
            return false; // we assume the number of points is the same for all joints (although speed would have 1 more point than positions)
        
		curSpeed = m_pointsElbow.GetAvgSpeed(m_timeToHoldPose, out numPoints);
        if (curSpeed.magnitude > m_maxMoveSpeed)
            return false; 
		
		curSpeed = m_pointsShoulder.GetAvgSpeed(m_timeToHoldPose, out numPoints);
        if (curSpeed.magnitude > m_maxMoveSpeed)
            return false; 
		
		return true;
    }

    /// this method tests the current point to figure out if we are in an exit pose
    /// @param numPoints the number of points tested
    /// @return true if we found the exit pose
    /// @note we assume the number of points is the same for all joints
    protected bool TestHandupPose(out int numPoints)
    {
        
		if (IsSteady(out numPoints) == false)
            return false;
		
        Vector3 handPos = m_pointsHand.GetAvgPos(m_timeToHoldPose, out numPoints);
        Vector3 elbowPos = m_pointsElbow.GetAvgPos(m_timeToHoldPose, out numPoints);
		Vector3 shoulderPos = m_pointsShoulder.GetAvgPos(m_timeToHoldPose, out numPoints);
		
        Vector3 handDir = handPos - elbowPos;
		Vector3 elbowDir = elbowPos - shoulderPos;
		
		float handAngle = Vector3.Angle(handDir.normalized, Vector3.up);
		
		if(Mathf.Abs(handAngle) > m_angleTolerance)
			return false;
		
		float elbowAngle = Vector3.Angle(elbowDir.normalized, Vector3.up);
		if(Mathf.Abs(elbowAngle) > m_angleTolerance)
			return false;
		
        return true;
    }


    /// Gesture initialization
    /// 
    /// This method is responsible for initializing the gesture to work with a specific hand tracker
    /// @param hand the hand tracker to work with
    /// @return true on success, false on failure (e.g. if the hand tracker does not work with the gesture).
    protected override bool InternalInit(NIPointTracker hand)
    {
        NISkeletonTracker curHand = hand as NISkeletonTracker;
        if (curHand == null)
            return false;
        return true;
    }

    // protected members
	
	protected bool m_useRightHand;
	
    /// the maximum speed (in mm/sec) allowed for each of the relevant joints.
    protected float m_maxMoveSpeed;

    /// the hands are supposed to be at about 45 degrees in each direction. This is the allowed 
    /// tolerance in degrees (i.e. a tolerance of 10 means everything from 35 degrees to 55 degrees is ok
    protected float m_angleTolerance;

    /// the time we need to hold the pose
    protected float m_timeToHoldPose;

    /// the time we need to hold the points
    protected float m_timeToSavePoints;

    /// if this is true then we found the pose
    protected bool m_holdingPose;

    /// the time we first detected the pose (and not lost it since)
    protected float m_timeDetectedPose;

    /// this holds true if we already fired the event
    protected bool m_firedEvent;

    /// this holds the points we are tracking on the hand
    protected NITimedPointSpeedListUtility m_pointsHand;
    /// this holds the points we are tracking on the elbow
    protected NITimedPointSpeedListUtility m_pointsElbow;
    /// this holds the points we are tracking on the shoulder
    protected NITimedPointSpeedListUtility m_pointsShoulder;

    
    /// this marks the result as clicked by updating the time and frame and the first
    /// time after the last change it also fires the gesture event.
    protected virtual void InternalFireDetectEvent()
    {

        m_timeDetected = Time.time;
        m_frameDetected = Time.frameCount;
        if (m_firedEvent == false)
        {
            DetectGesture();
            m_firedEvent = true;
        }
    }	
	
}
