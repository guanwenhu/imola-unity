using UnityEngine;
using System.Collections;
using OpenNI;
using ImolaNI;

public class SlidingView : MonoBehaviour{
	//public Texture2D img;
	//public Texture2D prevButton;
	//public Texture2D nextButton;
	public ImageFeed imageFeed;
	private static int singleImageWidth = Screen.width;
	private static int singleImageHeight = Screen.height;
	private GUIStyle blankStyle = new GUIStyle(); //an "empty" style to avoid any of Unity's default padding, margin and background defaults
	private Rect container = new Rect(0,0,singleImageWidth,singleImageHeight);
	private Rect content;
	private float target = 0;
	private float currentSelection;
	private bool loaded = false;
	
	private InputGestureDetector swipeRightDetector;
	private InputGestureDetector swipeLeftDetector;
	
	private NIInput m_input;
	
	private Rect myRect = new Rect (0, 0, 300, 100);
	public GUISkin menuSkin;
	
	public void Start ()
	{
		if(m_input==null)
        {
            m_input = FindObjectOfType(typeof(NIInput)) as NIInput;
            if (m_input == null)
                throw new System.Exception("Please add an NIInput object to the scene");
        }
		
		swipeRightDetector = new InputGestureDetector(m_input, "NIGUI_SWIPE_RIGHT");
		swipeLeftDetector = new InputGestureDetector(m_input, "NIGUI_SWIPE_LEFT");
		
		GUI.skin = menuSkin;
		
		singleImageWidth = Screen.width;
		singleImageHeight = Screen.height;
		container = new Rect(0,0,singleImageWidth,singleImageHeight);
	}

	void OnGUI () {
		if (imageFeed == null || !imageFeed.Loaded) {
			return;
		}
		if (!loaded) {
			content = new Rect(0, 0, singleImageWidth * imageFeed.Images.Count, singleImageHeight);
			loaded = true;
		}
		DrawImages ();
		
		SlideImage ();
		
		DrawButtons ();
	}
	
	void DrawButtons () 
	{
		// place the first button
		myRect.x = Screen.width - 300;
		myRect.y = Screen.height/2 - 40;
		myRect.width = 200;
		myRect.height = 80;
		if (NIGUI.Button (myRect, "Quit")) {
			Application.Quit ();
		}
	}
	
	void DrawImages ()
	{
		//scroll panel:
		GUI.BeginGroup(container);
		GUI.BeginGroup(content);
		
		for (int i = 0; i < imageFeed.Images.Count; i++) {
			GUI.DrawTexture(new Rect(i*singleImageWidth,0,singleImageWidth,singleImageHeight), imageFeed.Images[i], ScaleMode.ScaleAndCrop, true, 0f);
			//GUI.Label(new Rect(i*250,0,250,211),imageFeed.Images[i],blankStyle);
		}
		//GUI.Label(content,img,blankStyle);
		GUI.EndGroup();
		GUI.EndGroup();
	}
	
	void SlideImage ()
	{
		if (swipeRightDetector.Detect())
		{
		 	if (target > -content.width+container.width) {
				target-=container.width;
			} else {
				target = 0;
			}
			
			print("target: " + target);
			EstablishSlide();
		}
		if (swipeLeftDetector.Detect())
		{
		 	if (target < 0) {
				target+=container.width;
			} else {
				target = - container.width * (imageFeed.Images.Count - 1);
			}
			
			print("target: " + target);
			EstablishSlide();
		}
		
		//select button:
		if(GUI.Button(new Rect(0,0,singleImageWidth,singleImageHeight),"",blankStyle)){
			Selected();
		}
	}
	
		
	void EstablishSlide(){
		currentSelection=Mathf.Abs(target)/container.width + 1;
		iTween.Stop(gameObject,"value");
		iTween.ValueTo(gameObject,iTween.Hash("time",.8,"from",content.x,"to",target,"easetype",iTween.EaseType.easeInOutExpo,"onupdate","ApplySlide"));
	}
	
	void ApplySlide(float position){
		content.x=position;
	}
	
	void Selected(){
		print("Item: " + currentSelection + " was selected!");
	}
}