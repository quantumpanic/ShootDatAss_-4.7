using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RenameCrate : MonoBehaviour {

	void Update () {
		GameObject[] crates = GameObject.FindGameObjectsWithTag ("Crate");
		for(int i=0; i< crates.Length; i++){
			crates[i].name = i.ToString();
		}
	}
}
