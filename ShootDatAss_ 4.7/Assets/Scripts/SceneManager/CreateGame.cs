using UnityEngine;
using System.Collections;

public class CreateGame : MonoBehaviour {

	public int selectedMap;
	public GameObject selector;
	public GameObject[] map;

	public void SelectMap(int mapNumber){
		selectedMap = mapNumber;
		selector.SetActive (true);
		selector.transform.parent = map [mapNumber].transform;
		selector.transform.localPosition = new Vector3 (0,0,0);
	}

}
