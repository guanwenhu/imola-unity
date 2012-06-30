using System;
using UnityEngine;
using System.Collections;
using OpenNI;

public enum SwipeDirection
{
	SwipeLeft,
	SwipeRight,
	SwipeUp,
	SwipeDown
}
public class SwipeGestureDetector : NIGestureTracker
{	
    // protected members
	protected SwipeDirection m_swipeDirection;
	protected bool m_useRightHand;
	protected float m_swipeMinimalLength = 100.0f; // 10 cm
    protected float m_swipeMaximalHeight = 200.0f; // 20 cm
    protected int m_swipeMininalDuration = 100; // 100 ms
    protected int m_swipeMaximalDuration = 1500; //1500 ms
	protected float m_detectionThreshHold = 1.0f; // 1 second between gestures
	
	
	
	/// the maximum speed (in mm/sec) allowed for each of the relevant joints.
    protected float m_maxMoveSpeed;
    
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
    protected TimedPointList m_pointsHand;
	
	protected NITimedPointSpeedListUtility m_pointsTorso;
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
    public SwipeGestureDetector(SwipeDirection direction, bool useRightHand, 
		float swipeMinimalLength, float swipeMaximalHeight,
		int swipeMininalDuration, int swipeMaximalDuration,
		float detectionThreshHold)
    {
		m_swipeDirection = direction;
		m_useRightHand = useRightHand;
		m_swipeMinimalLength = swipeMinimalLength;
		m_swipeMaximalHeight = swipeMaximalHeight;
		m_swipeMininalDuration = swipeMininalDuration;
		m_swipeMaximalDuration = swipeMaximalDuration;
		m_detectionThreshHold = detectionThreshHold;
		m_maxMoveSpeed = 100f;
        m_timeToHoldPose = 1f;
        m_timeToSavePoints = 0.5f;
        
        m_holdingPose = false;
        m_timeDetectedPose = 0;
        m_firedEvent = false;
        m_pointsHand = new TimedPointList(15);
		m_pointsTorso = new NITimedPointSpeedListUtility (m_timeToSavePoints);
        //m_pointsElbow = new NITimedPointSpeedListUtility(timeToSavePoints);
        //m_pointsShoulder = new NITimedPointSpeedListUtility(timeToSavePoints);
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
		return 1.0f;
        //float diffTime = Time.time - m_timeDetectedPose;
        //if (diffTime >= minimumGesturePeriod)
        //    return 1.0f;
        //return diffTime / minimumGesturePeriod;
    }

