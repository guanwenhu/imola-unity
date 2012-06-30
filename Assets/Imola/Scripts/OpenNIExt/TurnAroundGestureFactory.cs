using UnityEngine;
using System.Collections;

public class TurnAroundGestureFactory : NIGestureFactory {

	public int   m_times;     // how many times to turn around
	public float m_angleTolerance;
	
	public override string GetGestureType()
	{
		return "TurnAroundGesture";
	}
	
	protected override NIGestureTracker GetNewTrackerObject ()
	{
		return new TurnAroundTracker(m_times, m_angleTolerance);
	}
}
