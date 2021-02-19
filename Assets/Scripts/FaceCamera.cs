using UnityEngine;

/*
 * FaceCamera class is to make text objects always face the camera
 */
public class FaceCamera : MonoBehaviour
{
    // target will be the camera
    private GameObject __target;

    // Update is called once per frame
    void Update()
    {
        // set camera here since this script is loaded before camera object
        if (__target == null)
        {
            __target = GameObject.Find(GameConstants.k_Camera);
        }
        if (__target != null)
        {
            transform.LookAt(__target.transform);
            transform.Rotate(Vector3.up - new Vector3(0, 180, 0));
        }
    }
}