    /// used for updating every frame
    /// @note While we are still steady (i.e. we haven't gotten a "not steady" event) we update
    /// the time and frame every frame!
    public override void UpdateFrame()
    {
        if (FillPoints() == false) {
			m_holdingPose = false;
			return;
		}
            
        int numPoints;
        bool foundPos = TestSwipePose(out numPoints);
        if (numPoints < 2) {
			m_holdingPose = false;
			return; // we don't have enough points to make a decision
		}
            
		//Debug.Log("foundPos=" + foundPos + ";holdingPose=" + m_holdingPose);
        if (foundPos == false)
        {
            m_holdingPose = false;
            return;
        }
        // now we know we have found a pose.
		
        // first time of the pose since last we didn't have a pose
        /*
		if (m_holdingPose == false)
        {
            Debug.Log("holdingPose false and found gesture");
			m_holdingPose = true;
            m_timeDetectedPose = Time.time;
            m_firedEvent = false;
        } else if (Time.time - m_timeDetectedPose > minimumGesturePeriod){
			Debug.Log("holdingPose true and found gesture,but long enough");
			m_timeDetectedPose = Time.time;
            m_firedEvent = false;
		} else {
			Debug.Log("holdingPose true and found gesture,but not long enough");
			m_firedEvent = true;
		}*/
		if (Time.time - m_timeDetectedPose > m_detectionThreshHold){
			//Debug.Log("holdingPose true and found gesture,but long enough");
			//m_timeDetectedPose = Time.time;
			m_holdingPose = true;
            m_firedEvent = false;
		} else if (m_holdingPose == false) {
			m_holdingPose = true;
		} else {
			//Debug.Log("holdingPose true and found gesture,but not long enough");
			m_firedEvent = true;
		}
		m_timeDetectedPose = Time.time;
		InternalFireDetectEvent();

        //float diffTime = Time.time - m_timeDetectedPose;
        //if (diffTime >= m_timeToHoldPose || m_timeToHoldPose <= 0)
        //{
        //   Debug.Log("update 5");
		//	InternalFireDetectEvent();
        //}
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
		SkeletonJointPosition torso;
        // We need to figure out if we have a good confidence on all joints
		if(m_useRightHand)
		{
        	SkeletonJointPosition rightHand;	
			
			if(player.GetSkeletonJointPosition(SkeletonJoint.RightHand,out rightHand) == false || rightHand.Confidence <= 0.5f)
            	return false;
        	
			
			if(player.GetSkeletonJointPosition(SkeletonJoint.Torso,out torso) == false || torso.Confidence <= 0.5f)
            	return false;	
			
			Vector3 pos = NIConvertCoordinates.ConvertPos(rightHand.Position);
        	m_pointsHand.AddPoint(ref pos);

        	pos = NIConvertCoordinates.ConvertPos(torso.Position);
        	m_pointsTorso.AddPoint(ref pos);
        	
		}
		else
		{
	        SkeletonJointPosition leftHand;		
	
	        if(player.GetSkeletonJointPosition(SkeletonJoint.LeftHand,out leftHand) == false || leftHand.Confidence <= 0.5f)
	            return false;
	
	        if(player.GetSkeletonJointPosition(SkeletonJoint.Torso,out torso) == false || torso.Confidence <= 0.5f)
            	return false;	
			
			Vector3 pos = NIConvertCoordinates.ConvertPos(leftHand.Position);
        	m_pointsHand.AddPoint(ref pos);

        	pos = NIConvertCoordinates.ConvertPos(torso.Position);
        	m_pointsTorso.AddPoint(ref pos);
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
        Vector3 curSpeed = m_pointsTorso.GetAvgSpeed(m_timeToHoldPose, out numPoints);
		//Debug.Log("speed of torso-" + curSpeed + ";number of points-" + numPoints);
		
        if (curSpeed.magnitude > m_maxMoveSpeed)
            return false; // we are moving
		
		return true;
    }

    /// this method tests the current point to figure out if we are in an swipe gesture
    /// @param numPoints the number of points tested
    /// @return true if we found the swipe
    /// @note we assume the number of points is the same for all joints
    protected bool TestSwipePose(out int numPoints)
    {
        //if (IsSteady(out numPoints) == false)
        //    return false;
		// Swipe to right
		//Debug.Log(m_pointsHand.GetDebugString());
		numPoints = m_pointsHand.Points.Count;
		if (numPoints < 2) return false;
		
		if (m_swipeDirection == SwipeDirection.SwipeRight) 
		{
	        return (ScanPositions((p1, p2) => Math.Abs(p2.y - p1.y) < m_swipeMaximalHeight, // Height
	            (p1, p2) => p2.x - p1.x > -10.0f, // Progression to right
	            (p1, p2) => Math.Abs(p2.x - p1.x) > m_swipeMinimalLength, // Length
	            m_swipeMininalDuration, m_swipeMaximalDuration)); // Duration
        
		}
        
		if (m_swipeDirection == SwipeDirection.SwipeLeft)
		{
			return (ScanPositions((p1, p2) => Math.Abs(p2.y - p1.y) < m_swipeMaximalHeight,  // Height
                (p1, p2) => p2.x - p1.x < 10.0f, // Progression to right
                (p1, p2) => Math.Abs(p2.x - p1.x) > m_swipeMinimalLength, // Length
                m_swipeMininalDuration, m_swipeMaximalDuration)); // Duration
            
		}
        return false;
    }
	
	protected bool ScanPositions(Func<Vector3, Vector3, bool> heightFunction, Func<Vector3, Vector3, bool> directionFunction, 
            Func<Vector3, Vector3, bool> lengthFunction, int minTime, int maxTime)
    {
        int start = 0;

        for (int index = 1; index < m_pointsHand.Points.Count - 1; index++)
        {
            if (!heightFunction(m_pointsHand.Points[0].m_point, m_pointsHand.Points[index].m_point) || 
				!directionFunction(m_pointsHand.Points[index].m_point, m_pointsHand.Points[index+1].m_point))
            {
                start = index;
				//Debug.Log("start=" + start);
            }

            if (lengthFunction(m_pointsHand.Points[index].m_point, m_pointsHand.Points[start].m_point))
            {
                //Debug.Log("length function passed" );
				double totalMilliseconds = (m_pointsHand.Points[index].m_time - m_pointsHand.Points[start].m_time) * 1000;
				//Debug.Log("totalMilliseconds = " + totalMilliseconds );
                if (totalMilliseconds >= minTime && totalMilliseconds <= maxTime)
                {
                    return true;
                }
            }
        }

        return false;
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

