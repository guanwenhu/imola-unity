using UnityEngine;
using System.Collections.Generic;


/// @brief Utility to hold and analyze points over time.
/// 
/// This class is a utility for handling a list of points (each with a time on it).
public class TimedPointList
{
    /// A struct to hold points and times
    public struct TimedPoint
    {
        public Vector3 m_point; ///< The position
        public float m_time;    ///< The time it was recorded
    }
	readonly List<TimedPoint> points = new List<TimedPoint>();
	
	readonly int windowSize; // Number of recorded positions
	
	public List<TimedPoint> Points
    {
        get { return points; }
    }

    public int WindowSize
    {
        get { return windowSize; }
    }
	
	public TimedPointList(int windowSize = 20)
    {
        this.windowSize = windowSize;
    }
	public virtual void AddPoint(ref Vector3 point)
    {
        
        TimedPoint newPoint;
        newPoint.m_point=point;
        newPoint.m_time=Time.time;
        points.Add(newPoint);
		
		 // Remove too old positions
        if (Points.Count > WindowSize)
        {
            TimedPoint entryToRemove = Points[0];

            Points.Remove(entryToRemove);
        }
    }
	
	public void clear() 
	{
		points.Clear();
	}
	
	public virtual string GetDebugString()
    {
        string str = "used "+points.Count + " points=";
        int numPnt=0;
        foreach (TimedPoint pnt in points)
        {
            str += "" + numPnt + ": " + pnt.m_point + " at" + pnt.m_time + ", ";
            numPnt++;
        }
        return str;
    }
}