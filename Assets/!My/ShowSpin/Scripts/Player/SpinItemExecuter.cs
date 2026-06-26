using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpinItemExecuter : MonoBehaviour
{
    private SpinResourceData data;
    private Dictionary<string, string[]> textAsset = new Dictionary<string, string[]>();

    private List<SpinItemInstance> itemInstances = new List<SpinItemInstance>();

    public List<SpinItemInstance> ItemInstances => itemInstances;


    private List<SpinItemInfo> fortuneItems = new List<SpinItemInfo>();
    public List<SpinItemInfo> FortuneItems { get => fortuneItems; set => fortuneItems = value; }

    public void Init()
    {
        data = Resources.Load<SpinResourceData>("SpinResourceData");
        textAsset = TSVReader.ReadTxtFromResources("SpinText3");

        FortuneItems = data.itemInfos.Where(x => !x.isVictorin).ToList();
        PocketRandomazer.CreatePocket<SpinItemInfo>("Victorin", 2, data.itemInfos.Where(x => x.isVictorin).ToArray());
        PocketRandomazer.CreatePocket<SpinItemInfo>("Fortune", 2, FortuneItems.ToArray());
    }

    public SpinItemInfo GetRandomVictorinItem()
    {
        return PocketRandomazer.GetRandomElement<SpinItemInfo>("Victorin");
    }

    public SpinItemInfo GetRandomFortuneItem()
    {
        return PocketRandomazer.GetRandomElement<SpinItemInfo>("Fortune");
    }

    public void AddItemInList (string id)
    {
        var item = CreateInstance(id);
        itemInstances.Add(item);

        if(item.itemInfo.itemBehaviour != null)
            item.itemInfo.itemBehaviour.ExecuteItem(GetContext(BehActionType.Get));
    }

    public void InvokeEvent (BehActionType actionType, params object[] items)
    {
        foreach (var item in ItemInstances)
        {
            if (item.itemInfo.itemBehaviour != null)
                item.itemInfo.itemBehaviour.ExecuteItem(GetContext(actionType, items));
        }
    }

    public SpinItemInstance CreateInstance (string id)
    {
        return new SpinItemInstance()
        {
            itemInfo = GetInfo(id),
        };
    }

    public SpinItemInfo GetInfo(string id) => data.itemInfos.First(x => x.ID == id);

    public SpinItemInfo GetPureInfo (SpinItemInfo info)
    {
        string keyInfo = DictorSpeachManager.language == Language.RU ? "InfoRU" : "InfoEN";
        string keyName = DictorSpeachManager.language == Language.RU ? "NameRU" : "NameEN";
        var infoTxt = TSVReader.GetIdLineAsDict(textAsset, info.ID);

        return new SpinItemInfo()
        {
            ID = info.ID,
            Name = infoTxt[keyName],
            Description = infoTxt[keyInfo].Replace("[Value]", info.GetTextValue()).Replace("[Value2]", info.GetTextValue2()),
            isVictorin = info.isVictorin,
            itemBehaviour = info.itemBehaviour,
        };
    }

    private SpinItemBehContext GetContext(BehActionType actionType, params object[] items)
    {
        return new SpinItemBehContext()
        {
            ActionType = actionType,
            items = items
        };
    }
}

[Serializable]
public struct SpinItemInfo
{
    public string ID;
    [HideInInspector] public string Name;
    [HideInInspector] public string Description;
    public bool isVictorin;

    [Space]
    public SpinItemBehaviour itemBehaviour;

    public string GetTextValue()
    {
        if (itemBehaviour)
            return itemBehaviour.GetTextValue;
        return "?";
    }

    public string GetTextValue2()
    {
        if (itemBehaviour)
            return itemBehaviour.GetTextValue2;
        return "?";
    }
}

[Serializable]
public class SpinItemInstance
{
    public SpinItemInfo itemInfo;
}