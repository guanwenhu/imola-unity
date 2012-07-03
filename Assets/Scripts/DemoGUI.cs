
using UnityEngine;
using System.Collections;

/// @brief This class is an example for the use of NIGUI
/// 
/// This class creates a sample use of NIGUI with various GUI elements.
/// @ingroup OpenNISpecificLogicSamples
public class DemoGUI : MonoBehaviour 
{
    NIInput m_input; ///< @brief The input we get the axes from.

    /// mono-behavior start - initializes the input
    public void Start()
    {
        m_input = FindObjectOfType(typeof(NIInput)) as NIInput;
    }

    /// mono-behavior OnGUI to show GUI elements
    public void OnGUI()
    {
        // create a regular label
        myRect.x=(Screen.width/2)-40;
        myRect.y=20;
        myRect.width=80;
        myRect.height=30;
        GUI.Label(myRect, "Demo GUI");


        // place the first button
        myRect.x = 200;
        myRect.y = 50;
        myRect.width = 80;
        myRect.height = 40;
        if (NIGUIExt.Button(myRect, "Button1"))
        {
           buttonPressedMessage = "Button 1 was pressed at time=" + Time.time;
        }

        // place the second button
        myRect.x = 300;
        myRect.y = 50;
        if (NIGUIExt.Button(myRect, "Button2"))
        {
            buttonPressedMessage = "Button 2 was pressed at time=" + Time.time;
        }

		// place the toggle button
        myRect.x = 300;
        myRect.y = 100;
        myRect.width = 250;
        m_toggle1 = NIGUIExt.Toggle(myRect, m_toggle1, "Toggle example");

        // place the GUI changed frame
        myRect.x = 50;
        myRect.y = (Screen.height / 2) - 140;
        myRect.width = 250;
        myRect.height = 30;
        GUI.Box(myRect, guiFrameCahngedMessage);

        // place the vScroll value label
        myRect.x = 50;
        myRect.y = (Screen.height / 2) - 100;
        myRect.width = 250;
        myRect.height = 30;
        GUI.Box(myRect, "vScroll=" + vScroll);

        // place the hScroll value label
        myRect.x = 50;
        myRect.y = (Screen.height / 2) - 60;
        myRect.width = 250;
        myRect.height = 30;
        GUI.Box(myRect, "hScroll=" + hScroll);

        // place the button pressed label
        myRect.x = 50;
        myRect.y = (Screen.height/2) - 20;
        myRect.width = 250;
        myRect.height = 30;
        GUI.Box(myRect, buttonPressedMessage);

        // Click axis value label
        myRect.x = 50;
        myRect.y = (Screen.height / 2) + 20;
        myRect.width = 250;
        myRect.height = 30;
        GUI.Box(myRect, "value=" + m_input.GetAxis("NIGUI_CLICK"));


        
   
        // placed the clipped area for the button        
        myRect.x = (Screen.width) - 500;
        myRect.y = (Screen.height / 2) - 225;
        myRect.width = 400;
        myRect.height = 200;
        NIGUIExt.BeginGroup(myRect);
        // place the internal button
        myRect.x=0;
        myRect.y=0;
        Color c = GUI.backgroundColor;
        Color almostClear = c;
        almostClear.a = 0.2f;
        GUI.backgroundColor = almostClear;
        GUI.Box(myRect, "");
        GUI.backgroundColor = c;
        if (NIGUIExt.Button(new Rect(150-hScroll, 50-vScroll, 300, 200), "a button to be clipped by the view"))
            buttonPressedMessage = "Internal button to group was pressed";
        NIGUIExt.EndGroup();

        
        
        // update the GUI changed frame
        if (NIGUIExt.changed)
            guiFrameCahngedMessage = "GUI changed at frame=" + Time.frameCount;


    }

    /// @brief useful members to draw the GUI
    /// 
    /// @{
    private string[] toolbarStrings = new string[] { "Toolbar1", "Toolbar1", "Toolbar3", "Toolbar4", "Toolbar5" };
    private int toolbarInt = 0; 
    private int selectionGridInt = 0;

    private string buttonPressedMessage = "Nothing was pressed yet";
    private string guiFrameCahngedMessage = "GUI last changed at frame 0"; 
    private Rect myRect = new Rect(0, 0, 100, 100); 
    private bool m_toggle1=false; 
    private float hScroll, vScroll;
    private float floatSlider;
    private float intSlider;
    //@}
}
