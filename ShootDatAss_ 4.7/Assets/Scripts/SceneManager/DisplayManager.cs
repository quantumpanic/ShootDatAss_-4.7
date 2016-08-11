using UnityEngine;
using System.Collections;

public class DisplayManager : MonoBehaviour {

	public GameObject rightPanel;
	public GameObject leftPanel;
	public float calibrate;
	private Camera camera;

	void Awake(){
		camera = GameObject.Find ("Main Camera").GetComponent<Camera> ();
	}

	void Start(){
		Vector3 left = camera.ScreenToWorldPoint (new Vector3(0,Screen.height/2,0));
		Vector3 right = camera.ScreenToWorldPoint (new Vector3(Screen.width,Screen.height/2,0));

		leftPanel.transform.position = new Vector3 (left.x, left.y + calibrate, 0);
		rightPanel.transform.position = new Vector3 (right.x, right.y + calibrate, 0);

		string resolution = ((float)Screen.width / (float)Screen.height).ToString ("F2");

		//print(resolution);

		if (resolution == "1.67" || resolution == "1.67") {
			leftPanel.transform.localScale = new Vector3(1,1,1);
			rightPanel.transform.localScale = new Vector3(1,1,1);
		}else if (resolution == "1.50") {
			leftPanel.transform.localScale = new Vector3(0.9f,0.9f,0.9f);
			rightPanel.transform.localScale = new Vector3(0.9f,0.9f,0.9f);			
		}else if (resolution == "1.78") {
			leftPanel.transform.localScale = new Vector3(1,1,1);
			rightPanel.transform.localScale = new Vector3(1,1,1);		
		}else if (resolution == "1.71") {
			leftPanel.transform.localScale = new Vector3(1,1,1);
			rightPanel.transform.localScale = new Vector3(1,1,1);		
		}else if (resolution == "1.60") {
			leftPanel.transform.localScale = new Vector3(0.95f,0.95f,0.95f);
			rightPanel.transform.localScale = new Vector3(0.95f,0.95f,0.95f);			
		}


	}
}
