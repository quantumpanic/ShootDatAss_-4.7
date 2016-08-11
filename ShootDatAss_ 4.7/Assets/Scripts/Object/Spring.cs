using UnityEngine;
using System.Collections;

public class Spring : MonoBehaviour {

	private bool isSppring;

	void OnTriggerEnter(Collider other){
		if(other.tag == "Player"){

			isSppring = !isSppring;
			transform.FindChild("Spring").FindChild("Spring").GetComponent<Animator>().SetBool("jump", isSppring);
			float velocityX = other.GetComponent<Rigidbody>().velocity.x;
			float velocityZ = other.GetComponent<Rigidbody>().velocity.z;
            other.GetComponent<CharacterController>().onTheGround = false;
			other.GetComponent<Rigidbody>().velocity = new Vector3(velocityX , 6, velocityZ);
            SoundManager.instance.PlaySfx("sfx_mJump1");
		}
	}
}
