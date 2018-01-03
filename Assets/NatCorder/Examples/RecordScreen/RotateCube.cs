using UnityEngine;

public class RotateCube : MonoBehaviour {
	
	public bool introduceLag;

	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.up * 90f * Time.deltaTime, Space.World);
		if (introduceLag) for (int i = 0, a = 0; i < 6000000; i++) a += i;
	}
}