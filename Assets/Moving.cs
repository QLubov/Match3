using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour
{
  Vector3 startPos { get; set; }
  Vector3 endPos { get; set; }
  Transform target;

  public float anim;

  public void SetTarget(Transform t, bool moveBack = false)
  {
    target = t;

    startPos = gameObject.transform.position;
    endPos = target.position;

    //Debug.Log(string.Format("endpos = {0} , startpos = {1}", endPos, startPos));
    if (moveBack)
      StartCoroutine(MoveAndBack(gameObject.transform.parent));
    else
      StartCoroutine(MoveToTarget());
  }

  IEnumerator MoveAndBack(Transform oldParent)
  {
    yield return StartCoroutine(MoveToTarget(2.0f));
    target = oldParent;
    startPos = gameObject.transform.position;
    endPos = target.position;
    yield return StartCoroutine(MoveToTarget(2.0f));
  }

  IEnumerator MoveToTarget(float multiplier = 1.0f)
  {
    //gameObject.transform.SetParent(GameObject.Find("Canvas").transform);
    gameObject.transform.SetParent(target);
    bool atTheEnd = false;
    int count = 0;
    while (!atTheEnd)
    {
      count++;
      //Debug.Log(string.Format("count for go {0} = {1}, anim = {2}", gameObject.GetInstanceID(), count, anim));
      var newPosition = startPos + anim * multiplier * (endPos - startPos);
      var step = newPosition - gameObject.transform.position;
      var toTarget = endPos - gameObject.transform.position;
      //Debug.Log(string.Format("step  = {0} ::: target = {1}", step, toTarget));
      atTheEnd = step.magnitude >= toTarget.magnitude;

      //gameObject.transform.Translate(step);
      gameObject.transform.position = newPosition;
      yield return new WaitForFixedUpdate();
    }
    //Debug.Log("while ended! count " + count);
    gameObject.transform.position = endPos;
    gameObject.transform.SetParent(target);
  }

  public void RemoveMe()
  {
    transform.SetParent(null);
    Destroy(gameObject);
  }
}
