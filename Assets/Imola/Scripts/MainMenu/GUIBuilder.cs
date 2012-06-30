using UnityEngine;
using System.Collections;
using OpenNI;
using ImolaNI;

public class GUIBuilder : MonoBehaviour
{
	private string buttonPressedMessage = "Nothing was pressed yet";
	private InputGestureDetector swipeRightDetector;
	private InputGestureDetector swipeLeftDetector;
	private InputGestureDetector crossHandDetector;
	private InputGestureDetector pushDetector;
	private InputGestureDetector waveDetector;
	private InputGestureDetector steadyDetector;
	private InputGestureDetector leftHandupDetector;
	private int waves = 0;
	private int pushes = 0;
	private int crossHands = 0;
	private int swipeLefts = 0;
	private int swipeRights = 0;
	private int steadys = 0;
	private int leftHandups = 0;
	
	
	private Rect myRect = new Rect (0, 0, 300, 100);
	NIInput m_input; ///< @brief The input we get the axes from.
	NISkeletonTracker m_tracker;
	
	public GUISkin menuSkin;
	public Rect menuArea;
	public Rect playButton;
	public Rect instructionsButton;
	public Rect quitButton;
	Rect menuAreaNormalized;
	/// mono-behavior start - initializes the input
	public void Start ()
	{
		if(m_input==null)
        {
            m_input = FindObjectOfType(typeof(NIInput)) as NIInput;
            if (m_input == null)
                throw new System.Exception("Please add an NIInput object to the scene");
        }
		
		if(m_tracker == null) {
			m_tracker = FindObjectOfType(typeof(NISkeletonTracker)) as NISkeletonTracker;
		}
		
		swipeRightDetector = new InputGestureDetector(m_input, "NIGUI_SWIPE_RIGHT");
		swipeLeftDetector = new InputGestureDetector(m_input, "NIGUI_SWIPE_LEFT");
		crossHandDetector = new InputGestureDetector(m_input, "NIGUI_CROSS_HAND");
		pushDetector = new InputGestureDetector(m_input, "NIGUI_PUSH");
		waveDetector = new InputGestureDetector(m_input, "NIGUI_WAVE");
		steadyDetector = new InputGestureDetector(m_input, "NIGUI_CLICK");
		leftHandupDetector = new InputGestureDetector(m_input, "NIGUI_LEFT_HANDUP");
		menuAreaNormalized =
			new Rect (menuArea.x * Screen.width - (menuArea.width * 0.5f),
				menuArea.y * Screen.height - (menuArea.height * 0.5f),
				menuArea.width, menuArea.height);
	}
	
	void OnGUI1 ()
	{
		GUI.skin = menuSkin;
		
		// place the button pressed label
		myRect.x = (Screen.width / 2) - 125;
		
		myRect.y = (Screen.height / 2) - 20;
		myRect.width = 600;
		myRect.height = 60;
		GUI.Box (myRect, buttonPressedMessage);
		
		GUI.BeginGroup (menuAreaNormalized);
		
		
		if (NIGUI.Button (new Rect (playButton), "Play")) {
			buttonPressedMessage = "Play was pressed at time=" + Time.time;
		}
		if (NIGUI.Button (new Rect (instructionsButton), "Instructions")) {
			buttonPressedMessage = "Instructions was pressed at time=" + Time.time;
		}
		if (NIGUI.Button (new Rect (quitButton), "Quit")) {
			Application.Quit ();
		}
		
		GUI.EndGroup ();
		
	}

	/// mono-behavior OnGUI to show GUI elements
	public void OnGUI ()
	{
		/*if (DetectGesture("NIGUI_CROSS_HAND", ref currentCrossHand)) {
			crossHands++;	
		}
		if (DetectGesture("NIGUI_PUSH", ref currentPush)) {
			pushes++;
		}
		if(DetectGesture("NIGUI_WAVE", ref currentWave)) {
			waves++;
		}
		//DetectGesture("NIGUI_RAISE_HAND");
		if(DetectGesture("NIGUI_CLICK", ref currentSteady)) {
			steadys++;
		}*/
		if (swipeRightDetector.Detect())
		{
		 	swipeRights++;
		}
		if (swipeLeftDetector.Detect())
		{
		 	swipeLefts++;
		}
		if (crossHandDetector.Detect())
		{
		 	crossHands++;
		}
		if (pushDetector.Detect())
		{
		 	pushes++;
		}
		if (waveDetector.Detect())
		{
		 	waves++;
		}
		if (steadyDetector.Detect())
		{
		 	steadys++;
		}
		if (leftHandupDetector.Detect())
		{
		 	leftHandups++;
		}
		DrawGUI();
	}
	
