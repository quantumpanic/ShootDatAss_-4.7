using UnityEngine;
using System.Collections;
using System;

// This class receive the updated transform from server for the remote player model
public class NetworkTransformReceiver : MonoBehaviour {
	Transform thisTransform;
	
	private NetworkTransformInterpolation interpolator;
	private AnimationSynchronizer animator;
	
	void Awake() {
		thisTransform = this.transform;
		animator = GetComponent<AnimationSynchronizer>();
		interpolator = GetComponent<NetworkTransformInterpolation>();
		if (interpolator!=null) {
			interpolator.StartReceiving();
		}
	}

    public void ReceiveTransform(NetworkTransform ntransform)
    {
        ntrans = ntransform;
		if (interpolator!=null) {
			// interpolating received transform
			interpolator.ReceivedTransform(ntransform);
		}
		else {
			//No interpolation - updating transform directly
			thisTransform.position = ntransform.Position;
			// Ignoring x and z rotation angles
			thisTransform.localEulerAngles = ntransform.AngleRotationFPS;
		}
    }

    NetworkTransform ntrans;

    void OnDrawGizmos()
    {
        if (ntrans == null) return;
        Gizmos.color = new Color(0,1,0,.5f);
        Gizmos.DrawSphere(ntrans.Position, .5f);
    }
		
}
