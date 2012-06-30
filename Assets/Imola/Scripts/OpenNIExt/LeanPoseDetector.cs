using UnityEngine;
using System.Collections;
using OpenNI;

public class LeanPoseDetector : NIGestureTracker {
	
	enum LeanDir
	{
		Left,
		Right,
		Forward,
		Backward,
		Center
	};
	
	protected float m_minLeanAngle;
	
	protected Vector3 m_rightShoulder;
	protected Vector3 m_leftShoulder;
	protected Vector3 m_rightHip;
	protected Vector3 m_leftHip;
	
	private float m_leanRangeAngle;
	private LeanDir m_leanDir;
	
	public LeanPoseDetector(float minLeanAngle, float leanRangeAngle)
	{
		m_minLeanAngle = minLeanAngle;
		m_leanRangeAngle = leanRangeAngle;
		
		m_leanDir = LeanPoseDetector.LeanDir.Center;
	}
	
	public override void ReleaseGesture ()
	{
		m_pointTracker = null;
	}
	
	public override float GestureInProgress ()
	{
		float res = 0f;
		// 0 = left; 0.25 = forward; 0.5 = right; 1 = backward
		if(m_leanDir == LeanPoseDetector.LeanDir.Left)
			res = 0.25f;
		if(m_leanDir == LeanPoseDetector.LeanDir.Forward)
			res = 0.5f;
		if(m_leanDir == LeanPoseDetector.LeanDir.Right)
			res = 0.75f;
		if(m_leanDir == LeanPoseDetector.LeanDir.Backward)
			res = 1.0f;
		
		return res;	
	}
	
	public override void UpdateFrame ()
	{
		m_leanDir = LeanPoseDetector.LeanDir.Center;
		
		if(CheckPoints() == false)
			return;
		
		bool foundPos = TestLeanPose();
		if(foundPos == false)
			return;
	}
	
	protected override bool InternalInit (NIPointTracker hand)
	{
		NISkeletonTracker curHand = hand as NISkeletonTracker;
        if (curHand == null)
            return false;
        return true;
	}
	
	protected bool CheckPoints()
	{
		NISkeletonTracker hand = m_pointTracker as NISkeletonTracker;
		if(hand == null)
			return false; // no hand to track
		
		NISelectedPlayer player = hand.GetTrackedPlayer();
        if (player == null || player.Valid == false || player.Tracking == false)
            return false; // no player to work with...

        // We need to figure out if we have a good confidence on all joints
		SkeletonJointPosition rightShoulder;
        SkeletonJointPosition leftShoulder;
        SkeletonJointPosition rightHip;
        SkeletonJointPosition leftHip;
        if(player.GetSkeletonJointPosition(SkeletonJoint.RightShoulder,out rightShoulder)==false || rightShoulder.Confidence <= 0.5f)
            return false;
        if(player.GetSkeletonJointPosition(SkeletonJoint.LeftShoulder,out leftShoulder)==false || leftShoulder.Confidence <= 0.5f)
            return false;
        if(player.GetSkeletonJointPosition(SkeletonJoint.RightHip,out rightHip)==false || rightHip.Confidence <= 0.5f)
            return false;
        if(player.GetSkeletonJointPosition(SkeletonJoint.LeftHip,out leftHip)==false || leftHip.Confidence <= 0.5f)
            return false;
        Vector3 pos = NIConvertCoordinates.ConvertPos(rightShoulder.Position);
		m_rightShoulder = pos;
        pos = NIConvertCoordinates.ConvertPos(leftShoulder.Position);
		m_leftShoulder = pos;
        pos = NIConvertCoordinates.ConvertPos(rightHip.Position);
		m_rightHip = pos;
        pos = NIConvertCoordinates.ConvertPos(leftHip.Position);
		m_leftHip = pos;
        return true;
	}
	
	protected bool TestLeanPose()
	{
		Vector3 midShoulder = (m_rightShoulder - m_leftShoulder)/2 + m_leftShoulder;
		Vector3 midHip = (m_rightHip - m_leftHip)/2 + m_leftHip;
		
		Vector3 trunk = midShoulder - midHip;
		float leanAngle = Vector3.Angle(Vector3.up, trunk.normalized);
		
		if(Mathf.Abs(leanAngle) < m_minLeanAngle)
			return false;
		
		// learn direction angle, Vector3.right is the base.
		float leanHorizontalAngle = Vector3.Angle(trunk.normalized - Vector3.up, Vector3.right);
		float leanVerticalAngle = Vector3.Angle(trunk.normalized - Vector3.up, Vector3.forward);
		
		if(Mathf.Abs(leanHorizontalAngle) < m_leanRangeAngle / 2)
		{
			m_leanDir = LeanPoseDetector.LeanDir.Right; // mirror;
		}
		if(Mathf.Abs(leanHorizontalAngle - 180.0f) < m_leanRangeAngle / 2)
		{
			m_leanDir = LeanPoseDetector.LeanDir.Left; // mirror;
		}
		
		if(Mathf.Abs(leanVerticalAngle) < m_leanRangeAngle / 2)
		{
			m_leanDir = LeanPoseDetector.LeanDir.Forward; // mirror;
		}
		if(Mathf.Abs(leanVerticalAngle - 180.0f) < m_leanRangeAngle / 2)
		{
			m_leanDir = LeanPoseDetector.LeanDir.Backward; // mirror;
		}
		
		//Debug.Log("HAngle="+leanHorizontalAngle+" VAngle="+leanVerticalAngle+" dir="+m_leanDir);
		return true;
	}
}
