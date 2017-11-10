using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Math;

public class Board : MonoBehaviour
{
  public int Width = 8;
  public int Height = 8;
  public Cell[,] cells { get; private set; }

  private void Awake()
  {
    cells = new Cell[Width, Height];
    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        cells[i, j] = transform.GetChild(i * Width + j).GetComponent<Cell>();
      }
    }
  }

  public void Generate()
  {
    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        if (!cells[i, j].IsEmpty)
          continue;

        var item = ItemFactory.Instance.Create(i, j);
        //TODO: just for beauty :) calculate start position for new elements instead of "100"
        item.transform.position = cells[0, j].transform.position + new Vector3(0, 100, 0);
        cells[i, j].Item = item;
      }
    }
  }

  public bool HasElementWithStatus(string name, bool val)
  {
    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        if (cells[i, j].IsEmpty)
          continue;
        if (cells[i, j].Item.Animator.GetBool(name) == val)
        {
          return true;
        }
      }
    }
    return false;
  }

  public bool HasEmptyCell()
  {
    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        if (cells[i, j].IsEmpty)
          return true;
      }
    }
    return false;
  }

  public List<Item> GetMatchThreeElements()
  {
    var res = new List<Item>();

    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        if (cells[i, j].IsEmpty)
          continue;
        if (res.Contains(cells[i, j].Item))
          continue;
        if(!res.Contains(cells[i, j].Item)) //carefully check this change
          res.AddRange(GetMatchThreeElements(cells[i, j].Item));
      }
    }
    return res;
  }

  public void SwapElements(Item first, Item second)
  {
    Transform temp = first.transform.parent;
    first.transform.SetParent(second.transform.parent);
    second.transform.SetParent(temp);
    SwapIndexes(first, second);
  }

  void SwapIndexes(Item element, Item other)
  {
    int tX = element.X;
    int tY = element.Y;

    element.X = other.X;
    element.Y = other.Y;
    other.X = tX;
    other.Y = tY;
  }

  public List<Item> GetMatchThreeElements(Item item)
  {
    var result = new List<Item>();
    result.AddRange(GetElementsFromLine(item));
    result.AddRange(GetElemenetsFromColumn(item));

    if (result.Count != 0)
      result.Add(item);

    return result;
  }

  List<Item> GetElementsFromLine(Item item)
  {
    var result = new List<Item>();

    for (int i = item.X - 1; i >= 0; --i)
    {
      var go = cells[i, item.Y].Item;
      if (!IsSameElements(go, item))
        break;
      result.Add(go);
    }
    for (int i = item.X + 1; i < Width; ++i)
    {
      var go = cells[i, item.Y].Item;
      if (!IsSameElements(go, item))
        break;
      result.Add(go);
    }
    if (result.Count < 2)
      return new List<Item>();
    return result;
  }

  List<Item> GetElemenetsFromColumn(Item item)
  {
    var result = new List<Item>();

    for (int i = item.Y + 1; i < Height; ++i)
    {
      var go = cells[item.X, i].Item;
      if (!IsSameElements(go, item))
        break;
      result.Add(go);
    }
    for (int i = item.Y - 1; i >= 0; --i)
    {
      var go = cells[item.X, i].Item;
      if (!IsSameElements(go, item))
        break;
      result.Add(go);
    }
    if (result.Count < 2)
      return new List<Item>();
    return result;
  }

  bool IsSameElements(Item first, Item second)
  {
    if (first == null || second == null)
      return false;
    if (first.ID != second.ID)
    {
      return false;
    }
    return true;
  }

  List<Item> GetAbove(int row, int col)
  {
    var result = new List<Item>();
    for (int i = row; i >= 0; --i)
    {
      var go = cells[i, col].Item;
      if (go != null)
        result.Add(go);
    }
    return result;
  }

  public void RemoveHoles()
  {
    for (int j = 0; j < Width; ++j)
    {
      for (int i = Height - 1; i >= 0; --i)
      {
        if (cells[i, j].IsEmpty)
        {
          var objects = GetAbove(i, j);
          if (objects.Count == 0)
            break;
          int k = i;
          foreach (var obj in objects)
          {
            obj.LayoutElement.ignoreLayout = true;
            cells[k, j].Item = obj;
            obj.X = k;
            obj.Y = j;
            k--;
          }
          break;
        }
      }
    }
  }

  public bool IsNeighbours(Item first, Item second)
  {
    return Abs(first.X - second.X) < 2 && (first.Y == second.Y) || Abs(first.Y - second.Y) < 2 && (first.X == second.X);
  }

  bool CheckStep(Item first, Item second)
  {
    SwapElements(first, second);
    var list = GetMatchThreeElements(first);
    list.AddRange(GetMatchThreeElements(second));
    SwapElements(first, second);
    if (list.Count != 0)
    {
      return true;
    }
    return false;
  }

  public List<Item> GetCombination()
  {
    var res = new List<Item>();
    for (int i = 0; i < Width - 1; ++i)
    {
      for (int j = 0; j < Height - 1; ++j)
      {
        if (CheckStep(cells[i, j].Item, cells[i + 1, j].Item))
        {
          res.Add(cells[i, j].Item);
          res.Add(cells[i + 1, j].Item);
          return res;
        }
        if (CheckStep(cells[i, j].Item, cells[i, j + 1].Item))
        {
          res.Add(cells[i, j].Item);
          res.Add(cells[i, j + 1].Item);
          return res;
        }
      }
    }
    return res;
  }

  public void Clear()
  {
    for (int i = 0; i < Width; ++i)
    {
      for (int j = 0; j < Height; ++j)
      {
        if (!cells[i, j].IsEmpty)
          cells[i, j].Item.RemoveMe();
      }
    }
  }

  public List<Item> GetAround(Item bf, int count = 8)
  {
    var res = new List<Item>();
    for (int i = bf.X - 1; i <= bf.X + 1; ++i)
    {
      for (int j = bf.Y - 1; j <= bf.Y + 1; ++j)
      {
        if (i == bf.X && j == bf.Y)
          continue;
        if (i < 0 || i >= Width || j < 0 || j >= Height)
          continue;
        var go = cells[i, j].Item;
        if (go != null && go.Animator.GetBool("Destroyed") == false)
          res.Add(go);
      }
    }
    return res;
  }

  public List<Item> GetRandomElements(int count = 8)
  {
    var res = new List<Item>();
    for (int i = 0; i < count;)
    {
      var x = UnityEngine.Random.Range(0, Width);
      var y = UnityEngine.Random.Range(0, Height);
      var element = cells[x, y].Item;
      if (res.Contains(element) || element == null)
        continue;
      if (!element.Animator.GetBool("Destroyed"))
        res.Add(element);
      ++i;
    }

    return res;
  }

  public void RecolorElements(Item item, int count = 8)
  {
    var list = GetRandomElements(count - 1);
    list.Add(item);
    foreach (var it in list)
    {
      it.Recolor();
    }
  }
}