	private bool DetectGesture(string gesture, ref bool currentGestureChecked)
	{
			
		float val = m_input.GetAxis (gesture);
		Debug.Log("current gesture " + gesture + " value is " + val + " and checked is " + currentGestureChecked +
			"swipeRight number is " + swipeRights);
		if (val >= 1.0f) {
			if(currentGestureChecked) {
				Debug.Log("ignore duplicated " + gesture);
				return false;
			} else {
				buttonPressedMessage = gesture + " has been detected value =" + val;
				Debug.Log("Fired:" + gesture);
				currentGestureChecked = true;
				return true;
			}
			
		}
		if (val > 0) {
			// The value is between 0 and 1 so we have detected the exit pose and are waiting
			// for it to continue long enough. 
			// Just pop up a message telling the user to continue holding the pose...
			buttonPressedMessage = "Hold the " + gesture + " pose until " + val + " reaches 1";
			currentGestureChecked = false;
		}
		currentGestureChecked = false;
		return false;
	}
	
	private bool DetectGesture(string gesture, ref float lastHappenTime)
	{
		if (lastHappenTime == 0) {
			lastHappenTime = Time.time;
		}
			
		float val = m_input.GetAxis (gesture);
		
		if (val >= 1.0f) {
			if(m_input.HasFiredSinceTime(lastHappenTime, gesture)) {
				return false;
			} else {
				buttonPressedMessage = gesture + " has been detected value =" + val;
				Debug.Log("Fired:" + gesture);
				lastHappenTime = Time.time;
				return true;
			}
			
		}
		if (val > 0) {
			// The value is between 0 and 1 so we have detected the exit pose and are waiting
			// for it to continue long enough. 
			// Just pop up a message telling the user to continue holding the pose...
			buttonPressedMessage = "Hold the " + gesture + " pose until " + val + " reaches 1";
		}
		
		return false;
	}

	private void DrawGUI()
	{
		GUI.skin = menuSkin;
		// place the first button
		myRect.x = Screen.width - 300;
		myRect.y = 100;
		myRect.width = 200;
		myRect.height = 80;
		if (NIGUI.Button (myRect, "Play")) {
			buttonPressedMessage = "Button Play was pressed at time=" + Time.time;
		}

		// place the second button
		//myRect.x = Screen.width - 200;;
		myRect.y = 300;
		if (NIGUI.Button (myRect, "Demo")) {
			buttonPressedMessage = "Button Demo was pressed at time=" + Time.time;
		}
		
		//myRect.x = Screen.width - 200;;
		myRect.y = 500;
		if (NIGUI.Button (myRect, "Quit")) {
			buttonPressedMessage = "Button Quit was pressed at time=" + Time.time;
			Application.Quit ();
		}

		// place the button pressed label
		myRect.x = Screen.width / 2 - 300;
		myRect.y = 20;
		myRect.width = 800;
		myRect.height = 50;
		GUI.Box (myRect, buttonPressedMessage);
		
		myRect.x = 100;
		myRect.y = 100;
		myRect.width = 240;
		myRect.height = 40;
		GUI.Box(myRect, "CrossHand:" + crossHands);
		
		myRect.x += 280;
		GUI.Box(myRect, "Wave:" + waves);
		
		myRect.x += 280;
		GUI.Box(myRect, "Push:" + pushes);
		
		myRect.x += 280;
		GUI.Box(myRect, "Steady:" + steadys);
		
		myRect.x = 100;
		myRect.y = 240;
		GUI.Box(myRect, "SwipeLeft:" + swipeLefts);
		
		myRect.x += 280;
		myRect.y = 240;
		GUI.Box(myRect, "SwipeRight:" + swipeRights);
		
		myRect.x += 280;
		myRect.y = 240;
		GUI.Box(myRect, "HandupL:" + leftHandups);
		
		NISelectedPlayer player = m_tracker.GetTrackedPlayer();
		SkeletonJointPosition rightHand;
        if(player.GetSkeletonJointPosition(SkeletonJoint.RightHand,out rightHand)==false || rightHand.Confidence<=0.5f)
            return ;
        
        Vector3 pos = NIConvertCoordinates.ConvertPos(rightHand.Position);
		myRect.x = 400;
		myRect.y = 300;
		GUI.Box(myRect, "" +  pos.x);
		
		myRect.x = 400;
		myRect.y = 400;
		GUI.Box(myRect,  "" + pos.y);
		// Click axis value label
		/*
		myRect.x = 50;
		myRect.y = (Screen.height / 2) + 20;
		myRect.width = 250;
		myRect.height = 30;
		GUI.Box (myRect, "value=" + m_input.GetAxis ("NIGUI_CLICK"));
        */
	}
}
