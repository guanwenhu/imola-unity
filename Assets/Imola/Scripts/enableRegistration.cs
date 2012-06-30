using UnityEngine;
using System.Collections;
using OpenNI;

public class enableRegistration : MonoBehaviour {

	// Use this for initialization
	void Start () {
		DepthGenerator depth = NIContext.Instance.CreateNode(NodeType.Depth) as DepthGenerator;
		ImageGenerator image = NIContext.Instance.CreateNode(NodeType.Image) as ImageGenerator;
		depth.AlternativeViewpointCapability.SetViewpoint(image);
	}
}
