using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ImageFeed : MonoBehaviour {
	private readonly List<Texture2D> images = new List<Texture2D>();
	
	public List<Texture2D> Images
    {
        get { return images; }
    }
	
	private List<string> imageNames = new List<string>();
	
	public List<string> ImageNames
    {
        get { return imageNames; }
    }
	
	public bool Loaded
	{
		get 
		{
			return (ImageNames != null && Images != null && Images.Count == ImageNames.Count);	
		}
	}
    public string Path = "Slides";
    public string SearchPattern = "*.PNG;*.JPG;*.jpeg";
	
	/*
    void Awake()
    {
        if (null == menu) {
            menu = GetComponent<ScrollingMenu>();
        }
        if (null == menu) {
            Debug.LogError("No menu attached to " + gameObject.name);
        }
    }
	*/
	// Use this for initialization
	void Start () {
        /*
		foreach (string filename in System.IO.Directory.GetFiles(Application.dataPath + "/" + Path, SearchPattern)) {
            ImageItem newItem = Instantiate(Item) as ImageItem;
            newItem.Init(filename);
            menu.Add(newItem.transform);
        }*/
		imageNames = GetFiles(Application.dataPath + "/" + Path, SearchPattern);
		
		foreach (string filename in imageNames) {
			//ImageLoader loader = new ImageLoader(images);
			//loader.Init(filename);
			StartCoroutine(LoadImage(filename));
		}
	}
	
	IEnumerator LoadImage(string imagePath)
    {
        Debug.Log("Image path is " + imagePath + ";full path is " + "file://" + System.IO.Path.GetFullPath(imagePath).Replace('\\','/'));
        WWW req = new WWW("file://" + System.IO.Path.GetFullPath(imagePath).Replace('\\','/'));
        yield return req;
        //transform.Find("Image").localScale = new Vector3((float)req.texture.width / (float)req.texture.height, 1.0f, 1.0f);
        //transform.Find("Image").renderer.material.mainTexture = req.texture;
		Texture2D image = new Texture2D(64, 64);
		req.LoadImageIntoTexture(image);
		Debug.Log("loaded one image");
		images.Add(image);
    }
	
	private static List<string> GetFiles(
                                string path,
                                string searchPattern)
	{
	    Debug.Log("Image Path is " + path + "search pattern is " + searchPattern);
		string[] exts = searchPattern.Split(';');
	
	    List<string> strFiles = new List<string>();
	    foreach(string filter in exts)
	    {
	        Debug.Log("search by pattern " + filter);
			strFiles.AddRange(
	               System.IO.Directory.GetFiles(path, filter));
	    }
	    //return strFiles.ToArray();
		Debug.Log("file size is " + strFiles.Count);
		return strFiles;
	}

}
