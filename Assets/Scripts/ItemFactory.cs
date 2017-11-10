using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemFactory : MonoBehaviour
{
  public GameObject ItemReference;
  public int Count = 64;
  public static ItemFactory Instance { get; private set; }

  void Awake()
  {
    Instance = this;
    for (int i = 0; i < Count; ++i)
    {
      var go = GameObject.Instantiate<GameObject>(ItemReference);
      go.transform.SetParent(gameObject.transform);
    }
  }

  void Start()
  {    
  }

  public Item Create(int x, int y)
  {
    GameObject go = null;
    if (transform.childCount == 0)
      go = GameObject.Instantiate<GameObject>(ItemReference);
    else
      go = transform.GetChild(0).gameObject;

    go.SetActive(true);
    var item = go.GetComponent<Item>();
    item.X = x;
    item.Y = y;
    item.Recolor();    
    return item;   
  }

  public void Remove(Item item)
  {
    item.transform.SetParent(transform);
    ResetComponents(item);
    item.Feature.Start();
    item.gameObject.SetActive(false);
  }

  void ResetComponents(Item item)
  {    
    item.transform.localScale = ItemReference.transform.localScale;
    item.transform.rotation = ItemReference.transform.rotation;
  }
}
