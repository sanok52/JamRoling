using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpinGamerManager
{
    private Dictionary<string, SpinGamer> spinGamers = new Dictionary<string, SpinGamer>();
    private Dictionary<string, int> gamersCounts = new Dictionary<string, int>();
    private bool isGamePlay;

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
            //Debug.Log($"{item.Key} {item.Value} < {minCount} ({minID})");
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
        if(spinGamer.View != null)
            spinGamer.View.Dead();
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
        gamersCounts.Add("Player", 0);

        var views = UnityEngine.Object.FindObjectsByType<SpinGamerView>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        PocketRandomazer.CreatePocket("EnemyHard",
            new PocketRandomDataCreate<float>(0.5f, 6),
            new PocketRandomDataCreate<float>(1f, 9),
            new PocketRandomDataCreate<float>(1.5f, 3));

        int num = 111;
        for (int i = 0; i < views.Length && i < Gamers; i++) {
            SpinGamerView item = views[i];
            OnGamerProgress += item.TryUpdateProgress;

            if (item.ID == "Player")
            {
                spinGamers["Player"].View = item;
                continue;
            }

            gamersCounts.Add(item.ID, 0);
            num += UnityEngine.Random.Range(1, 10);
            if (num == 451)
                num++;


            spinGamers.Add(item.ID, new SpinGamer()
            {
                ID = item.ID,
                Name = $"N-{num}",
                View = item,
                coefSin = UnityEngine.Random.Range(0.1f, 1f),
                offsetSin = UnityEngine.Random.Range(0, 10f),
                coefSpeed = UnityEngine.Random.Range(1, 5f) * PocketRandomazer.GetRandomElement<float>("EnemyHard")
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

    public void SetPlayState(bool isGamePlay)
    {
        this.isGamePlay = isGamePlay;
    }

    public void Update()
    {
        if (!isGamePlay)
            return;

        foreach (var gamer in spinGamers.Values)
        {
            if (gamer.IsDead || gamer.ID == "Player")
                continue;

            float deltaC = 1f;
            int deltaAtPlayer = gamersCounts["Player"] - gamersCounts[gamer.ID];
            if (deltaAtPlayer < -7)
            {
                if (deltaAtPlayer < -25)
                    deltaC = 0.3f;
                else
                    deltaC = 0.7f;
            }
            else if (deltaAtPlayer > 10)
                deltaC = 1.1f;
            else if (deltaAtPlayer > 20)
                deltaC = 1.5f;

            bool isFog = G.SpinGameFlow.GameMode == SpinGameFlow.SpinGameMode.Fog;

            int progress = gamer.Progress(deltaC * (isFog ? 0.1f : 1f));
            if(progress > 0)
                GamerProgress(gamer.ID, progress * (isFog ? 20 : progress));
        }
    }

    [Serializable]
    public class SpinGamer
    {
        public string ID;
        public string Name;
        public SpinGamerView View;
        public bool IsDead;

        [Space]
        public float coefSpeed = 1f;
        public float offsetSin = 0f;
        public float coefSin = 1f;

        public float currentProgress; 

        public int Progress(float deltaCoef)
        {
            currentProgress += coefSpeed * Mathf.Abs(Mathf.Sin((Time.time * coefSin) + offsetSin)) * deltaCoef * Time.deltaTime;
            if (currentProgress >= 1f)
            {
                currentProgress = 0;
                return 1;
            }
            else
                return 0;
        }
    }
}