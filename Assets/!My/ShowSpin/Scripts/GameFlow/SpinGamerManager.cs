using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SpinLeaderBoardUI;

public class SpinGamerManager
{
    private Dictionary<string, SpinGamer> spinGamers = new Dictionary<string, SpinGamer>();
    private Dictionary<string, int> gamersScores = new Dictionary<string, int>();
    private bool isGamePlay;
    private Vector2 fastAndSlow = new Vector2(10f, 15f); //fast, slow

    public int CountGamers => spinGamers.Values.Where(x => !x.IsDead).Count();
    public Dictionary<string, SpinGamer> SpinGamers => spinGamers;
    public Dictionary<string, int> GamersScores => gamersScores;

    public event Action<Dictionary<string, int>> OnChangeProgress;
    public event Action<string, int> OnGamerProgress;
    public event Action<string, int> OnGamerProgressDelta;
    public event Action<SpinGamer, bool> OnGamerBroke;
    public event Action<SpinGamer> OnGamerDead;

    public SpinGamerManager.SpinGamer GetLast()
    {
        string minID = string.Empty;
        int minCount = int.MaxValue;
        foreach (var item in gamersScores)
        {
            //Debug.Log($"{item.Key} {item.Value} < {minCount} ({minID})");
            if (!spinGamers[item.Key].IsDead && item.Value < minCount)
            {
                minID = item.Key;
                minCount = item.Value;
            }
        }

        return spinGamers[minID];
    }

    public void Kill(SpinGamer spinGamer)
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
        gamersScores.Add("Player", 0);

        var views = UnityEngine.Object.FindObjectsByType<SpinGamerView>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        PocketRandomazer.CreatePocket("EnemyHard",
            new PocketRandomDataCreate<float>(0.5f, 6),
            new PocketRandomDataCreate<float>(1f, 9),
            new PocketRandomDataCreate<float>(1.5f, 3));

        int num = 111;
        for (int i = 0; i < views.Length; i++) {
            SpinGamerView item = views[i];

            if(i >= Gamers)
            {
                if (item.ID != "Player")
                    item.gameObject.SetActive(false);
                continue;
            }    

            OnGamerProgress += item.TryUpdateProgress;

            if (item.ID == "Player")
            {
                spinGamers["Player"].View = item;
                Gamers++;
                continue;
            }

            gamersScores.Add(item.ID, 0);
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
                coefSpeed = UnityEngine.Random.Range(3f, 5f) * PocketRandomazer.GetRandomElement<float>("EnemyHard")
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
        if(gamersScores.ContainsKey(id) == false)
            gamersScores.Add(id, count);
        else
            gamersScores[id] += count;

        OnGamerProgress?.Invoke(id, gamersScores[id]);
        OnGamerProgressDelta?.Invoke(id, count);
        OnChangeProgress?.Invoke(gamersScores);
    }

    public void ClearAllProgress()
    {
        foreach (var item in gamersScores)
        {
            gamersScores[item.Key] = 0;
            OnGamerProgress?.Invoke(item.Key, 0);
        }
        
        OnChangeProgress?.Invoke(gamersScores);
    }

    public void SetPlayState(bool isGamePlay)
    {
        this.isGamePlay = isGamePlay;
    }

    float speedTimer = 0f;
    bool isFast;

    public void Update()
    {
        if (!isGamePlay)
            return;

        speedTimer += Time.deltaTime;
        if(speedTimer >= (isFast ? fastAndSlow.x : fastAndSlow.y))
        {
            speedTimer = 0;
            isFast = !isFast;
        }

        foreach (var gamer in spinGamers.Values)
        {
            if (gamer.IsDead || gamer.ID == "Player")
                continue;

            if (gamer.IsBroke)
            {
                gamer.brokeWait -= Time.deltaTime;
                if (gamer.brokeWait < 0)
                    Broke(gamer.ID, false);
                continue;
            }

            float deltaC = 1f;
            int deltaAtPlayer = gamersScores["Player"] - gamersScores[gamer.ID];
            if (deltaAtPlayer < -7)
            {
                if (deltaAtPlayer < -25)
                    deltaC = 0.25f;
                else
                    deltaC = 0.7f;
            }
            else if (deltaAtPlayer > 10)
                deltaC = 1.1f;
            else if (deltaAtPlayer > 20)
                deltaC = 1.3f;
            else if (deltaAtPlayer > 30)
                deltaC = 1.5f;

            bool isFog = G.SpinGameFlow.GameMode == SpinGameFlow.SpinGameMode.Fog;

            deltaC *= isFast ? 1f : 0.65f;

            int progress = gamer.Progress(deltaC * (isFog ? 0.1f : 1f));
            if(progress > 0)
                GamerProgress(gamer.ID, progress * (isFog ? 20 : progress));
        }
    }

    public void BrokePlayer (bool isBroke)
    {
        Broke("Player", isBroke);
    }

    public void Broke (string id, bool isBroke)
    {
        if (spinGamers[id].IsBroke == isBroke)
            return;

        spinGamers[id].IsBroke = isBroke;
        spinGamers[id].brokeWait = Mathf.Clamp(UnityEngine.Random.Range(9f, 15f) / spinGamers[id].coefSpeed, 9f, 18f);
        if (isBroke)
            spinGamers[id].View.FixAnim();
        else
            spinGamers[id].View.EndFix();

        OnGamerBroke?.Invoke(spinGamers[id], isBroke);
    }

    public SpinGamer[] GetLeaders(int leaders)
    {
        List<LeaderboardData> datas = new List<LeaderboardData>();
        Dictionary<string, SpinGamer> keyValues = new Dictionary<string, SpinGamer>();
        foreach (var item in spinGamers)
        {
            int count = gamersScores.ContainsKey(item.Key) ? gamersScores[item.Key] : 0;
            var data = new LeaderboardData(item.Value.Name, count, item.Value.IsDead, item.Value.IsBroke);
            datas.Add(data);
            keyValues.Add(data.Name, item.Value);
        }

        var sorted = datas
            .OrderBy(d => d.IsDead)
            .ThenByDescending(d => d.Count)
            .ToList().Where(x => !x.IsDead).ToList();

        List<SpinGamer> gamers = new List<SpinGamer>();
        for (int i = 0; i < sorted.Count && i < leaders; i++)
        {
            LeaderboardData item = sorted[i];
            gamers.Add(keyValues[item.Name]);
        }

        return gamers.ToArray();
    }

    [Serializable]
    public class SpinGamer
    {
        public string ID;
        public string Name;
        public SpinGamerView View;
        public bool IsDead;
        public bool IsBroke;

        [Space]
        public float coefSpeed = 1f;
        public float offsetSin = 0f;
        public float coefSin = 1f;
        public float brokeWait = 5f;

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