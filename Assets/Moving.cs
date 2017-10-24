using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour
{
  Vector3 startPos { get; set; }
  Vector3 endPos { get; set; }

  Transform target;

  public float anim;

  // Use this for initialization
  void Start ()
  {
	}
	
	// Update is called once per frame
	void Update ()
  {    
  }

  public void SetTarget(Transform t)
  {
    target = t;

    startPos = gameObject.transform.position;//.TransformPoint(gameObject.transform.position);
    endPos = target.position;// TransformPoint(target.position);
    gameObject.GetComponent<Animator>().SetTrigger("IsMoved");
    
    //Debug.Log(string.Format("endpos = {0} , startpos = {1}", endPos, startPos));
    StartCoroutine(MoveToTarget());
  }

  IEnumerator MoveToTarget()
  {
    //gameObject.transform.SetParent(GameObject.Find("Canvas").transform);
    bool atTheEnd = false;
    int count = 0;
    while (!atTheEnd)
    {
      count++;
      //Debug.Log(string.Format("count for go {0} = {1}, anim = {2}", gameObject.GetInstanceID(), count, anim));
      var newPosition = startPos + anim * (endPos - startPos);
      var step = newPosition - gameObject.transform.position;
      var toTarget = endPos - gameObject.transform.position;
      //Debug.Log(string.Format("step  = {0} ::: target = {1}", step, toTarget));
      atTheEnd = step.magnitude >= toTarget.magnitude;

      //gameObject.transform.Translate(step);
      gameObject.transform.position = newPosition;
      yield return new WaitForFixedUpdate();
    }
    Debug.Log("while ended! count " + count);
    gameObject.transform.position = endPos;
    gameObject.transform.SetParent(target);
  }
}
