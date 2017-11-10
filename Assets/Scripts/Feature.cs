using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FeatureType
{
  Bomb,
  Lighting,
  None,
  Recolor
}

public class Feature : MonoBehaviour
{
  public FeatureType fType;
  public Board board;

  public void Start()
  {
    if (Random.Range(0, 40) == 1)
    {
      SetFeatureType((FeatureType)Random.Range(0, 3));
    }    
  }

  public void SetFeatureType(FeatureType type)
  {
    fType = type;
    GetComponent<Animator>().SetInteger("FeatureType", (int)fType);
  }

  private void FixedUpdate()
  {
    if (gameObject.GetComponent<Animator>().GetInteger("FeatureType") != (int)fType)
      SetFeatureType(fType);
  }
  public List<Item> PerformFeature()
  {
    var res = new List<Item>();
    switch (fType)
    {
      case FeatureType.Bomb:
        {
          res.AddRange(board.GetAround(GetComponent<Item>()));
          break;
        }
      case FeatureType.Lighting:
        {
          res.AddRange(board.GetRandomElements());
          break;
        }
      case FeatureType.None:
        {
          break;
        }
      case FeatureType.Recolor:
        {
          board.RecolorElements(GetComponent<Item>());
          break;
        }
    }
    return res;
  }
}
