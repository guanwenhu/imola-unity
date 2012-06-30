/****************************************************************************
*                                                                           *
*  OpenNI Unity Toolkit                                                     *
*  Copyright (C) 2011 PrimeSense Ltd.                                       *
*                                                                           *
*                                                                           *
*  OpenNI is free software: you can redistribute it and/or modify           *
*  it under the terms of the GNU Lesser General Public License as published *
*  by the Free Software Foundation, either version 3 of the License, or     *
*  (at your option) any later version.                                      *
*                                                                           *
*  OpenNI is distributed in the hope that it will be useful,                *
*  but WITHOUT ANY WARRANTY; without even the implied warranty of           *
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the             *
*  GNU Lesser General Public License for more details.                      *
*                                                                           *
*  You should have received a copy of the GNU Lesser General Public License *
*  along with OpenNI. If not, see <http://www.gnu.org/licenses/>.           *
*                                                                           *
****************************************************************************/
using UnityEngine;
using OpenNI;
using System.Collections.Generic;

/// @brief Utility class to calculate the normal of the floor
/// @ingroup OpenNISamples
public class NIFloorNormalCalculator : MonoBehaviour
{
	/// a link to the object with the NI context. We will be following the user tracking from here.
    public OpenNISettingsManager m_context;
	
    /// mono-behavior Start for initialization
    public void Start()
    {
		if (m_context == null)
            m_context = FindObjectOfType(typeof(OpenNISettingsManager)) as OpenNISettingsManager;
        if (m_manager == null)
            m_manager = FindObjectOfType(typeof(OpenNISettingsManager)) as OpenNISettingsManager;
        if (m_manager == null)
            throw new System.Exception("Cannot do anything without an OpenNISettingsManager object");
    }

    /// This is the mono-behavior Update.<br>
    /// This method checks if the floor normal is already calculated. If not, it checks if
    /// we have a calibrated user. It then goes over the various calibrated users and attempts
    /// to find one which has legal head and torso positions. Once it finds one it creates the normal
    /// from them.<br>
    /// @note The assumption is that immediately after calibration, the calibrated user stands erect
    /// and that the floor's is leveled. In addition, we assume that even if the joint's confidence is
    /// low, as long as the value is legal (i.e. non zero and is a number) then it is a legal position
    /// (as was defined in the calibration).
    public void Update()
    {
        if (NIConvertCoordinates.NormalUpdated)
            return; // nothing to do here, already got a normal.

        if (m_manager.m_useSkeleton == false || m_manager.UserSkeletonValid == false)
            return; // we don't have a valid user generator to work with
        // get the list of users
		int[] users = m_context.UserGenrator.GetUserIds();
		if(users.Length == 0)
			return; // no users
		
        //IList<int> userList = m_manager.UserGenrator.;
		foreach (int userId in users)
		{
			if (userId < 0)
                continue; // an invalid user
			if(m_manager.UserGenrator.Skeleton==null)
                continue; // no skeleton
            if (m_manager.UserGenrator.Skeleton.IsJointAvailable(SkeletonJoint.Head) == false ||
                m_manager.UserGenrator.Skeleton.IsJointAvailable(SkeletonJoint.Torso) == false)
                return; // we don't have the relevant joints.
            SkeletonJointTransformation skelTransform;
            skelTransform = m_manager.UserGenrator.Skeleton.GetSkeletonJoint(userId, SkeletonJoint.Head);
            Vector3 headPos = NIConvertCoordinates.ConvertPos(skelTransform.Position.Position);
            float mag=headPos.sqrMagnitude;
            if(float.IsNaN(mag) || float.IsInfinity(mag))
                continue; // not a good number
            if(headPos.z>=0)
                continue; // this is not a good value
            skelTransform = m_manager.UserGenrator.Skeleton.GetSkeletonJoint(userId, SkeletonJoint.Torso);
            Vector3 torsoPos = NIConvertCoordinates.ConvertPos(skelTransform.Position.Position);
            mag = torsoPos.sqrMagnitude;
            if (float.IsNaN(mag) || float.IsInfinity(mag))
                continue; // not a good number
            if (torsoPos.z >= 0)
                continue; // this is not a good value
            // if we are here we have two good values.
            NIConvertCoordinates.UpdateFloorNormal(headPos - torsoPos, false);
            //m_manager.Log("Updated floor normal", NIEventLogger.Categories.Initialization, NIEventLogger.Sources.Skeleton);
		}
    }

    /// the settings manager (which holds the users and skeletons).
    public OpenNISettingsManager m_manager;
}
