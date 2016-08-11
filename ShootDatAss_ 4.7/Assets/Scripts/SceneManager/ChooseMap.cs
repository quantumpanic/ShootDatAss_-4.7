using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChooseMap : MonoBehaviour {

	public int mapChoosen;
	public bool[] mapChoose;
	public RectTransform[] rectTransform;
	public Sprite[] buttonImages;
	public SpriteRenderer mapPreview;
	public Sprite[] mapImages;


	public void Choose(int number){
		mapChoosen = number;
		for(int i=0; i< mapChoose.Length; i++){
			if(i == number) {
				mapChoose[i] = true;
				rectTransform[i].gameObject.GetComponent<Image>().sprite = buttonImages[1];
			}else {
				mapChoose[i] = false;
				rectTransform[i].gameObject.GetComponent<Image>().sprite = buttonImages[0];
			}
		}

		mapPreview.sprite = mapImages [number];
	}

	public void FixedUpdate(){
		for(int i=0; i< rectTransform.Length; i++){
			if(mapChoose[i]){
				rectTransform[i].sizeDelta = Vector2.Lerp(rectTransform[i].sizeDelta, new Vector2(180, 26), 10 * Time.deltaTime);
			}else{
				rectTransform[i].sizeDelta = Vector2.Lerp(rectTransform[i].sizeDelta, new Vector2(150, 26), 10 * Time.deltaTime);
			}
		}
	}
}
