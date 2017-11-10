using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
  public int X { get; set; }
  public int Y { get; set; }
  public int ID { get; set; }
  public Animator Animator { get; private set; }
  public Feature Feature { get; private set; }
  public Image Image { get; private set; }
  public LayoutElement LayoutElement { get; private set; }
  public List<Color> Colors = new List<Color>();

  public void Awake()
  {
    Animator = GetComponent<Animator>();
    Feature = GetComponent<Feature>();
    Image = GetComponent<Image>();
    LayoutElement = GetComponent<LayoutElement>();
  }

  public void Recolor()
  {
    ID = Random.Range(0, Colors.Count);
    Image.color = Colors[ID];
  }

  public void RemoveMe()
  {
    ItemFactory.Instance.Remove(this);
  }
}
