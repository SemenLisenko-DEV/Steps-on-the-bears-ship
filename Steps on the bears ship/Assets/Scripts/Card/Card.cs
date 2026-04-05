using ActionDatabase;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Card : MonoBehaviour, IAction
{
    public string id;
    public string cardTier;
    //сохранить:
    public bool isTaken = false;
    //дальше не сохранять

    public static List<Card> cards = new List<Card>();

    [SerializeField] private AudioDictionary _audioDictionary;
    public UnityEvent onTake;
    public void Awake()
    {
        cards.Add(this);
        Load();
        SaveLoadControl.SaveEvent += Save;
    }
    public void StartEvent()
    {
        Noise.MakeNoise(transform.position, 15, _audioDictionary.Find("Take"));
        PlayerControl.Instance.cardTaken.Add(cardTier);
        List<Card> cardsToDestroy = new List<Card>();
        isTaken = true;
        foreach (Card card in cards)
        {
            if(card.cardTier == cardTier)
            {
                card.isTaken = true;
                cardsToDestroy.Add(card);
            }
        }
        if (onTake != null)
        {
            onTake.Invoke();
        }
        foreach (Card card in cardsToDestroy)
        {
            card.gameObject.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        SaveLoadControl.SaveEvent -= Save;
        cards.Remove(this);
    }
    public void Save()
    {
        if (Equals(id, "")) { return; }
        SaveLoadControl.gameData.Remove(ref SaveLoadControl.gameData.cardDatas, id);
        SaveLoadControl.gameData.cardDatas.Add(new CardData(this));
    }
    public void Load()
    {
        CardData cardData = SaveLoadControl.gameData.GetData(ref SaveLoadControl.gameData.cardDatas, id);
        if (cardData == null) { return; }
        isTaken = cardData.isTaken;
        if (isTaken)
        {
            Destroy(gameObject);
        }
    }
}
