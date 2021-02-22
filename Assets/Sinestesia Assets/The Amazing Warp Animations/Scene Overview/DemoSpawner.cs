using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSWarpAnimations
{
    public class DemoSpawner : MonoBehaviour {
    	public GameObject prefab_to_spawn;
        public float loop_time = 15f;
    	private GameObject warp;

    	// Use this for initialization
    	void Start () {
    		StartCoroutine("WaitAndSpawn");
    	}
    	

    //warp = Instantiate(prefab_to_spawn, transform.position, transform.rotation);
    	private IEnumerator WaitAndSpawn()
        {
        	float aux_random_time = Random.Range( 0.0f, 1.5f );
        	yield return new WaitForSeconds(aux_random_time);

            while (true)
            {
            	warp = Instantiate(prefab_to_spawn, transform.position, transform.rotation);
                yield return new WaitForSeconds( loop_time );
                Destroy(warp);            
            }
        }

    }
}