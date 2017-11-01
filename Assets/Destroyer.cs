using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {

	// Update is called once per frame
	void Update ()
  {
    if (transform.childCount > 0)
    {
      for (int i = 0; i < transform.childCount; i++)
      {
        Destroy(transform.GetChild(i).gameObject, 1);
      }
    }
		
	}
}
