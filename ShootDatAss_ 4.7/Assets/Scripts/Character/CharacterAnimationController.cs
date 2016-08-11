using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CharacterAnimationController : MonoBehaviour {

	public string playerName;
	public int costume = 0;
	public string faceDirection = "down";
	public string gun = "DefaultGun";
	public GameObject[] characterFace;

	public Vector2 shootPosition;

	private string faceDirectionTEMP;
	private string gunTEMP;
	private bool idle;
	private float hurt = 1;
	public bool preview;

    void LateUpdate()
    {
        if (preview) SetFaceDirection(new KeyValuePair<float, bool>(270, true));
        ChangeCostume();
        ChangeGun();
    }

    void ChangeCostume()
    {
        var subSprite = Resources.LoadAll<Sprite>("Costume/" + "costume_" + costume);
        foreach (GameObject go in characterFace)
        {
            foreach (var renderer in go.GetComponentsInChildren<SpriteRenderer>())
            {
                string spriteName = renderer.sprite.name;
                spriteName = spriteName.Replace("costume_", "");
                string[] spriteNameSplit = spriteName.Split(new char[] { '_' });

                var newSprite = Array.Find(subSprite, item => item.name == "costume_" + costume + "_" + spriteNameSplit[1]);
                if (newSprite) renderer.sprite = newSprite;

                renderer.color = new Color(1, hurt, hurt, 1);
            }
        }
    }

	public void SetFaceDirection(KeyValuePair<float,bool> dict){
        float angle = dict.Key;
        idle = dict.Value;

        CharacterController chr = transform.parent.GetComponent<CharacterController>();
        if (chr != null)
        {
            GameObject pivot = chr.shotPivot;
            pivot.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        }

        string direction = "";

        if (angle < 22.5f || angle > 337.5)
        {
            direction = "right";
        }
        else if (angle > 22.5f && angle < 67.5f)
        {
            direction = "upRight";
        }
        else if (angle > 67.5 && angle < 112.5f)
        {
            direction = "up";
        }
        else if (angle > 112.5f && angle < 157.5f)
        {
            direction = "upLeft";
        }
        else if (angle > 157.5f && angle < 202.5f)
        {
            direction = "left";
        }
        else if (angle > 202.5f && angle < 247.5f)
        {
            direction = "downLeft";
        }
        else if (angle > 247.5f && angle < 292.5f)
        {
            direction = "down";
        }
        else if (angle > 292.5f && angle < 337.5f)
        {
            direction = "downRight";
        }

        if (idle) direction += "Idle";
        faceDirection = direction;

		FaceCharacter (direction);
	}

    public void FaceCharacter(string face)
    {

        if (faceDirection.Contains("Idle"))
        {
            idle = true;
            face = faceDirection.Replace("Idle", "");
        }
        else
        {
            idle = false;
        }

        if (face == "right")
        {
            CharacterFace("Right", false);
        }
        else if (face == "upRight")
        {
            CharacterFace("UpRight", false);
        }
        else if (face == "up")
        {
            CharacterFace("Up", false);
        }
        else if (face == "upLeft")
        {
            CharacterFace("UpRight", true);
        }
        else if (face == "left")
        {
            CharacterFace("Right", true);
        }
        else if (face == "leftDown")
        {
            CharacterFace("RightDown", true);
        }
        else if (face == "down")
        {
            CharacterFace("Down", false);
        }
        else if (face == "downLeft")
        {
            CharacterFace("DownRight", true);
        }
        else if (face == "downRight")
        {
            CharacterFace("DownRight", false);
        }
        else
        {
            CharacterFace("Down", false);
        }

        faceDirection = face;

    }

	void CharacterFace(string face, bool flip){
		foreach(GameObject go in characterFace){
			if(go.name == face){

				go.SetActive(true);

				if(flip){
					go.transform.localScale = new Vector3(-1,1,1);
				}else{
					go.transform.localScale = new Vector3(1,1,1);
				}

				foreach(Transform g in go.transform.FindChild("Body")){
					if(g.name == gun){
						g.gameObject.SetActive(true);
					}else{
						g.gameObject.SetActive(false);
					}
				}
				go.transform.FindChild("Body").GetComponent<Animator>().SetBool("idle",idle);
				go.transform.FindChild("Foot").GetComponent<Animator>().SetBool("idle",idle);
			
			}else{

				go.SetActive(false);
			
			}
		}
	}

	void ChangeGun(){
		foreach(GameObject go in characterFace){
			if(go.activeSelf == true){
				foreach(Transform g in go.transform.FindChild("Body")){
					if(g.name == gun){
						g.gameObject.SetActive(true);
					}else{
						g.gameObject.SetActive(false);
					}
				}
			}
		}
	}

	public void SetCostume(int numberCostume){
		costume = numberCostume;
	}

	public void SetSortingLayer(float sortingNumber){
		int sortingOrder = (int)(-sortingNumber * 10);
		foreach(GameObject go in characterFace){
			foreach(var renderer in go.GetComponentsInChildren<SpriteRenderer>()){
				if(go.name == "Up"){
					if(renderer.gameObject.name == "Body") renderer.sortingOrder = sortingOrder;
					else renderer.sortingOrder = sortingOrder - 1;
				}else{
					if(renderer.gameObject.name == "Body") renderer.sortingOrder = sortingOrder;
					else if(renderer.gameObject.name == "Foot") renderer.sortingOrder = sortingOrder - 1;
					else renderer.sortingOrder = sortingOrder + 1;
				}
			}
		}
	}

	public Vector2 GetShootPoint(){
		Vector2 shootPoint = new Vector2 (0,0);

		if(faceDirection == "right" || faceDirection == "rightIdle" ){
			shootPoint = 	new Vector2(1.12f, 0.86f);
		}else if(faceDirection == "upRight" || faceDirection == "upRightIdle" ){
			shootPoint = new Vector2(1.12f, 1.41f);
		}else if(faceDirection == "up" || faceDirection == "upIdle" ){
			shootPoint = new Vector2(0.05f, 1.5f);
		}else if(faceDirection == "upLeft" || faceDirection == "upLeftIdle" ){
			shootPoint = new Vector2(-1.12f, 1.41f);
		}else if(faceDirection == "left" || faceDirection == "leftIdle" ){
			shootPoint = new Vector2(-1.12f, 0.86f);
		}else if(faceDirection == "downLeft" || faceDirection == "downLeftIdle" ){
			shootPoint = new Vector2(-0.8f, 0.25f);
		}else if(faceDirection == "down" || faceDirection == "downIdle" ){
			shootPoint = new Vector2(-0.25f, 0.3f);
		}else if(faceDirection == "downRight" || faceDirection == "downRightIdle" ){
			shootPoint = new Vector2(0.8f, 0.25f);
		}

		return shootPoint;
	}

	void Update(){
		hurt = Mathf.Lerp(hurt, 1, 3f * Time.deltaTime);
	}

	public void GetHit(){
		hurt = 0;
	}
}
