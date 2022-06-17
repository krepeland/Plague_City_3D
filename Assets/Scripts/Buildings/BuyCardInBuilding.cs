using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyCardInBuilding : MonoBehaviour, ITickable
{
    public int TicksTilAdd = 12;
    public int CardsCountLeftDelta = 1;
    public int CardsCountLeftMax = 3;
    public int ÑardsCountLeftNow;

    [HideInInspector] public int ticks;

    [SerializeField] List<BuyCardData> buyCardDatas = new List<BuyCardData>();

    void Start() {
        Ticker.AddTickable(this, TickablePriority.Medium);
        ticks = TicksTilAdd;
    }

    public void Tick(int luck)
    {
        if (ÑardsCountLeftNow >= CardsCountLeftMax)
            return;
        ticks -= 1;
        if (ticks < 1) {
            ticks = TicksTilAdd;
            ÑardsCountLeftNow = Mathf.Clamp(ÑardsCountLeftNow + CardsCountLeftDelta, 0, CardsCountLeftMax);
        }
    }

    public string GetText() {
        return $"{ÑardsCountLeftNow}/{CardsCountLeftMax} CARDS AVAILABLE TO BUY";
    }

    public List<BuyCardData> GetBuyCardDatas() {
        return buyCardDatas;
    }
}
