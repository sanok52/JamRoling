using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SpinLeaderBoardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text leadersTmp;
    [SerializeField] private TMP_Text firstWallTmp;
    [SerializeField] private TMP_Text secondWallTmp;
    [SerializeField] private int countInOneWall = 20;

    private string prevLeader = "Player";

    public event Action<string> OnLeadersChanged;

    public void ChangeProgeressWork(Dictionary<string, int> dictionary)
    {
        List<LeaderboardData> datas = new List< LeaderboardData >();
        foreach (var item in G.GamerManager.SpinGamers)
        {
            int count = dictionary.ContainsKey(item.Key) ? dictionary[item.Key] : 0;
            datas.Add(new LeaderboardData(item.Value.Name, count, item.Value.IsDead));
        }

        var sorted = datas
            .OrderBy(d => d.IsDead)
            .ThenByDescending(d => d.Count)
            .ToList();

        leadersTmp.text = "Leaders:\n";
        int leaderCount = 3;
        for (int i = 0; i < leaderCount; i++)
        {
            var data = sorted[i];
            WriteTextAtWall(leadersTmp, i, data);
        }

        if (sorted[0].Name != prevLeader)
        {
            prevLeader = sorted[0].Name;
            OnLeadersChanged?.Invoke(prevLeader);
        }

        firstWallTmp.text = "";
        secondWallTmp.text = "";

        TMP_Text currentWall = firstWallTmp;
        for (int i = leaderCount; i < countInOneWall + leaderCount && i < sorted.Count; i++)
        {
            LeaderboardData data = sorted[i];
            WriteTextAtWall(firstWallTmp, i, data);
        }
        for (int i = countInOneWall + leaderCount; i < sorted.Count; i++)
        {
            LeaderboardData data = sorted[i];
            WriteTextAtWall(secondWallTmp, i, data);
        }
    }

    private static void WriteTextAtWall(TMP_Text currentWall, int i, LeaderboardData data)
    {
        string text = $"{i + 1}. {data.Count} - {data.Name}";
        currentWall.text += data.IsDead ? $"<color=orange>{text}[Dead]</color>\n" : (text + "\n");        
    }

    public struct LeaderboardData
    {
        public string Name;
        public int Count;
        public bool IsDead;

        public LeaderboardData(string name, int count, bool isDead)
        {
            Name = name;
            Count = count;
            IsDead = isDead;
        }
    }
}
