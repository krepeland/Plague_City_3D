using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Infection_Settings infection_Settings;

    [HideInInspector] public GeneratorManager generatorManager;
    [HideInInspector] public InfectionManager infectionManager;
    [HideInInspector] public DiscontentManager discontentManager;

    public Ticker ticker;
    public CardsManager cardsManager;

    [HideInInspector] public ResourceManager resourceManager;
    [HideInInspector] public UnitManager unitManager;
    [HideInInspector] public TaskManager taskManager;
    public TasksDisplay tasksDisplay;
    public GameRoundEnd gameEnd;

    public List<GameObject> BuildingShopBlock;
    public List<GameObject> SellCardsBlock;
    public List<GameObject> TopBlock;

    public List<GameObject> HandBlocks;

    public static GameManager singleton;
    public static LevelData LevelData;
    [SerializeField] private EGameStage gameStage;
    public static bool IsWin;

    public static void SetLevelData(LevelData levelData) {
        LevelData = levelData;
    }

    private void Awake()
    {
        gameStage = EGameStage.Loading;
        generatorManager = Instantiate(PrefabManager.GetPrefab("Managers_GeneratorManager")).GetComponent<GeneratorManager>();
        if (LevelData == null)
        {
            var townSize = LevelMapManager.GetTownSize(0);
            generatorManager.InitGenerators(townSize.Item1, townSize.Item2, ERiverType.DoubledRiver);
        }
        else
        {
            var townSize = LevelMapManager.GetTownSize(LevelData.Size);
            generatorManager.InitGenerators(townSize.Item1, townSize.Item2, ERiverType.DoubledRiver);
        }
        generatorManager.transform.SetParent(transform);

        resourceManager = Instantiate(PrefabManager.GetPrefab("Managers_ResourceManager")).GetComponent<ResourceManager>();
        resourceManager.transform.SetParent(transform);

        //ticker = Instantiate(PrefabManager.GetPrefab("Managers_Ticker")).GetComponent<Ticker>();

        infectionManager = Instantiate(PrefabManager.GetPrefab("Managers_InfectionManager")).GetComponent<InfectionManager>();
        infectionManager.LoadConfig(infection_Settings);
        infectionManager.transform.SetParent(transform);

        discontentManager = Instantiate(PrefabManager.GetPrefab("Managers_DiscontentManager")).GetComponent<DiscontentManager>();
        discontentManager.transform.SetParent(transform);

        unitManager = Instantiate(PrefabManager.GetPrefab("Managers_UnitManager")).GetComponent<UnitManager>();
        unitManager.transform.SetParent(transform);

        taskManager = Instantiate(PrefabManager.GetPrefab("Managers_TaskManager")).GetComponent<TaskManager>();
        taskManager.transform.SetParent(transform);
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        singleton = this;

        generatorManager.Generate();
        
        LoadLevelSettings();

        gameEnd.Init();

        infectionManager.Tick(0);
    }

    void LoadLevelSettings() {
        ticker.Hour = 6;
        ColorDayInterval.singleton.Tick(0);

        //cardsManager.GiveSomeCards(new List<string>() { "Town Hall", "Big Field", "Field", "Field" });
        cardsManager.GiveSomeCards(new List<string>() { "Town Hall" }, true);
        for (var i = 0; i < 4; i++)
            ticker.SetSpeedButtonLock(true, i);
        
        ticker.SetSpeedIndex(3);

        ChangeBlockEnabledState(BuildingShopBlock, false);
        ChangeBlockEnabledState(SellCardsBlock, false);
        ChangeBlockEnabledState(TopBlock, false);

        resourceManager.AddResourceCount(EResource.Food, 1000);
        resourceManager.AddResourceCount(EResource.Gold, 1000);
        resourceManager.AddResourceCount(EResource.Worker, 10);

        if (LevelData == null)
        {
            var MainTasks = new List<GameTask>() 
                {
                new Task_InfectedPercent(TaskCountComparison.LessThan, 0),
            };


            var ExtraTasks = new List<GameTask>() 
            {
                new Task_Days(TaskCountComparison.LessThan, 5),
                new Task_DeadPercent(TaskCountComparison.LessThan, 25),
            };
            taskManager.Init(MainTasks, ExtraTasks, tasksDisplay);
        }
        else {
            taskManager.Init(LevelData.MainTasks, LevelData.ExtraTasks, tasksDisplay);
        }
    }

    void ChangeBlockEnabledState(List<GameObject> gameObjects, bool isEnabled) {
        foreach (var e in gameObjects) {
            e.SetActive(isEnabled);
        }
    }

    private void Update()
    {
        switch (gameStage) {
            case EGameStage.Loading:
                if (cardsManager.CardsPacks[CardClassType.Building].Cards.Any(x => x.CardData.CardName == "Town Hall"))
                {
                    LoadNextStage();
                }
                return;
            case EGameStage.TownHallPlacing:
                if (cardsManager.CardsPacks[CardClassType.Building].Cards.Any(x => x.CardData.CardName == "Town Hall") ||
                    (cardsManager.SelectedCard != null && cardsManager.SelectedCard.CardData.CardName == "Town Hall"))
                {
                    return;
                }
                LoadNextStage();
                break;
        }
    }

    void LoadNextStage() {
        switch (gameStage) {
            case EGameStage.Loading:
                gameStage = EGameStage.TownHallPlacing;
                break;
            case EGameStage.TownHallPlacing:
                gameStage = EGameStage.Game;

                for (var i = 0; i < 4; i++)
                    ticker.SetSpeedButtonLock(false, i);

                ticker.SetSpeedIndex(0);

                ChangeBlockEnabledState(BuildingShopBlock, true);
                ChangeBlockEnabledState(SellCardsBlock, true);
                ChangeBlockEnabledState(TopBlock, true);
                break;
            case EGameStage.Game:
                gameStage = EGameStage.End;
                break;
        }
    }

    public void GameEnded(bool isWin) {
        if (gameStage != EGameStage.Game)
            return;
        IsWin = isWin;
        LoadNextStage();

        for (var i = 0; i < 4; i++)
            ticker.SetSpeedButtonLock(true, i);

        ticker.SetSpeedIndex(3);

        BuildingInfo.singleton.Close();
        ChangeBlockEnabledState(BuildingShopBlock, false);
        ChangeBlockEnabledState(SellCardsBlock, false);
        ChangeBlockEnabledState(TopBlock, false);
        ChangeBlockEnabledState(HandBlocks, false);

        Ticker.singleton.AddPausingCount(1, true);
        gameEnd.GameEnded(isWin);
    }

    enum EGameStage
    {
        Loading,
        TownHallPlacing,
        Game,
        End
    }
}
