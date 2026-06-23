using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpinGamerManager
{
    private Dictionary<string, SpinGamer> spinGamers = new Dictionary<string, SpinGamer>();
    private Dictionary<string, int> gamersCounts = new Dictionary<string, int>();

    public int CountGamers => spinGamers.Values.Where(x => !x.IsDead).Count();
    public Dictionary<string, SpinGamer> SpinGamers => spinGamers;


    public event Action<Dictionary<string, int>> OnChangeProgress;
    public event Action<string, int> OnGamerProgress;
    public event Action<SpinGamer> OnGamerDead;

    public void KillLast()
    {
        string minID = string.Empty;
        int minCount = int.MaxValue;
        foreach (var item in gamersCounts)
        {
            Debug.Log($"{item.Key} {item.Value} < {minCount} ({minID})");
            if(!spinGamers[item.Key].IsDead && item.Value < minCount)
            {
                minID = item.Key;
                minCount = item.Value;
            }
        }

        Kill(spinGamers[minID]);
    }

    private void Kill(SpinGamer spinGamer)
    {
        spinGamer.IsDead = true;
        OnGamerDead?.Invoke(spinGamer);
    }

    public void Init(int Gamers)
    {
        spinGamers.Add("Player", new SpinGamer()
        {
            ID = "Player",
            Name = "N-451",
            View = null
        });

        var views = UnityEngine.Object.FindObjectsByType<SpinGamerView>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        int num = 111;
        for (int i = 0; i < views.Length && i < Gamers; i++) {
            SpinGamerView item = views[i];
            OnGamerProgress += item.TryUpdateProgress;

            gamersCounts.Add(item.ID, 0);
            if (item.ID == "Player")
            {
                spinGamers["Player"].View = item;
                continue;
            }

            num += UnityEngine.Random.Range(1, 10);
            if (num == 451)
                num++;

            spinGamers.Add(item.ID, new SpinGamer()
            {
                ID = item.ID,
                Name = $"N-{num}",
                View = null
            });
        }
        

        if (spinGamers["Player"].View == null)
        {
            spinGamers["Player"].View = views.First(x => x.ID == "Player");
        }
    }

    public void PlayerProgress(int count)
    {
        GamerProgress("Player", count);
    }

    public void GamerProgress(string id, int count)
    {
        if(gamersCounts.ContainsKey(id) == false)
            gamersCounts.Add(id, count);
        else
            gamersCounts[id] += count;

        OnGamerProgress?.Invoke(id, gamersCounts[id]);
        OnChangeProgress?.Invoke(gamersCounts);
    }

    public void ClearAllProgress()
    {
        foreach (var item in gamersCounts)
        {
            gamersCounts[item.Key] = 0;
            OnGamerProgress?.Invoke(item.Key, 0);
        }
        
        OnChangeProgress?.Invoke(gamersCounts);
    }

    [Serializable]
    public class SpinGamer
    {
        public string ID;
        public string Name;
        public SpinGamerView View;
        public bool IsDead;
    }
}