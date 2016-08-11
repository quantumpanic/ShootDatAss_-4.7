using UnityEngine;
using System.Collections;

public class GameEntity : MonoBehaviour {
    public BoxCollider canvas;
    public BoxCollider map;
    public Vector3 map2canvas;
    public Vector3 canvas2map;
    public float heightAdder;
	
	private bool onTheGround;

    public GameObject canvasObj;
    public GameObject shadow;
	float shadowSize;

	// Use this for initialization
	public void Start () {
        map = GameObject.Find("Ground").GetComponent<BoxCollider>();
        canvas = GameObject.Find("Map").GetComponent<BoxCollider>();
        map2canvas = new Vector3(map.size.x / canvas.size.x, map.size.z / canvas.size.y);
        canvas2map = new Vector3(canvas.size.x / map.size.x, canvas.size.y / map.size.z);
	}
	
	// Update is called once per frame
    public void Update()
    {
		DrawOnCanvas ();
		//KeepCanvasUpright ();
	}

    public void KeepCanvasUpright()
	{
		canvasObj.transform.rotation = Quaternion.Euler(Vector3.zero);
	}

	public void DrawOnCanvas()
	{
        float scaleToScr = canvas2map.y;
		float addHeight = heightAdder;
        canvasObj.transform.position = new Vector3(transform.position.x, (transform.position.z + addHeight) * scaleToScr, 0);
		if (shadow) DrawShadow();
	}

    public void DrawShadow()
	{
		float scaleToScr = map2canvas.y;
		shadow.transform.position = new Vector3 (transform.position.x, (transform.position.z* scaleToScr) - 0.17f, 0);
		shadow.GetComponent<SpriteRenderer>().sortingOrder = (int)(-transform.position.z * 10) - 2;
		float scale = shadowSize - Mathf.Abs(transform.position.y);
		if (scale < 0) scale = 0;
		if(scale > 0) shadow.transform.localScale = new Vector3 (scale, scale, scale);
	}
	
	public virtual void GotCollide (Collision collision)
	{
		// to override
	}

    public void OnDestroy()
    {
        Destroy(canvasObj);
    }
}