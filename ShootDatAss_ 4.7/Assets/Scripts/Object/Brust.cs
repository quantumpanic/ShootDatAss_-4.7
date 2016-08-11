using UnityEngine;
using System.Collections;

public class Brust : MonoBehaviour {

	public SpriteRenderer spriteRenderer;
	public float alpha = 1;

	void Start(){
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
	}

	void Update () {
		alpha = Mathf.Lerp (alpha, 0, 10 * Time.deltaTime);
		spriteRenderer.color = new Color (1,1,1,alpha);

		if(alpha < 0.09f) Destroy(gameObject);
	}

    public Vector2 GetShootPoint(string faceDirection)
    {
        Vector2 shootPoint = new Vector2(0, 0);

        if (faceDirection == "right" || faceDirection == "rightIdle")
        {
            shootPoint = new Vector2(1.12f, 0.86f);
        }
        else if (faceDirection == "upRight" || faceDirection == "upRightIdle")
        {
            shootPoint = new Vector2(1.12f, 1.41f);
        }
        else if (faceDirection == "up" || faceDirection == "upIdle")
        {
            shootPoint = new Vector2(0.05f, 1.5f);
        }
        else if (faceDirection == "upLeft" || faceDirection == "upLeftIdle")
        {
            shootPoint = new Vector2(-1.12f, 1.41f);
        }
        else if (faceDirection == "left" || faceDirection == "leftIdle")
        {
            shootPoint = new Vector2(-1.12f, 0.86f);
        }
        else if (faceDirection == "downLeft" || faceDirection == "downLeftIdle")
        {
            shootPoint = new Vector2(-0.8f, 0.25f);
        }
        else if (faceDirection == "down" || faceDirection == "downIdle")
        {
            shootPoint = new Vector2(-0.25f, 0.3f);
        }
        else if (faceDirection == "downRight" || faceDirection == "downRightIdle")
        {
            shootPoint = new Vector2(0.8f, 0.25f);
        }

        return shootPoint;
    }
}
