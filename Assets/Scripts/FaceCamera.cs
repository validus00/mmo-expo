using UnityEngine;

/*
 * FaceCamera class is to make text objects always face the camera
 */
public class FaceCamera : MonoBehaviour {
    // target will be the camera
    private Transform __target;

    // Update is called once per frame
    void Update() {
        // set camera here since this script is loaded before camera object
        if (__target == null) {
            __target = GameObject.Find("Camera").transform;
        }
        transform.LookAt(__target);
        transform.Rotate(Vector3.up - new Vector3(0, 180, 0));
    }
}
