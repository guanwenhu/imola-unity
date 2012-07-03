
using UnityEngine;
using System.Collections.Generic;

/// @brief This class implements NI aware GUI elements. 
/// 
/// This class is designed to replace the regular GUI static class for all NI aware
/// Only simple GUI elements are implemented and this class will aware of two cursors 
/// which will follow left hand and right hand.
/// elements (e.g. buttons).
/// @ingroup OpenNIGUIUtiliites
public static class NIGUIExt 
{
    /// Method to set the cursor
    /// @note before starting to use NIGUI one MUST set a cursor (the prefab does that by 
    /// setting it in the "awake" method.
    /// @param newCursor the new cursor to add
    public static void AddCursor(NIGUICursor newCursor)
    {
        //m_cursor=newCursor;
		if (m_cursors == null) {
			m_cursors = new List<NIGUICursor>();
		}
		m_cursors.Add(newCursor);
    }

    /// sets the cursor to be active or deactivated
    /// @param state True means NIGUI is active and false means it is not...
    public static void SetActive(bool state)
    {
        //m_cursor.SetActive(state);
		foreach (NIGUICursor cursor in m_cursors) {
			cursor.SetActive(state);
		}
    }


    /// Accessor to the "changed" value.
    public static bool changed
    {
        get
        {
            if (GUI.changed)
                return true;
            if (m_curFrame < Time.frameCount)
            {
                m_curFrame = Time.frameCount;
                m_changedSinceLastFrame = false;
                return false; // we are at a new frame.
            }
            return m_changedSinceLastFrame;
        }
    }


    /// same as GUI.Button but which can handle Natural Interactions
    /// @param pos the rect of where to draw the button
    /// @param text the text to show on the button
    /// @return true if the button was pressed
    public static bool Button(Rect pos, string text)
    {
        if (GUI.Button(pos, text))
            return true;
        bool clicked=TestClicked(ref pos);
        if (clicked)
            MarkChange();
        return clicked;
    }

    /// same as GUI.Button but which can handle Natural Interactions
    /// @param pos the rect of where to draw the button
    /// @param image the texture to show on the button
    /// @return true if the button was pressed
    public static bool Button(Rect pos, Texture image)
    {
        if (GUI.Button(pos, image))
            return true;
        bool clicked = TestClicked(ref pos);
        if (clicked)
            MarkChange();
        return clicked;
    }

    /// same as GUI.Button but which can handle Natural Interactions
    /// @param pos the rect of where to draw the button
    /// @param content the GUIContent to show on the button
    /// @return true if the button was pressed
    public static bool Button(Rect pos, GUIContent content)
    {
        if (GUI.Button(pos, content))
            return true;
        bool clicked = TestClicked(ref pos);
        if (clicked)
            MarkChange();
        return clicked;
    }

    /// same as GUI.Button but which can handle Natural Interactions
    /// @param pos the rect of where to draw the button
    /// @param text the text to show on the button
    /// @param style the GUIStyle to use
    /// @return true if the button was pressed
    public static bool Button(Rect pos, string text, GUIStyle style)
    {
        if (GUI.Button(pos, text, style))
            return true;
        bool clicked = TestClicked(ref pos);
        if (clicked)
            MarkChange();
        return clicked;
    }

    /// same as GUI.Button but which can handle Natural Interactions
    /// @param pos the rect of where to draw the button
    /// @param image the texture to show on the button
    /// @param style the GUIStyle to use
    /// @return true if the button was pressed
    public static bool Button(Rect pos, Texture image, GUIStyle style)
    {
        if (GUI.Button(pos, image, style))
            return true;
        bool clicked = TestClicked(ref pos);
        if (clicked)
            MarkChange();
        return clicked;
    }

    /// same as GUI.Button but which can handle Natural Interactions
    /// @param pos the rect of where to draw the button
    /// @param content the GUIContent to show on the button
    /// @param style the GUIStyle to use
    /// @return true if the button was pressed
    public static bool Button(Rect pos, GUIContent content, GUIStyle style)
    {
        if (GUI.Button(pos, content, style))
            return true;
        bool clicked = TestClicked(ref pos);
        if (clicked)
            MarkChange();
        return clicked;
    }

    

