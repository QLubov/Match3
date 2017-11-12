using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreCounter
{
  public uint Score { get; private set; }

  public void Add(uint val)
  {
    Score += val;
  }
}

public class BonusCounter : MonoBehaviour
{
  public uint minReward = 1;
  public ScoreCounter counter { get; } = new ScoreCounter();
  List<int> chain = new List<int>();

  public void SetUserInput(List<Item> input)
  {
    chain.Add(input[0].ID);
    if (Verify(chain))
    {
      if (chain.Count == 2)
      {
        Debug.Log("chain has 2 elements!");
        //set feature
      }
      else if (chain.Count == 3)
      {
        Debug.Log("chain has 3 elements!");
        //set mega-feature
        chain.Clear();
      }
    }
    else
    {
      chain.Clear();
    }
  }

  bool Verify(List<int> IDs)
  {
    return IDs.Where(e => e != IDs[0]).Count() == 0;
  }

  public void CalculateScore(List<Item> items)
  {
    counter.Add((uint)items.Count * minReward);
  }
}
