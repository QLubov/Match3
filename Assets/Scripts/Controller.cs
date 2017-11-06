using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Controller : MonoBehaviour
{
  static GameObject clicked;
  public GameManager gameMgr;
  public static bool IsBombUsed = false;
  public static bool IsRecolorUsed = false;

  //move Controller to Panel from Item 
  public void OnClick()
  {
    //TODO: do not forget to set timer is On Action for these two cases
    if (IsBombUsed == true)
    {
      if (clicked != null)
        clicked.GetComponent<Animator>().SetBool("Focused", false);
      clicked = null;
      gameObject.GetComponent<Feature>().SetFeatureType(FeatureType.Bomb);
      var toRemove = new List<GameObject>();
      toRemove.Add(gameObject);
      gameMgr.StartCoroutine(gameMgr.ProcessBoard(toRemove));
      IsBombUsed = false;
      return;
    }
    if (IsRecolorUsed == true)
    {
      if (clicked != null)
        clicked.GetComponent<Animator>().SetBool("Focused", false);
      clicked = null;
      gameObject.GetComponent<Feature>().SetFeatureType(FeatureType.Recolor);
      gameObject.GetComponent<Feature>().PerformFeature();
      gameObject.GetComponent<Feature>().SetFeatureType(FeatureType.None);
      gameMgr.StartCoroutine(gameMgr.GenerateCoroutine());
      IsRecolorUsed = false;
      return;
    }
    if (clicked == null)
    {
      clicked = gameObject;
      clicked.GetComponent<Animator>().SetBool("Focused", true);
    }
    else
    {
      clicked.GetComponent<Animator>().SetBool("Focused", false);
      //It was a very big mistake to start coroutine for object which was about to destroy
      //StartCoroutine(gameMgr.DoGameStep(clicked, gameObject));
      gameMgr.StartCoroutine(gameMgr.DoGameStep(clicked, gameObject));
      clicked = null;
    }
  }
}