    /// same as GUI.Toggle but which can handle Natural Interactions
    /// @param pos the rect of where to draw the Toggle
    /// @param text the text to show on the Toggle
    /// @param value the value from before (how to draw)
    /// @return true if the Toggle was pressed
    public static bool Toggle(Rect pos, bool value, string text)
    {
        bool tmpValue;
        tmpValue=GUI.Toggle(pos,value, text);
        if (tmpValue != value)
            return tmpValue;
        if (TestClicked(ref pos))
        {
            MarkChange();
            return !tmpValue;
        }
        return tmpValue;
    }

    /// same as GUI.Toggle but which can handle Natural Interactions
    /// @param pos the rect of where to draw the Toggle
    /// @param image the texture to show on the Toggle
    /// @param value the value from before (how to draw)
    /// @return true if the Toggle was pressed
    public static bool Toggle(Rect pos, bool value, Texture image)
    {
        bool tmpValue;
        tmpValue = GUI.Toggle(pos, value, image);
        if (tmpValue != value)
            return tmpValue;
        if (TestClicked(ref pos))
        {
            MarkChange();
            return !tmpValue;
        }
        return tmpValue;
    }

    /// same as GUI.Toggle but which can handle Natural Interactions
    /// @param pos the rect of where to draw the Toggle
    /// @param value the value from before (how to draw)
    /// @param content the GUIContent to show on the Toggle
    /// @return true if the Toggle was pressed
    public static bool Toggle(Rect pos, bool value, GUIContent content)
    {
        bool tmpValue;
        tmpValue = GUI.Toggle(pos, value, content);
        if (tmpValue != value)
            return tmpValue;
        if (TestClicked(ref pos))
        {
            MarkChange();
            return !tmpValue;
        }
        return tmpValue;

    }

    /// same as GUI.Toggle but which can handle Natural Interactions
    /// @param pos the rect of where to draw the Toggle
    /// @param text the text to show on the Toggle
    /// @param value the value from before (how to draw)
    /// @param style the GUIStyle to use
    /// @return true if the Toggle was pressed
    public static bool Toggle(Rect pos, bool value, string text, GUIStyle style)
    {
        bool tmpValue;
        tmpValue = GUI.Toggle(pos, value, text, style);
        if (tmpValue != value)
            return tmpValue;
        if (TestClicked(ref pos))
        {
            MarkChange();
            return !tmpValue;
        }
        return tmpValue;
    }

    /// same as GUI.Toggle but which can handle Natural Interactions
    /// @param pos the rect of where to draw the Toggle
    /// @param value the value from before (how to draw)
    /// @param image the texture to show on the Toggle
    /// @param style the GUIStyle to use
    /// @return true if the Toggle was pressed
    public static bool Toggle(Rect pos, bool value, Texture image, GUIStyle style)
    {
        bool tmpValue;
        tmpValue = GUI.Toggle(pos, value, image, style);
        if (tmpValue != value)
            return tmpValue;
        if (TestClicked(ref pos))
        {
            MarkChange();
            return !tmpValue;
        }
        return tmpValue;
    }

    /// same as GUI.Toggle but which can handle Natural Interactions
    /// @param pos the rect of where to draw the Toggle
    /// @param value the value from before (how to draw)
    /// @param content the GUIContent to show on the Toggle
    /// @param style the GUIStyle to use
    /// @return true if the Toggle was pressed
    public static bool Toggle(Rect pos, bool value, GUIContent content, GUIStyle style)
    {
        bool tmpValue;
        tmpValue = GUI.Toggle(pos, value, content, style);
        if (tmpValue != value)
            return tmpValue;
        if (TestClicked(ref pos))
        {
            MarkChange();
            return !tmpValue;
        }
        return tmpValue;
    }


    /// same as GUI.BeginGroup but which can handle Natural Interactions
    /// @param pos the rect of where to draw the BeginGroup
    static public void BeginGroup(Rect pos)
    {
        GUI.BeginGroup(pos);
        m_groups.Add(pos);
    }

