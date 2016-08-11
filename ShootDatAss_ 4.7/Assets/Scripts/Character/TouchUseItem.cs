using UnityEngine;
using System.Collections;

public class TouchUseItem : MonoBehaviour {

	public int slotNumber;
	public TouchController touchController;
	float timer;
	public float tapTime = 0.17f;

    void OnMouseUp()
    {
		touchController.UseItem (slotNumber);
		timer = 0;
	}

	void OnMouseDrag()
	{
		timer += Time.deltaTime;
		if(timer >= tapTime) touchController.ChargeItem (slotNumber);
	}

	public void Initialize(int number, TouchController tc){
		slotNumber = number;
		touchController = tc;
	}
}

public class SniperTarget : TouchUseItem
{
    public Sniper parentSniper;

    void OnMouseUp()
    {
        parentSniper.SnipePlayer(transform.parent.gameObject.GetComponent<CharacterController>().playerName);
    }
}
