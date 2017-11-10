using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public class Controller : MonoBehaviour
{
  static Item clicked;
  public GameManager gameMgr;
  public static bool IsBombUsed = false;
  public static bool IsRecolorUsed = false;

  //move Controller to Panel from Item 
  public void OnClick(BaseEventData data)
  {
    var item = ((PointerEventData)data).pointerCurrentRaycast.gameObject.GetComponent<Item>();
    //TODO: do not forget to set timer is On Action for these two cases
    if (IsBombUsed == true)
    {
      clicked?.Animator.SetBool("Focused", false);
      clicked = null;
      item.Feature.SetFeatureType(FeatureType.Bomb);
      var toRemove = new List<Item>();
      toRemove.Add(item);
      gameMgr.StartCoroutine(gameMgr.ProcessBoard(toRemove));
      IsBombUsed = false;
      return;
    }
    if (IsRecolorUsed == true)
    {
      clicked?.Animator.SetBool("Focused", false);
      clicked = null;
      item.Feature.SetFeatureType(FeatureType.Recolor);
      item.Feature.PerformFeature();
      item.Feature.SetFeatureType(FeatureType.None);
      gameMgr.StartCoroutine(gameMgr.GenerateCoroutine());
      IsRecolorUsed = false;
      return;
    }
    if (clicked == null)
    {
      clicked = item;
      clicked.Animator.SetBool("Focused", true);
    }
    else
    {
      clicked.Animator.SetBool("Focused", false);
      //It was a very big mistake to start coroutine for object which was about to destroy
      //StartCoroutine(gameMgr.DoGameStep(clicked, gameObject));
      gameMgr.StartCoroutine(gameMgr.DoGameStep(clicked, item));
      clicked = null;
    }
  }
}
