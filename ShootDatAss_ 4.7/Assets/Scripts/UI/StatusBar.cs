using UnityEngine;
using System.Collections;

public class StatusBar : MonoBehaviour {

	public GameObject statusHide;
	public GameObject statusShow;
	public bool isShow;
	public Vector3[] target;

	void Start(){
		target [0] = new Vector3 (transform.localPosition.x, transform.localPosition.y, 0);
		target [1] = new Vector3 (transform.localPosition.x + 4.75f, transform.localPosition.y, 0);
	}

	void FixedUpdate () {
		if (!isShow) {
			if(transform.localPosition.x < target[0].x + 0.02){
				statusHide.SetActive(true);
				statusShow.SetActive(false);
			}
			transform.localPosition = Vector3.Lerp(transform.localPosition, target[0], 5 * Time.deltaTime);
		}else{
			statusHide.SetActive(false);
			statusShow.SetActive(true);
			transform.localPosition = Vector3.Lerp(transform.localPosition, target[1], 5 * Time.deltaTime);
		}
	}

	void OnMouseDown(){
		isShow = !isShow;
	}
}