    /// same as GUI.BeginGroup but which can handle Natural Interactions
    /// @param pos the rect of where to draw the BeginGroup
    /// @param text the text to show on the BeginGroup
    static public void BeginGroup(Rect pos, string text)
    {
        GUI.BeginGroup(pos,text);
        m_groups.Add(pos);
    }
    /// same as GUI.BeginGroup but which can handle Natural Interactions
    /// @param pos the rect of where to draw the BeginGroup
    /// @param image the texture to show on the BeginGroup
    static public void BeginGroup(Rect pos, Texture image)
    {
        GUI.BeginGroup(pos,image);
        m_groups.Add(pos);
    }
    /// same as GUI.BeginGroup but which can handle Natural Interactions
    /// @param pos the rect of where to draw the BeginGroup
    /// @param content the GUIContent to show on the BeginGroup
    static public void BeginGroup(Rect pos, GUIContent content)
    {
        GUI.BeginGroup(pos,content);
        m_groups.Add(pos);
    }
    /// same as GUI.BeginGroup but which can handle Natural Interactions
    /// @param pos the rect of where to draw the BeginGroup
    /// @param style the GUIStyle to use
    static public void BeginGroup(Rect pos,GUIStyle style)
    {
        GUI.BeginGroup(pos, style);
        m_groups.Add(pos);
    }
    /// same as GUI.BeginGroup but which can handle Natural Interactions
    /// @param pos the rect of where to draw the BeginGroup
    /// @param text the text to show on the BeginGroup
    /// @param style the GUIStyle to use
    static public void BeginGroup(Rect pos, string text, GUIStyle style)
    {
        GUI.BeginGroup(pos, text, style);
        m_groups.Add(pos);
    }
    /// same as GUI.BeginGroup but which can handle Natural Interactions
    /// @param pos the rect of where to draw the BeginGroup
    /// @param image the texture to show on the BeginGroup
    /// @param style the GUIStyle to use
    static public void BeginGroup(Rect pos, Texture image, GUIStyle style)
    {
        GUI.BeginGroup(pos, image, style);
        m_groups.Add(pos);
    }
    /// same as GUI.BeginGroup but which can handle Natural Interactions
    /// @param pos the rect of where to draw the BeginGroup
    /// @param content the GUIContent to show on the BeginGroup
    /// @param style the GUIStyle to use
    static public void BeginGroup(Rect pos, GUIContent content, GUIStyle style)
    {
        GUI.BeginGroup(pos, content, style);
        m_groups.Add(pos);
    }

    /// same as GUI.EndGroup but which can handle Natural Interactions
    static public void EndGroup()
    {
        GUI.EndGroup();
        m_groups.RemoveAt(m_groups.Count - 1);
    }

    
    /// resets the group (should be called on level load as a group might have been open when the load level is called.
    /// @note this is done by default on the GUI cursor!
    static public void ResetGroups()
    {
        m_groups.Clear();
    }

    /// this is the minimum size (number of pixels) which the slider button can be for the purpose of deciding whether
    /// a click is on it or not.
    static public int m_minSliderButtonSize=20;



    /// Method to check if we clicked since the last found click
    /// @param where the rect of the control to check
    /// @return true when we clicked.
    static private bool TestClicked(ref Rect where)
    {
        if (m_cursors == null || m_cursors.Count == 0)
            throw new System.Exception("You must set a NIGUICursor using NIGUI.AddCursor(newCursor).");
		
		foreach (NIGUICursor cursor in m_cursors) {
			if (TestClicked(cursor, ref where)) {
				return true;
			}
		}
		return false;
    }
	
