using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

  public int Width = 8;
  public int Height = 8;
  public GameObject ReferenceCell;
  public List<Color> colors = new List<Color>();

  Queue<GameObject> toRemove = new Queue<GameObject>();

  void Start()
  {
    GenerateDesk();
  }

  void GenerateDesk()
  {
    System.Random r = new System.Random();
    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        var obj = GameObject.Instantiate(ReferenceCell);
        Image img = obj.transform.GetChild(0).gameObject.GetComponent<Image>();
        int rand = r.Next(colors.Count - 1);
        img.color = colors[rand];
        obj.transform.SetParent(gameObject.transform);
        GameElement e = obj.transform.GetChild(0).gameObject.GetComponent<GameElement>();
        e.X = i;
        e.Y = j;
        e.ID = rand;
      }
    }
  }

  // Update is called once per frame
  void Update()
  {

  }

  GameObject GetCell(int i, int j)
  {
    return gameObject.transform.GetChild(i * Width + j).gameObject.transform.gameObject;
  }

  /* for( i= 0)
     for (j = 0)
     mass[i,j]*/

  GameObject Get(int i, int j)
  {
    var cell = gameObject.transform.GetChild(i * Width + j).gameObject.transform;
    if (cell.childCount > 0)
      return cell.GetChild(0).gameObject;
    return null;
  }

  bool IsNeighbours(GameObject first, GameObject second)
  {
    var e1 = first.GetComponent<GameElement>();
    var e2 = second.GetComponent<GameElement>();

    return (Math.Abs(e1.X - e2.X) < 2 && (e1.Y == e2.Y)) || (Math.Abs(e1.Y - e2.Y) < 2 && (e1.X == e2.X));
  }

  void SwapIndexes(GameObject element, GameObject other)
  {
    var e1 = element.GetComponent<GameElement>();
    var e2 = other.GetComponent<GameElement>();
    int tX = e1.X;
    int tY = e1.Y;

    e1.X = e2.X;
    e1.Y = e2.Y;
    e2.X = tX;
    e2.Y = tY;
  }

  void SwapElements(GameObject element, GameObject other)
  {
    Transform temp = element.transform.parent;
    element.transform.SetParent(other.transform.parent);
    other.transform.SetParent(temp);
    SwapIndexes(element, other);
  }

  bool AtLine(GameObject element)
  {
    //terrible method
    //rewrite it please
    //int count = 1;
    var ge = element.GetComponent<GameElement>();
    Queue<GameObject> result = new Queue<GameObject>();

    result.Enqueue(element);
    for (int i = ge.X - 1; i >= 0; --i)
    {
      //TODO: extract to func
      var go = Get(i, ge.Y);
      if (go == null)
        break;
      if (go.GetComponent<GameElement>().ID != element.GetComponent<GameElement>().ID)
      {
        break;
      }
      //count++;
      result.Enqueue(go);
    }
    for (int i = ge.X + 1; i < Width; ++i)
    {
      var go = Get(i, ge.Y);
      if (go == null)
        break;
      if (go.GetComponent<GameElement>().ID != element.GetComponent<GameElement>().ID)
      {
        break;
      }
      //count++;
      result.Enqueue(go);
    }
    if (result.Count >= 3) //match 3
    {
      toRemove = new Queue<GameObject>(toRemove.Concat(result));
      return true;
    }
    //count = 1;
    result.Clear();
    result.Enqueue(element);
    for (int i = ge.Y + 1; i < Height; ++i)
    {
      var go = Get(ge.X, i);
      if (go == null)
        break;
      if (go.GetComponent<GameElement>().ID != element.GetComponent<GameElement>().ID)
      {
        break;
      }
      //count++;
      result.Enqueue(go);
    }
    for (int i = ge.Y - 1; i >= 0; --i)
    {
      var go = Get(ge.X, i);
      if (go == null)
        break;
      if (go.GetComponent<GameElement>().ID != element.GetComponent<GameElement>().ID)
      {
        break;
      }
      //count++;
      result.Enqueue(go);
    }
    if (result.Count >= 3) //match 3
    {
      toRemove = new Queue<GameObject>(toRemove.Concat(result));
      return true;
    }
    return false;
  }

  public bool MoveElements(GameObject element, GameObject other)
  {
    //extract to separate func, which
    //checks behaviour availability also
    //(e.x. frozen element)

    if (!IsNeighbours(element, other))
    {
      return false;
    }

    SwapElements(element, other);
    //play animation

    if (!AtLine(element) && !AtLine(other))
    {
      SwapElements(element, other);
      //play animation?
      return false;
    }
    MatchThree();

    MoveAllDown();
    return true;
  }

  void MatchThree()
  {
    while (toRemove.Count > 0)
    {
      //TODO:
      //call behaviour.Exec() to add new elements to toRemove
      //call View.Exec() to play anim, etc.
      var go = toRemove.Dequeue();
      go.transform.SetParent(null);
      Destroy(go);
    }
  }

  List<GameObject> GetAbove(int row, int col)
  {
    var result = new List<GameObject>();
    for (int i = row; i >= 0; --i)
    {
      var go = Get(i, col);
      if (go != null)
        result.Add(go);
    }
    return result;
  }

  void MoveAllDown()
  {
    for (int j = 0; j < Width; ++j)
    {
      for (int i = Height - 1; i >= 0; --i)
      {
        var go = Get(i, j);
        if (go == null)
        {
          var objects = GetAbove(i, j);
          if (objects.Count == 0)
            break;
          int k = i;
          foreach (var obj in objects)
          {
            obj.transform.SetParent(GetCell(k, j).transform);
            var ge = obj.GetComponent<GameElement>();
            ge.X = k;
            ge.Y = j;
            k--;
          }
        }
      }
    }
  }
}
