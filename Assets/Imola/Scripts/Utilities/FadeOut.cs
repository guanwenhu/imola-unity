using UnityEngine;
using System.Collections;

public class FadeOut : MonoBehaviour {
	
	public float fadeoutSpeed;
	public float stayTime; // in second
	
	// Use this for initialization
	void Start () {
		m_startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time - m_startTime > stayTime)
		{
			float deltaSize = Time.deltaTime * fadeoutSpeed;
		
			transform.localScale = new Vector3(transform.localScale.x - deltaSize, transform.localScale.y - deltaSize, transform.localScale.z - deltaSize);
		
			if(transform.localScale.x < 0.0)
				Destroy(gameObject);
		}
	}
	
	private float m_startTime;
}
