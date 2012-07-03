using UnityEngine;
using System.Collections;

/// @brief An initializer to make sure the cursor on the object it is attached to (a prefab)
/// will register itself on the NIGUI.
/// @ingroup OpenNIGUIUtiliites
[RequireComponent(typeof(NIGUICursorExt))]
public class NICursorInitializerExt : MonoBehaviour
{
	public bool m_rightHand = true;
	/// Just an initialization to get the cursor from the prefab and set it to NIGUI
	void Awake ()
	{
		NIGUICursorExt[] cursors = gameObject.GetComponents<NIGUICursorExt> ();
		foreach (NIGUICursorExt cursor in cursors) {
			if (m_rightHand && cursor.m_rightHand) {
				NIGUIExt.AddCursor(cursor);
				NIGUIExt.ResetGroups(); // to make sure we don't have any baggages..
				return;
			}
		}
	}
}