	static private bool TestClicked(NIGUICursor cursor, ref Rect where) {
		Vector2 pos;
        return TestClicked(cursor, ref where, out pos);
	}
	static private bool TestClicked(NIGUICursor cursor, ref Rect where, out Vector2 lastClickPosition) {
		int lastClickFrame;
        float lastClickTime;
        lastClickTime=cursor.GetLastClickedTime(out lastClickFrame,out lastClickPosition);
        if(lastClickTime<=m_lastClickTimeUsed)
            return false; // we already used or ignored that click.
        if (lastClickFrame < Time.frameCount - 1)
            return false; // if we are two frames back then we sure didn't miss it...
        if (CorrectRectForGroups(ref where) == false)
            return false; // the entire control was clipped.
        if (m_doingSlider)
        {
            if (where.Contains(m_sliderStartPos) == false || where.Contains(m_sliderEndPos) == false)
                return false; // we are doing a slider but are looking at a different controller
        }
        else
        {
            if (where.Contains(lastClickPosition) == false)
                return false; // this is not us
        }
        m_lastClickTimeUsed=Time.time; // we clicked now
        return true;
	}
    


    /// method to correct a rect for groups. Basically a group would create a moving and clipping of
    /// the rect based on the groups before it.
    /// @param rectToCorrect The rect to correct. @note This will CHANGE the rect (even if the rect is clipped)
    /// @return True when the rect is still legal after correction, false if it has been totally clipped.
    static private bool CorrectRectForGroups(ref Rect rectToCorrect)
    {
        for (int i = 0; i < m_groups.Count; i++)
        {
            // move the start position.
            rectToCorrect.x += m_groups[i].x;
            rectToCorrect.y += m_groups[i].y;

            // do the clipping

            // x axis clipping         
            if (rectToCorrect.x < m_groups[i].x)
            {
                rectToCorrect.width += rectToCorrect.x;
                rectToCorrect.x = m_groups[i].x;
            }
            if(rectToCorrect.x+rectToCorrect.width>=m_groups[i].x+m_groups[i].width)
                rectToCorrect.width = m_groups[i].x + m_groups[i].width - rectToCorrect.x; // clip it to reach the end.

            if (rectToCorrect.width <= 0)
                return false; // nothing left

            // y axis clipping         
            if (rectToCorrect.y < m_groups[i].y)
            {
                rectToCorrect.height += rectToCorrect.y;
                rectToCorrect.y = m_groups[i].y;
            }
            if (rectToCorrect.y + rectToCorrect.height >= m_groups[i].y+m_groups[i].height)
                rectToCorrect.height = m_groups[i].y + m_groups[i].height - rectToCorrect.y; // clip it to reach the end.

            if (rectToCorrect.height <= 0)
                return false; // nothing left

        }
        return true;
    }

    /// this utility marks a change in the GUI (should be called whenever there is a change.
    static private void MarkChange()
    {
        m_curFrame = Time.frameCount;
        m_changedSinceLastFrame = true;
    }

    static private int m_curFrame=-1; ///< holds the current frame
    static private bool m_changedSinceLastFrame=false; ///< holds true if there was a change from last frame.
    //static private NIGUICursor m_cursor=null; ///< the cursor
	static private List<NIGUICursor> m_cursors;
    static private float m_lastClickTimeUsed=-1.0f; ///< the time of the last click we actually used to click
    static private List<Rect> m_groups = new List<Rect>(); ///< a list of groups opened by BeginGroup which weren't closed by EndGroup (their rect).
    static private bool m_doingSlider=false;               ///< if this is true then we clicked on the slider and are currently sliding
    ///this is the position of the slider defined and working in @ref m_doingSlider
    ///@note it is defined as the start of the range + the size from the start of the button itself. <br>
    ///To better understand this we can look at an example: Lets say we have a horizontal slider starting from x=100
    ///to x=200. Assume the slider's button itself is size 0. In this case the vector's x position would be 100.
    ///If on the other hand the slider's button itself was 10 pixels wide, then this value would be 100 if we clicked at
    ///its beginning, 105 if we clicked at its middle and 110 if we clicked at its end.
    static private Vector2 m_sliderStartPos=Vector2.zero;

    ///this is the end position of the slider defined and working in @ref m_doingSlider
    ///since more than one slider MIGHT start at the same place we also save the other end...
    static private Vector2 m_sliderEndPos = Vector2.zero;  
}
