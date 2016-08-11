using UnityEngine;
using System.Collections;

public class ListenerObject : MonoBehaviour {
	MatchManager matchmanager;
	Camera mainCam;

	void Awake()
	{
		mainCam = GameObject.Find ("Main Camera").GetComponent<Camera>();
	}

	void CombatCam()
	{
		float posX = mainCam.transform.position.x;
		float posY = 0f;
		float posZ = mainCam.transform.position.y * MatchManager.instance.map2scrRatio;
		transform.position = new Vector3 (posX, posY, posZ);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (MatchManager.instance != null)
			matchmanager = MatchManager.instance;
		if (matchmanager != null) CombatCam ();
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		//Gizmos.DrawSphere (transform.position, 1);
	}
}
