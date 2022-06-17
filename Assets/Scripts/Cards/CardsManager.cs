using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CardsManager : MonoBehaviour
{
    public bool IsCardInHand;
    public GameCard SelectedCard;

    public Dictionary<CardClassType, CardsPack> CardsPacks;
    public List<CardsPack> AllCardsPacks = new List<CardsPack>();

    private Transform CardPrefab;

    public Dictionary<CardClassType, int> CardsCountNow;
    public Dictionary<CardClassType, float> CardsPacksWidth;

    public int CardsInHandCount = 0;
    public int TotalCardsCount = 0;
    int totalCardsCountPrevious = 0;
    public int PacksOffset = 20;

    public Material BuildingPrefabCanBePlaced;
    public Material BuildingPrefabCantBePlaced;

    public static int MaxCardsCount = 10;
    [SerializeField] CardsLimitDisplay cardsLimitDisplay;

    private float moveSpeed;

    private Selector selector;
    private UnitManager unitManager;
    private GameObject buildingPreview;
    private int buildingRotation;

    private Dictionary<string, CardData> cardsData;

    private Dictionary<string, BuildingCard> cardsBuildings;
    private Dictionary<CardData, BuildingCard> cardDataToBuilding;

    private Dictionary<string, UnitCard> cardsUnits;
    private Dictionary<CardData, UnitCard> cardDataToUnits;

    private Dictionary<string, BuyCardData> buyCardsData;
    private Dictionary<BuyCardData, CardData> buyCardDataToCardData;

    private Dictionary<string, int> settedBuildings;
    private Dictionary<string, List<PlacedBuilding>> buildings;

    public static CardsManager singleton;

    private void Awake()
    {
        singleton = this;
        CardsPacks = new Dictionary<CardClassType, CardsPack>();
        CardsCountNow = new Dictionary<CardClassType, int>();
        CardsPacksWidth = new Dictionary<CardClassType, float>();
        settedBuildings = new Dictionary<string, int>();
        buildings = new Dictionary<string, List<PlacedBuilding>>();

        moveSpeed = CardsPack.ResizeToTargetSpeed * 0.5f;

        foreach (var e in AllCardsPacks) {
            CardsPacks[e.PackType] = e;
            CardsCountNow[e.PackType] = e.Cards.Count;
            CardsPacksWidth[e.PackType] = e.TargetSize;
        }
    }

    private void Start()
    {
        CardPrefab = PrefabManager.GetPrefab("Cards_Card");
        LoadCards();
        LoadBuyCardData();

        selector = Selector.singleton;
        unitManager = UnitManager.singleton;
        BuildingsShop.singleton.Init();
    }

    public void LoadBuyCardData()
    {
        buyCardsData = new Dictionary<string, BuyCardData>();
        buyCardDataToCardData = new Dictionary<BuyCardData, CardData>();

        foreach (var data in Resources.LoadAll<BuyCardData>("Cards/BuyCardsData"))
        {
            buyCardsData[data.CardName] = data;
            buyCardDataToCardData[data] = GetCardData(data.CardName);
            Debug.Log($"Loaded buy card data: {data.CardName}");
        }
    }

    public void GiveSomeCards(List<string> cardsNames, bool isFastGive) {
        if (isFastGive)
        {
            foreach (var e in cardsNames)
            {
                SpawnGameCard(e);
                RecalculateTotalCardsCount();
            }
        }
        else
        {
            StartCoroutine(EnumeratorGiveSomeCardsTest(cardsNames));
        }
    }

    private IEnumerator EnumeratorGiveSomeCardsTest(List<string> cardsNames)
    {
        foreach (var e in cardsNames)
        {
            SpawnGameCard(e);
            RecalculateTotalCardsCount();
            yield return new WaitForSeconds(0.1f);
        }
    }

    void LoadCards() {
        cardsData = new Dictionary<string, CardData>();
        foreach (var e in Resources.LoadAll<CardData>("Cards/CardsData")) {
            if (cardsData.ContainsKey(e.CardName))
            {
                Debug.LogError($"Card data with name \"{e.CardName}\" already loaded (error prefab name = \"{e.name}\")");
            }
            else
            {
                cardsData[e.CardName] = e;
                Debug.Log($"Loaded card data: {e.CardName}");
            }
        }

        cardsBuildings = new Dictionary<string, BuildingCard>();
        cardDataToBuilding = new Dictionary<CardData, BuildingCard>();
        foreach (var e in Resources.LoadAll<BuildingCard>("Cards/CardsBuildings"))
        {
            if (cardsBuildings.ContainsKey(e.CardName))
            {
                Debug.LogError($"Card building with name \"{e.CardName}\" already loaded (error prefab name = \"{e.name}\")");
            }
            else
            {
                if (e.LoadPrefab() && cardsData.ContainsKey(e.CardName))
                {
                    cardsBuildings[e.CardName] = e;
                    cardDataToBuilding[cardsData[e.CardName]] = e;
                    Debug.Log($"Loaded card building: {e.CardName}");
                }
                else {
                    if (!cardsData.ContainsKey(e.CardName))
                        Debug.LogError($"Not found card data to card building: {e.CardName}");
                    Debug.LogError($"Not loaded card building: {e.CardName}");
                }
            }
        }

        cardsUnits = new Dictionary<string, UnitCard>();
        cardDataToUnits = new Dictionary<CardData, UnitCard>();
        foreach (var e in Resources.LoadAll<UnitCard>("Cards/CardsUnits"))
        {
            if (cardsUnits.ContainsKey(e.CardName))
            {
                Debug.LogError($"Card unit with name \"{e.CardName}\" already loaded (error prefab name = \"{e.name}\")");
            }
            else
            {
                if (e.LoadPrefab() && cardsData.ContainsKey(e.CardName))
                {
                    cardsUnits[e.CardName] = e;
                    cardDataToUnits[cardsData[e.CardName]] = e;
                    Debug.Log($"Loaded card unit: {e.CardName}");
                }
                else
                {
                    if (!cardsData.ContainsKey(e.CardName))
                        Debug.LogError($"Not found card data to card unit: {e.CardName}");
                    Debug.LogError($"Not loaded card unit: {e.CardName}");
                }
            }
        }
    }

    public void UpdateSelectedCard()
    {
        if (SelectedCard == null)
            return;
        if (SelectedCard.CardClassType != CardClassType.Building) {
            ProcessUnitCard();
        }
        return;
    }

    public void UseCard() {
        if (SelectedCard == null)
            return;

        if (CameraController.singleton.IsMouseOnGUI)
            return;

        if (SelectedCard.CardClassType == CardClassType.Building)
        {
            if (TryUseBuildingCard(selector.SelectedIntCoords, cardDataToBuilding[SelectedCard.CardData]))
            {
                if (!settedBuildings.ContainsKey(SelectedCard.CardData.CardName))
                    settedBuildings[SelectedCard.CardData.CardName] = 0;
                settedBuildings[SelectedCard.CardData.CardName] += 1;
                BuildingsShop.singleton.CheckAllIsUnlocked();
                DeleteCardFromHand(true);
                RecalculateTotalCardsCount(true);
            }
            return;
        }
        else {
            if (TryUseUnitCard())
            {
                DeleteCardFromHand();
                RecalculateTotalCardsCount(true);
            }
            return;
        }
    }

    public void CardSold(List<ResourceRequired> resourcesFromSold) {
        foreach (var e in resourcesFromSold) {
            ResourceManager.singleton.AddResourceCount(e.Resource, e.Count, true);
        }
        DeleteCardFromHand(true);
    }

    public void DeleteCardFromHand(bool removeFromCount = false)
    {
        DestroyBuildingPreview();
        SelectedCard.cardsPack.IsHasEmptySpace = false;
        CardsPacks[SelectedCard.CardClassType].DeletedCardFromHand();
        var card = SelectedCard;
        SelectedCard = null;
        IsCardInHand = false;
        Selector.singleton.SetSelectorMode(ESelectorMode.DirectHit);
        UnitManager.singleton.Unselected();
        Destroy(card.gameObject);

        if (removeFromCount)
        {
            TotalCardsCount -= 1;
            RecalculateTotalCardsCount(true);
        }
    }

    public GameCard SpawnGameCard(string cardDataName) {
        return SpawnGameCard(GetCardData(cardDataName));
    }

    public GameCard SpawnGameCard(CardData cardData) {
        var gameCard = Instantiate(CardPrefab, Vector2.zero, Quaternion.identity).GetComponent<GameCard>();
        gameCard.SetCardData(cardData);

        return gameCard;
    }

    public CardData GetCardData(string cardName) {
        if (cardsData.ContainsKey(cardName))
            return cardsData[cardName];

        Debug.LogError($"Card data with name \"{cardName}\" not found");
        return cardsData["Error"];
    }

    public CardData GetCardData(BuyCardData buyCardData)
    {
        if (buyCardDataToCardData.ContainsKey(buyCardData))
            return buyCardDataToCardData[buyCardData];

        Debug.LogError($"Cant convert BuyCardData with name \"{buyCardData.CardName}\" to CardData");
        return buyCardDataToCardData[GetBuyCardData("Error")];
    }

    public BuildingCard GetBuildingCard(string cardName)
    {
        if (cardsBuildings.ContainsKey(cardName))
            return cardsBuildings[cardName];

        Debug.LogError($"Building Card with name \"{cardName}\" not found");
        return cardsBuildings["Error"];
    }

    public BuildingCard GetBuildingCard(CardData cardData)
    {
        if (cardDataToBuilding.ContainsKey(cardData))
            return cardDataToBuilding[cardData];

        Debug.LogError($"Cant convert CardData with name \"{cardData.CardName}\" to building card");
        return cardDataToBuilding[GetCardData("Error")];
    }

    public UnitCard GetUnitCard(string cardName)
    {
        if (cardsUnits.ContainsKey(cardName))
            return cardsUnits[cardName];

        Debug.LogError($"Unit Card with name \"{cardName}\" not found");
        return cardsUnits["Error"];
    }

    public UnitCard GetUnitCard(CardData cardData)
    {
        if (cardDataToUnits.ContainsKey(cardData))
            return cardDataToUnits[cardData];

        Debug.LogError($"Cant convert CardData with name \"{cardData.CardName}\" to Unit card");
        return cardDataToUnits[GetCardData("Error")];
    }

    public List<PlacedBuilding> GetAllBuildingsWithName(string name) {
        if (!buildings.ContainsKey(name))
            return new List<PlacedBuilding>();
        return buildings[name];
    }

    public BuyCardData GetBuyCardData(string cardName)
    {
        if (buyCardsData.ContainsKey(cardName))
            return buyCardsData[cardName];

        Debug.LogError($"Buy Card Data with name \"{cardName}\" not found");
        return buyCardsData["Error"];
    }

    public BuyCardData GetBuyCardDataOrNull(string cardName)
    {
        if (buyCardsData.ContainsKey(cardName))
            return buyCardsData[cardName];

        return null;
    }

    public bool HaveBuyCardData(string cardName) {
        return buyCardsData.ContainsKey(cardName);
    }

    public List<BuyCardData> GetAllBuyCardsOfType(CardClassType cardClassType) {
        var result = new List<BuyCardData>();
        foreach (var pair in buyCardDataToCardData) {
            if (pair.Value.CardClassType == cardClassType) {
                result.Add(pair.Key);
            }
        }
        return result;
    }

    public int GetSettedBuildingsCount(string cardName) {
        if (settedBuildings.ContainsKey(cardName))
            return settedBuildings[cardName];
        return 0;
    }

    public void AddCardToDeck(GameCard card)
    {
        CardsPacks[card.CardClassType].PutCardBack(card);
        card.transform.localPosition = Vector2.zero;
        card.cardsPack.SetIsHidden(false);
        TotalCardsCount += 1;
    }

    public void RecalculateCardsCount(CardClassType cardClassType, int count) {
        CardsCountNow[cardClassType] = count;

        CardsInHandCount = 0;
        foreach (var e in CardsCountNow) {
            CardsInHandCount += e.Value;
        }
        RecalculateTotalCardsCount();
    }

    public void RecalculateTotalCardsCount(bool isForceUpdate = false) { 
        cardsLimitDisplay.SetCardsCount(TotalCardsCount, MaxCardsCount);
        if (totalCardsCountPrevious != TotalCardsCount || isForceUpdate)
        {
            totalCardsCountPrevious = TotalCardsCount;
            CardsPack.CardOffset = 90 - TotalCardsCount * 1.5f;
            RecalculateAllCardsPackWidth();
            foreach (var cardPack in CardsPacks) {
                cardPack.Value.CheckIsHidden();
            }
        }
    }

    public bool IsLessThanMaxCardsCount() {
        return TotalCardsCount < MaxCardsCount;
    }

    public void RecalculateAllCardsPackWidth() {
        foreach (var e in AllCardsPacks) {
            e.RecalculateCardPosition();
        }
    }

    public void RecalculateCardsPackWidth(CardClassType cardClassType, float width)
    {
        CardsPacksWidth[cardClassType] = width;
        CardsPacksWidthUpdated();
    }

    public void CardsPacksWidthUpdated(float moveSpeedMultiplier = 1) {
        if (!BuildingsShop.singleton.IsOpened)
        {
            foreach (var pack in AllCardsPacks)
            {
                if (!pack.IsHidden)
                {
                    pack.gameObject.SetActive(true);
                }
            }

            var nextPos = 0f;
            foreach (var width in CardsPacksWidth)
            {
                if (width.Value == 0)
                    continue;
                nextPos += width.Value;
                nextPos += PacksOffset;
            }
            nextPos *= -0.5f;

            foreach (var pack in AllCardsPacks)
            {
                if (CardsPacksWidth[pack.PackType] == 0)
                    continue;
                nextPos += CardsPacksWidth[pack.PackType] * 0.5f;
                nextPos += PacksOffset * 0.5f;
                pack.SetPackTargetMove(pack, new Vector2(nextPos, 10), moveSpeed * moveSpeedMultiplier);

                nextPos += CardsPacksWidth[pack.PackType] * 0.5f;
                nextPos += PacksOffset * 0.5f;
            }

            cardsLimitDisplay.SetPackTargetMove(new Vector2(nextPos + 60, 10), moveSpeed * moveSpeedMultiplier);
        }
        else
        {
            foreach (var pack in AllCardsPacks)
            {
                pack.gameObject.SetActive(false);
            }
            var buildingsPack = CardsPacks[CardClassType.Building];
            buildingsPack.SetIsHidden(false, false);
            buildingsPack.Close();
            buildingsPack.SetPackTargetMove(buildingsPack, BuildingsShop.singleton.BuildingShopTarget.anchoredPosition, 3000);
            cardsLimitDisplay.SetPackTargetMove(BuildingsShop.singleton.BuildingShopTarget.anchoredPosition + new Vector2(150, 0), 3000);
        }
    }

    public bool TakedCard(GameCard card, int takedWay, bool isTakedFromPack = true)
    {
        if (IsCardInHand)
            return false;

        SelectedCard = card;
        IsCardInHand = true;
        card.cardsPack.TakedCard(card, isTakedFromPack);
        SelectedCard.transform.SetParent(UIManager.singleton.CursorContainer);
        SelectedCard.transform.localPosition = new Vector3(0, 0, 0);
        Selector.singleton.SetSelectorMode(SelectedCard.CardData.SelectorModeWhenTaked);

        if (card.CardClassType == CardClassType.Building) {
            SpawnBuildingPreview(cardDataToBuilding[card.CardData]);
        }

        return true;
    }

    public void PutCardBackFromHand()
    {
        if (!IsCardInHand)
            return;

        unitManager.RemoveWay();
        unitManager.IsUnitSelected = false;

        DestroyBuildingPreview();
        SelectedCard.cardsPack.PutCardBack(SelectedCard);
        SelectedCard = null;
        IsCardInHand = false;
        Selector.singleton.SetSelectorMode(ESelectorMode.DirectHit);
    }

    void SpawnBuildingPreview(BuildingCard buildingCard) {
        DestroyBuildingPreview();

        var buildingPreviewObject = Instantiate(buildingCard.prefab_preview, selector.Container);
        buildingPreview = buildingPreviewObject.gameObject;
        buildingPreviewObject.transform.localPosition = new Vector3(0.5f, 0, 0.5f);
        buildingPreviewObject.transform.localRotation = Quaternion.Euler(0, 0, 0);

        UpdateBuildingPreviewIsCanBePlaced();
    }

    void DestroyBuildingPreview() {
        if (buildingPreview != null)
            Destroy(buildingPreview);
    }

    public void SetBuildingRotation(int rotationValue) {
        buildingRotation = rotationValue;
        Selector.singleton.Container.localRotation = Quaternion.Euler(0, 90 * buildingRotation, 0);
    }

    bool TryUseBuildingCard(Vector3Int basePos, BuildingCard buildingCard)
    {
        if (IsBuildingCanBePlacedAt(basePos, buildingCard, GetBuyCardDataOrNull(buildingCard.CardName), out var wrongPoses, out var placedBuildingsToDestroy))
        {
            var poses = new List<Vector2Int>();
            foreach (var offset in GetRotatedOffsets(buildingCard.BuildingPositions))
            {
                poses.Add(new Vector2Int(basePos.x + offset.x, basePos.z + offset.y));

                //Debug placing object

                //Instantiate(PrefabManager.GetPrefab("DebugObject"), 
                //    new Vector3(
                //        basePos.x + offset.x, 
                //        GeneratorSpawner.singleton.GetHeigthAt(new Vector2Int(basePos.x + offset.x, basePos.z + offset.y)), 
                //        basePos.z + offset.y),
                //    transform.rotation
                //    );
            }
            if (buildingCard.IsConnectingToRoad && !TownManager.singleton.AddBuildingRoad(poses))
                return false;

            DestroyBuildingPreview();
            foreach (var placedBuilding in placedBuildingsToDestroy) {
                if (placedBuilding != null && placedBuilding.gameObject != null) {
                    TownManager.singleton.DestroyBuilding(placedBuilding);
                }
            }

            var building = GeneratorSpawner.singleton.SpawnObject(
                buildingCard.prefab_placedBuilding.gameObject,
                basePos,
                Selector.singleton.Container.rotation, 
                null, 
                true).GetComponent<PlacedBuilding>();

            building.SetBuildingData(new BuildingData(poses.ToArray(), 1));

            if (!buildings.ContainsKey(SelectedCard.CardData.CardName))
                buildings[SelectedCard.CardData.CardName] = new List<PlacedBuilding>();
            buildings[SelectedCard.CardData.CardName].Add(building);

            building.IsUnitCanWalkThrough = buildingCard.IsUnitsCanWalkThrough;

            building.SetBuildingImage(buildingCard.BuildingInfoSprite);
            TownManager.singleton.RegisterBuilding(building, EBuilding.CustomBuilding);
            return true;
        }
        return false;
    }

    bool IsBuildingCanBePlacedAt(
        Vector3Int pos, 
        BuildingCard buildingCard,
        BuyCardData buyCardData,
        out List<Vector3Int> wrongPoses, 
        out List<PlacedBuilding> buildingsToDestroy) 
    {
        wrongPoses = new List<Vector3Int>();
        buildingsToDestroy = new List<PlacedBuilding>();
        if (buyCardData != null)
        {
            var result = true;
            foreach (var e in buyCardData.BuildingsRequired)
            {
                switch (e.CountComparison)
                {
                    case CountComparison.EqualTo:
                        if (GetSettedBuildingsCount(e.CardName) != e.Count)
                            result = false;
                        break;
                    case CountComparison.MoreOrEqualTo:
                        if (GetSettedBuildingsCount(e.CardName) < e.Count)
                            result = false;
                        break;
                    case CountComparison.NoMoreThan:
                        if (GetSettedBuildingsCount(e.CardName) > e.Count)
                            result = false;
                        break;
                }
            }
            if (!result)
                return false;
        }

        foreach (var offset in GetRotatedOffsets(buildingCard.BuildingPositions)) {
            var newPos = new Vector2Int(pos.x + offset.x, pos.z + offset.y);
            var height = GeneratorSpawner.singleton.GetHeigthAt(newPos);

            if (!buildingCard.IsCanBePlacedOnEarth) { 
                if(GeneratorSpawner.singleton.GetFormAt(newPos))
                    wrongPoses.Add(new Vector3Int(newPos.x, height, newPos.y));
            }

            if (!buildingCard.IsCanBePlacedOnWater)
            {
                if (!GeneratorSpawner.singleton.GetFormAt(newPos))
                    wrongPoses.Add(new Vector3Int(newPos.x, height, newPos.y));
            }

            if (buildingCard.IsCanBePlacedOnlyOnRoad)
            {
                if (!TownManager.singleton.Roads.ContainsKey(newPos) || TownManager.singleton.Roads[newPos] == RoadType.None)
                    wrongPoses.Add(new Vector3Int(newPos.x, height, newPos.y));
            }

            if (height != pos.y)
                wrongPoses.Add(new Vector3Int(newPos.x, height, newPos.y));
            else {
                var buildingTypeAt = TownManager.singleton.GetBuildingTypeAt(newPos);
                if (buildingTypeAt != EBuilding.None && !buildingCard.CanReplaceBuildingsTypes.Contains(buildingTypeAt) || 
                    (buildingTypeAt == EBuilding.House && ((B_House)TownManager.singleton.GetBuildingAt(newPos)).InfectedPeople > 0))
                {
                    wrongPoses.Add(new Vector3Int(newPos.x, height, newPos.y));
                }
                else
                {
                    if (buildingTypeAt != EBuilding.None)
                    {
                        buildingsToDestroy.Add(TownManager.singleton.GetBuildingAt(newPos));
                    }
                }
            }
        }
        return wrongPoses.Count == 0;
    }

    void ProcessUnitCard() {
        unitManager.UpdatedUnitCardInHand(SelectedCard);
    }

    bool TryUseUnitCard()
    {
        if (Selector.singleton.SelectedObject != null && Selector.singleton.SelectedObject.IsCanBeVisitedByUnits())
        {
            if (Selector.singleton.SelectedType == SelectedType.Building ||
                Selector.singleton.SelectedType == SelectedType.Road)
            {
                if (UnitManager.singleton.Way == null || UnitManager.singleton.Way.Count == 0)
                    return false;

                var unitCard = GetUnitCard(SelectedCard.CardData);
                var unitBase = unitManager.SpawnUnit(unitCard);

                var way = unitManager.FindWay(unitManager.startPoints, unitManager.targets);

                unitBase.Init(unitCard, way, unitManager.Target, unitManager.TargetType);

                unitManager.SelectedUnit = null;
                unitManager.IsUnitSelected = false;
                unitManager.targets = new HashSet<Vector2Int>();
                unitManager.startPoints = new HashSet<Vector2Int>();
                unitManager.RemoveWay();

                return true;
            }
        }
        else {
            return false;
        }
        return false;
    }

    public void UpdateBuildingPreviewIsCanBePlaced() {
        if (SelectedCard.CardClassType == CardClassType.Building)
        {
            if (buildingPreview == null)
                return;

            var isCanBePlaced = IsBuildingCanBePlacedAt(
                selector.SelectedIntCoords, 
                cardDataToBuilding[SelectedCard.CardData],
                GetBuyCardDataOrNull(SelectedCard.CardData.CardName),
                out var wrongPoses, 
                out var buildingsToDestroy);

            foreach (var e in buildingPreview.GetComponent<BuildingPreview>().MeshRenderers)
            {
                e.material = isCanBePlaced ? BuildingPrefabCanBePlaced : BuildingPrefabCantBePlaced;
            }

            return;
        }
    }

    IEnumerable<Vector2Int> GetRotatedOffsets(IEnumerable<Vector2Int> offsets) {
        foreach (var e in offsets) {
            switch (buildingRotation) {
                case 0:
                    yield return e;
                    break;
                case 1:
                    yield return new Vector2Int(e.y, -e.x);
                    break;
                case 2:
                    yield return new Vector2Int(-e.x, -e.y);
                    break;
                case 3:
                    yield return new Vector2Int(-e.y, e.x);
                    break;
            }
        }
    }
}
