using UnityEngine;
using System.Collections;
using OpenNI;

public class TurnAroundTracker : NIGestureTracker {
	
	enum TurningAngle
	{
		Zero,
		Quater,
		Half,
		Third,
		Full
	};
	
	// protected members
	protected int   m_turnAroundTimes;
	protected bool  m_clockwise;
	protected float m_angleTolerance;
	
    protected SkeletonJointPosition m_jointRightShoulder;
	protected SkeletonJointPosition m_jointLeftShoulder;
	
	// private members
	private TurningAngle m_turningAngle;
	private TurningAngle m_prevTurningAngle;
	private Vector3 m_baseVector;
	private bool m_first;
	private int m_count;
	private bool m_ccDir; // true - clockwise; false - counter-clockwise
	
	public override float GestureInProgress()
	{
		float progress = 0;
		
		switch(m_turningAngle)
		{
			case TurningAngle.Zero:
				progress = 0;
			break;
			
			case TurningAngle.Quater:
				progress = 0.25f;
			break;
			
			case TurningAngle.Half:
				progress = 0.5f;
			break;
			
			case TurningAngle.Third:
				progress = 0.75f;
			break;
			
			case TurningAngle.Full:
				progress = 1;
			break;
		}
		
		if(progress > 0)
		{
			if(m_ccDir == false)
				progress *= -1; // minor progress means turning around counter-clockwise
		}
		
		return progress;
	}
	
	public override void ReleaseGesture ()
	{
		m_pointTracker = null;
	}
	
	protected override bool InternalInit (NIPointTracker hand)
	{
        NISkeletonTracker curHand = hand as NISkeletonTracker;
        if (curHand == null)
            return false;
        return true;
	}
	
	protected bool VeridateJoints()
	{
		NISkeletonTracker hand = m_pointTracker as NISkeletonTracker;
		if(hand == null)
			return false;
		
		NISelectedPlayer player = hand.GetTrackedPlayer();
        if (player == null || player.Valid == false || player.Tracking == false)
            return false; // no player to work with...
		
		SkeletonJointPosition rightShoulder;
        SkeletonJointPosition leftShoulder;
		
        if(player.GetSkeletonJointPosition(SkeletonJoint.RightShoulder,out rightShoulder) == false || rightShoulder.Confidence <= 0.5f)
            return false;
        if(player.GetSkeletonJointPosition(SkeletonJoint.LeftShoulder,out leftShoulder) == false || leftShoulder.Confidence <= 0.5f)
            return false;
		
		m_jointRightShoulder = rightShoulder;
		m_jointLeftShoulder = leftShoulder;
		
		return true;
	}
	
	protected void TrackGesture()
	{
		Vector3 rightShoulderPos = NIConvertCoordinates.ConvertPos(m_jointRightShoulder.Position);
		Vector3 leftShoulderPos = NIConvertCoordinates.ConvertPos(m_jointLeftShoulder.Position);
		
		// shoulders	
		Vector3 shouldersVect = rightShoulderPos - leftShoulderPos;
		
		if(m_first)
		{
			m_first = false;
			m_baseVector = shouldersVect; // take the vector at the begining for future measurement.
			return;
		}
		
		float turnedAngle = Vector3.Angle(shouldersVect.normalized, m_baseVector.normalized);
		
		if(Mathf.Abs(turnedAngle) < m_angleTolerance)
		{
			if(m_prevTurningAngle == TurnAroundTracker.TurningAngle.Third)
				m_turningAngle = TurnAroundTracker.TurningAngle.Full;
			else
				m_turningAngle = TurnAroundTracker.TurningAngle.Zero;
		}
		
		if((90 - Mathf.Abs(turnedAngle)) < m_angleTolerance)
		{
			if(m_prevTurningAngle == TurnAroundTracker.TurningAngle.Zero)
			{
				m_turningAngle = TurnAroundTracker.TurningAngle.Quater;
				
				if(m_jointRightShoulder.Position.Z > m_jointLeftShoulder.Position.Z)
					m_ccDir = true;
				else
					m_ccDir = false;
			}
			
			if(m_prevTurningAngle == TurnAroundTracker.TurningAngle.Half)
			{
				if(m_jointRightShoulder.Position.Z > m_jointLeftShoulder.Position.Z)
				{
					if(m_ccDir)
						m_turningAngle = TurnAroundTracker.TurningAngle.Quater;
					else
						m_turningAngle = TurnAroundTracker.TurningAngle.Third;
				}
				else
				{
					if(m_ccDir)
						m_turningAngle = TurnAroundTracker.TurningAngle.Third;
					else
						m_turningAngle = TurnAroundTracker.TurningAngle.Quater;
				}
			}
		}
		
		if(180 - Mathf.Abs(turnedAngle) < m_angleTolerance)
		{
			m_turningAngle = TurnAroundTracker.TurningAngle.Half;
		}
		
		m_prevTurningAngle = m_turningAngle;
		
		//Debug.Log("angle = " + turnedAngle + "state= " + m_turningAngle);
	}
	
	public override void UpdateFrame()
	{
		if(VeridateJoints() == false)
			return;
		
		TrackGesture();
		
		if(m_turningAngle == TurnAroundTracker.TurningAngle.Full)
		{
			// TODO: check turning around times. currently support 1 time only.
			
			//Debug.Log("fired...");
			DetectGesture();
		}
	}
	
	public TurnAroundTracker(int times, float angleTolerance)
	{
		m_turnAroundTimes = times;
		m_angleTolerance = angleTolerance;
		
		m_first = true;
		m_turningAngle = TurnAroundTracker.TurningAngle.Zero;
		m_prevTurningAngle = TurnAroundTracker.TurningAngle.Zero;
	}
	
}
