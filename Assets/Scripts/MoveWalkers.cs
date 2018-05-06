using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWalkers : MonoBehaviour {

    float speed;

    private void Start()
	{
        speed = 4 + 3 * Random.value;
	}

	// Update is called once per frame
	void Update () {
        Vector3 destination = new Vector3(1, 2.8f);
        this.transform.position = Vector3.MoveTowards(this.transform.position, destination, speed * Time.deltaTime);

        // destroy units that reach their destination
        if (this.transform.position == destination) {
            Destroy(gameObject);
        }
	}
}
