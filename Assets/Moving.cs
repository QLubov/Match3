using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour
{
  public GameManager gameMgr;
  static GameObject clickedGameObject = null;
  public void OnClick()
  {
    //add somthing about focus
    if (clickedGameObject == null)
    {
      clickedGameObject = gameObject;
    }
    else
    {
      gameMgr.MoveElements(gameObject, clickedGameObject);
      clickedGameObject = null;
    }
  }
}
