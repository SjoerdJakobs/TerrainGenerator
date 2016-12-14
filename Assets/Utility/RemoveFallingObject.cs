using UnityEngine;
using System.Collections;

public class RemoveFallingObject : MonoBehaviour {
    Rigidbody rigid;
    private int timer;
	// Use this for initialization
	void Start () {
        timer = 0;
        StartCoroutine(SlowUpdate());
	}

    // Update is called once per frame
    IEnumerator SlowUpdate()
    {
        while (true)
        {
            timer++;
            if (transform.position.y < -30)
            {
                GameObject.Destroy(gameObject);
            }
            if(timer >= 100)
            {
                GameObject.Destroy(GetComponent<Rigidbody>());
                GameObject.Destroy(GetComponent<RemoveFallingObject>());
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
