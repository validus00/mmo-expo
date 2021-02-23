using UnityEngine;

/*
 * FaceCamera class is to make text objects always face the camera
 */
public class FaceCamera : MonoBehaviour
{
    // target will be the camera
    private GameObject _target;

    // Update is called once per frame
    void Update()
    {
        // set camera here since this script is loaded before camera object
        if (_target == null)
        {
            _target = GameObject.Find(GameConstants.K_Camera);
        }
        if (_target != null)
        {
            transform.LookAt(_target.transform);
            transform.Rotate(Vector3.up - new Vector3(0, 180, 0));
        }
    }
}
