using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_House : PlacedBuilding, IInfectable
{
    public int Size { get; private set; }
    public EFeedState FeedState = EFeedState.Feed;
    private int _discontent;
    public int Discontent 
    {
        get => _discontent; 
        set
        {
            var delta = value - _discontent;
            _discontent = value;
            DiscontentManager.TotalDiscontent += delta;
        } 
    }
    public int HungerStage;

    public int Immunity;
    public int HealedImmunity;
    public float TimeToDeath;


    int _totalPeople;
    int _healthyPeople;
    int _infectedPeople;
    int _deadPeople;

    public int HealthyPeople
    {
        get => _healthyPeople;
        set
        {
            var delta = value - _healthyPeople;
            _healthyPeople = value;
            TotalPeople += delta;
            TownManager.singleton.AddHealthyPeopleCount(delta);
        }
    }

    public int InfectedPeople
    {
        get => _infectedPeople;
        set
        {
            var delta = value - _infectedPeople;
            _infectedPeople = value;
            TotalPeople += delta;
            TownManager.singleton.AddInfectedPeopleCount(delta);
        }
    }

    public int DeadPeople
    {
        get => _deadPeople;
        set
        {
            var delta = value - _deadPeople;
            _deadPeople = value;
            TotalPeople += delta;
            TownManager.singleton.AddDeadPeopleCount(delta);
        }
    }

    public int TotalPeople
    {
        get => _totalPeople;
        set
        {
            var delta = value - _totalPeople;
            _totalPeople = value;
            TownManager.singleton.AddTotalPeopleCount(delta);
        }
    }


    public float ImmunityMultiplier = 1f;

    public bool IsEmptyHouse;

    public static int TimeToDeathSample = 18;
    public static int TimeToDeathRandom = 6;

    public static int MaxImmunity = 100;
    public static int ImmunityPerHuman = 10;
    bool specialUpdatePeopleCount;
    public bool IsHouseDestroyed;

    public void InitData(int baseSize) {
        Size = baseSize * buildingData.floors;
        HealthyPeople = 4 * Size;

        TimeToDeath += Random.Range(
            TimeToDeathSample - TimeToDeathRandom,
            TimeToDeathSample + TimeToDeathRandom + 1);
        Immunity = ImmunityPerHuman;

        UpdatedInfectionState();
        UpdateFeedState();

        DiscontentManager.MaxDiscontent += 100;
        if (GameManager.LevelData == null)
        {
            SetDiscontent(0);
        }
        else
        {
            switch (GameManager.LevelData.DiscontentProblems)
            {
                case ELevelData_DiscontentProblems.NoDiscontentProblems:
                    SetDiscontent(0);
                    break;
                case ELevelData_DiscontentProblems.WeakDiscontentProblems:
                    SetDiscontent(Random.Range(5, 20));
                    break;
                case ELevelData_DiscontentProblems.StrongDiscontentProblems:
                    SetDiscontent(Random.Range(10, 50));
                    break;
            }
        }
        //SetDiscontent(Random.Range(0, 10));
    }

    public void AddDiscontent(int delta)
    {
        SetDiscontent(Discontent + delta);
    }

    public void SetDiscontent(int value)
    {
        Discontent = Mathf.Clamp(value, 0, 100);
        UpdateDiscontentDisplay();
    }

    public void UpdateDiscontentDisplay()
    {
        if (isDestroyed)
        {
            return;
        }

        var value = Discontent;
        if (IsEmptyHouse)
            value = 0;

        foreach (var e in buildingData.poses)
        {
            DiscontentManager.SetDiscontentTextureState(e.x, e.y, value, IsEmptyHouse);
        }
        TryUpdateBuildingInfo();
    }

    public void AddHungerTick(int delta) {
        HungerStage = Mathf.Clamp(HungerStage + delta, 0, 10);
        HealedImmunity = Mathf.Clamp(HealedImmunity - 30, 0, 75);

        if (delta > 0) {
            var value = HungerStage + 1;
            AddDiscontent(delta * Random.Range(value, value * DiscontentManager.DiscontentForHungerTick));
        }

        if (HungerStage > 4)
            KillInfected(1);

        if (HungerStage > 6)
        {
            KillInfected(Random.Range(0, HungerStage / 2));
            KillHealthy(Random.Range(0, HungerStage / 2));
        }

        if (HealthyPeople <= 0 && InfectedPeople <= 0)
        {
            IsEmptyHouse = true;
            SetDiscontent(0);
        }

        CalculateEnemyUnitsSpawn();

        UpdatedInfectionState();
    }

    void CalculateEnemyUnitsSpawn() {

        int count = 0;
        int chance = 0;

        if (Discontent >= 20 && Discontent < 30) {
            chance = 2;
            count = 1;
        }

        if (Discontent >= 30 && Discontent < 50)
        {
            chance = 5;
            count = 1;
        }

        if (Discontent >= 50 && Discontent < 70)
        {
            chance = 10;
            count = 1;
        }

        if (Discontent >= 70 && Discontent < 80)
        {
            chance = 20;
            count = 1;
        }

        if (Discontent >= 80 && Discontent < 90)
        {
            chance = 50;
            count = 2;
        }

        if (Discontent >= 90)
        {
            chance = 75;
            count = 4;
        }


        if (TryWithChance(chance))
        {
            AddDiscontent(-Random.Range(10, 21));
            SpawnEnemyUnits(Random.Range(0, count) + 1);
        }
    }

    bool TryWithChance(int chance) {
        return Random.Range(0, 100) < chance;
    }

    void SpawnEnemyUnits(int count) {
        for (var i = 0; i < count; i++) {
            if (HealthyPeople + InfectedPeople <= 0)
                return;

            var rand = Random.Range(0, HealthyPeople + InfectedPeople);
            if (rand < HealthyPeople)
            {
                //Spawn angry
                var unit = UnitManager.singleton.SpawnAndInitEnemy("EnemyTest", buildingData.poses[Random.Range(0, buildingData.poses.Length)], this, false);
                if (unit != null)
                {
                    HealthyPeople -= 1;
                }
            }
            else
            {
                //Spawn infected
                var unit = UnitManager.singleton.SpawnAndInitEnemy("EnemyTest", buildingData.poses[Random.Range(0, buildingData.poses.Length)], this, false);
                if (unit != null)
                {
                    InfectedPeople -= 1;
                }
            }
            specialUpdatePeopleCount = true;
        }
    }

    public void ChangeImmunty(int delta)
    {
        if (delta < 0)
        {
            if (HealedImmunity > 0)
            {
                if (HealedImmunity > -delta)
                {
                    HealedImmunity += delta;
                    delta = 0;
                }
                else
                {
                    delta += HealedImmunity;
                    HealedImmunity = 0;
                }
            }
        }
        if (delta < 0 && !InfectionManager.singleton.InfectedObjects.Contains(this))
            InfectionManager.singleton.AddInfectedHouse(this);

        Immunity = Mathf.Clamp(Immunity + delta, -100, MaxImmunity);
        CalculateInfection();
    }

    public void InfectHealthy(int count)
    {
        if (!InfectionManager.singleton.InfectedObjects.Contains(this))
            InfectionManager.singleton.AddInfectedHouse(this);

        if (count > HealthyPeople)
            count = HealthyPeople;

        HealthyPeople -= count;
        InfectedPeople += count;

        AddDiscontent(DiscontentManager.DiscontentForInfected * count);
        UpdatedInfectionState();
    }

    public void KillInfected(int count) {
        if (count > InfectedPeople)
            count = InfectedPeople;

        DeadPeople += count;
        InfectedPeople -= count;

        AddDiscontent(DiscontentManager.DiscontentForDeadInfected * count);
        UpdatedInfectionState();
    }

    public void KillHealthy(int count)
    {
        if (count > HealthyPeople)
            count = HealthyPeople;

        DeadPeople += count;
        HealthyPeople -= count;

        AddDiscontent(DiscontentManager.DiscontentForDeadHealthy * count);
        UpdatedInfectionState();
    }

    public void KillRandom(int count) {
        for (var i = 0; i < count; i++) {
            var rand = Random.Range(1, HealthyPeople + InfectedPeople + 1);
            if (rand > HealthyPeople) {
                KillInfected(1);
            }
            else{
                KillHealthy(1);
            }
        }
    }

    public override bool IsCanBeBurnedDown()
    {
        return !IsEmptyHouse;
    }

    public override void BurnDown(int count)
    {
        base.BurnDown(count);
        KillRandom(count);

        UpdatedInfectionState();
        
        //KillHealthy(HealthyPeople);
        //KillInfected(InfectedPeople);
        //IsEmptyHouse = true;
        //UpdatedInfectionState();
    }

    public void Heal(int count)
    {
        if (InfectedPeople > 0) {
            var res = Mathf.Min(InfectedPeople, count);

            InfectedPeople -= res;
            HealthyPeople += res;

            HealedImmunity = Mathf.Clamp(HealedImmunity + 30, 0, 75);
            AddDiscontent(-2);
        }
        AddDiscontent(-2);
        UpdatedInfectionState();
    }

    public void CalculateInfection()
    {
        if (IsEmptyHouse)
        {
            UpdatedInfectionState();
            return;
        }

        while (Immunity < 0) {
            Immunity += ImmunityPerHuman;
            InfectHealthy(1);
        }

        if (InfectedPeople > 0 && HealthyPeople > 0)
        {
            Immunity -= 2 + InfectedPeople;
        }
        else {
            Immunity = 0;
        }

        if (InfectedPeople > 0)
        {
            TimeToDeath -= Mathf.Max(InfectedPeople * 0.5f, 1f);
            while (TimeToDeath <= 0)
            {
                TimeToDeath += Random.Range(
                    TimeToDeathSample - TimeToDeathRandom,
                    TimeToDeathSample + TimeToDeathRandom);
                KillInfected(1);
            }
        }

        if (InfectedPeople <= 0 && HealthyPeople <= 0)
            IsEmptyHouse = true;
        UpdatedInfectionState();
    }

    public bool CheckIsCanBeInfected()
    {
        return !IsEmptyHouse;
    }

    public PlacedBuilding TryGetPlacedBuilding()
    {
        return this;
    }

    public override IEnumerable<IBuildingInfoBlock> GetBuildingInfo()
    {
        var block = BuildingInfo.singleton.SpawnBuildingInfoBlock(BuildingInfoBlockType.HouseData);
        ((BI_HouseData)block).Init(HealthyPeople, InfectedPeople, DeadPeople);
        yield return block;

        block = BuildingInfo.singleton.SpawnBuildingInfoBlock(BuildingInfoBlockType.Text);
        ((BI_Text)block).Init($"Healthy: {HealthyPeople}\nInfected: {InfectedPeople}\nDead: {DeadPeople}");
        yield return block;

        block = BuildingInfo.singleton.SpawnBuildingInfoBlock(BuildingInfoBlockType.Text);
        ((BI_Text)block).Init($"Discontent: {Discontent}%");
        yield return block;
    }

    public override void UpdateSelectedBuildingInfo()
    {
        base.UpdateSelectedBuildingInfo();
        BuildingInfo.singleton.SetInfectedStatus(GetEInfectedBuildingStatus());

        var block = BuildingInfo.singleton.buildingInfoBlocks[0];
        if (specialUpdatePeopleCount)
        {
            specialUpdatePeopleCount = false;
            ((BI_HouseData)block).Init(HealthyPeople, InfectedPeople, DeadPeople);
        }
        else
        {
            ((BI_HouseData)block).UpdateData(HealthyPeople, InfectedPeople, DeadPeople);
        }

        block = BuildingInfo.singleton.buildingInfoBlocks[1];
        ((BI_Text)block).Init($"Healthy: {HealthyPeople}\nInfected: {InfectedPeople}\nDead: {DeadPeople}");

        block = BuildingInfo.singleton.buildingInfoBlocks[2];
        ((BI_Text)block).Init($"Discontent: {Discontent}%");

    }

    public override bool IsHaveBuildingInfo()
    {
        return true;
    }

    public override string GetBuildingName()
    {
        return "House";
    }

    public override Sprite GetBuildingImage()
    {
        return base.GetBuildingImage();
    }

    public override EInfectedBuildingStatus GetEInfectedBuildingStatus()
    {
        if (IsEmptyHouse)
            return EInfectedBuildingStatus.Empty;
        if (InfectedPeople > 0)
            return EInfectedBuildingStatus.Infected;
        return EInfectedBuildingStatus.NotInfected;
    }

    public virtual void UpdateFeedDisplay(EFeedState state)
    {
        if (isDestroyed)
            return;

        foreach (var e in buildingData.poses)
        {
            ResourceManager.SetFeedTextureState(e.x, e.y, state);
        }
    }

    public void ChangeFeedState() {
        if (FeedState == EFeedState.NotFeed)
        {
            FeedState = EFeedState.Feed;
        }
        else
        {
            FeedState = EFeedState.NotFeed;
        }
        UpdateFeedState();
    }

    public void UpdateFeedState()
    {
        if (IsEmptyHouse)
        {
            UpdateFeedDisplay(EFeedState.Dead);
        }
        else
        {
            UpdateFeedDisplay(FeedState);
        }
    }

    void UpdatedInfectionState()
    {
        if (HealthyPeople <= 0 && InfectedPeople <= 0)
        {
            IsEmptyHouse = true;
            SetDiscontent(0);
        }

        if (IsEmptyHouse)
        {
            UpdateInfectionDisplay(CellInfectionState.Dead);
            UpdateDiscontentDisplay();
            UpdateFeedState();
        }
        else {
            if (InfectedPeople > 0)
            {
                UpdateInfectionDisplay(CellInfectionState.Infected);
            }
            else {
                UpdateInfectionDisplay(CellInfectionState.NotInfected);

                InfectionManager.singleton.RemoveInfectedHouse(this);
            }
        }
        TryUpdateBuildingInfo();
    }
}
