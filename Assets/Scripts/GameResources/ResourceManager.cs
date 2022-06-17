using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour, ITickable
{
    private Dictionary<EResource, int> resourcesCounts;
    private Dictionary<EResource, int> resourcesDeltas;
    private Transform ResourceChangedPrefab;

    static Dictionary<EResource, GameResourceData> resourcesData;
    public static ResourceManager singleton;
    public static Texture2D FeedTexture;

    public FoodManager FoodManager;
    public GoldManager GoldManager;

    void Awake()
    {
        singleton = this;

        if (resourcesData == null)
        {
            resourcesData = new Dictionary<EResource, GameResourceData>();

            foreach (var rData in Resources.LoadAll<GameResourceData>("GameResources"))
            {
                resourcesData[rData.EResource] = rData;
            }
        }

        resourcesCounts = new Dictionary<EResource, int>();
        InitResourceDictionary(resourcesCounts);

        resourcesDeltas = new Dictionary<EResource, int>();
        InitResourceDictionary(resourcesDeltas);

        FeedTexture = new Texture2D(80, 80);
        FeedTexture.filterMode = FilterMode.Point;
        FeedTexture.wrapMode = TextureWrapMode.Clamp;
        for (var x = 0; x < 80; x++)
        {
            for (var y = 0; y < 80; y++)
            {
                SetFeedTextureState(x, y, EFeedState.None);
            }
        }
    }

    void InitResourceDictionary(Dictionary<EResource, int> dict)
    {
        foreach (var resource in GetAllResourceType())
        {
            dict[resource] = 0;
        }
    }

    void Start()
    {
        Ticker.AddTickable(this, TickablePriority.Main);

        ResourceChangedPrefab = PrefabManager.GetPrefab("UI_ResourceChanged");
    }

    public void SetResourceCount(EResource resource, int value)
    {
        resourcesCounts[resource] = value;
        ResourceBlock.ResourceBlocks[resource].UpdateValues();
        BuildingsShop.singleton.CheckAllIsUnlocked();
        BuildingInfo.singleton.CheckAllIsUnlocked();

        TaskManager.UpdatedETask(ETask.ResourceCount);
    }

    public int GetResourceCount(EResource resource) => resourcesCounts[resource];
    public void AddResourceCount(EResource resource, int delta, bool spawnResourceChanged = false)
    {
        resourcesCounts[resource] += delta;
        if (resourcesCounts[resource] < 0)
            resourcesCounts[resource] = 0;
        ResourceBlock.ResourceBlocks[resource].UpdateValues();
        BuildingsShop.singleton.CheckAllIsUnlocked();
        BuildingInfo.singleton.CheckAllIsUnlocked();

        if (spawnResourceChanged)
        {
            var rc = Instantiate(ResourceChangedPrefab, ResourceBlock.ResourceBlocks[resource].transform.parent);
            rc.GetComponent<ResourceChanged>().SetData(
                ResourceBlock.ResourceBlocks[resource].ResourceIcon.sprite,
                ResourceBlock.ResourceBlocks[resource].ResourceIcon.color,
                ResourceBlock.ResourceBlocks[resource].ResourceIconOutline.sprite,
                ResourceBlock.ResourceBlocks[resource].ResourceIconOutline.color,
                delta);
            rc.transform.position = ResourceBlock.ResourceBlocks[resource].transform.position;
        }

        TaskManager.UpdatedETask(ETask.ResourceCount);
    }

    public bool IsHaveEnoughResources(EResource resource, int count, out int delta) {
        delta = GetResourceCount(resource) - count;
        return delta >= 0;
    }

    public bool TrySubstractResource(EResource resource, int count, out int delta, bool spawnResourceChange = false) {
        if (IsHaveEnoughResources(resource, count, out delta)) {
            AddResourceCount(resource, -count, spawnResourceChange);
            return true;
        }
        return false;
    }

    public void SetResourceDelta(EResource resource, int value) => resourcesDeltas[resource] = value;
    public int GetResourceDelta(EResource resource) => resourcesDeltas[resource];
    public void AddResourceDelta(EResource resource, int delta) => resourcesDeltas[resource] += delta;

    public void Tick(int luck)
    {
        foreach (var resource in GetAllResourceType())
        {
            AddResourceCount(resource, GetResourceDelta(resource));
        }

        if (Ticker.singleton.Hour == 6 || Ticker.singleton.Hour == 18)
        {
            FoodManager.TickFood();
        }

        if (Ticker.singleton.Hour == 6) {
            GoldManager.TickGold();
        }
    }


    public IEnumerable<EResource> GetAllResourceType() {
        foreach (var e in resourcesData) {
            yield return e.Key;
        }
    }

    public static string GetResourceName(EResource resource) {
        if (resourcesData == null) {
            resourcesData = new Dictionary<EResource, GameResourceData>();

            foreach (var rData in Resources.LoadAll<GameResourceData>("GameResources"))
            {
                resourcesData[rData.EResource] = rData;
            }
        }

        if (resourcesData.ContainsKey(resource))
            return resourcesData[resource].ResourceName;
        Debug.LogError($"No resource name at {resource}");
        return "Resource_Error";
    }

    public static Sprite GetResourceSprite(EResource resource)
    {
        if (resourcesData.ContainsKey(resource))
            return resourcesData[resource].Sprite;
        Debug.LogError($"No resource sprite at {resource}");
        return null;
    }

    public static Sprite GetResourceSpriteOutline(EResource resource)
    {
        if (resourcesData.ContainsKey(resource))
            return resourcesData[resource].SpriteOutline;
        Debug.LogError($"No resource sprite at {resource}");
        return null;
    }

    public static void SetFeedTextureState(int x, int y, EFeedState state)
    {
        var color = new Color(0.8f, 0.8f, 0.8f);
        switch (state)
        {
            case EFeedState.None:
                color = new Color(0.7f, 0.7f, 0.7f);
                break;
            case EFeedState.Dead:
                color = Color.black;
                break;
            case EFeedState.NotFeed:
                color = new Color(0.5f, 0.46f, 0);
                break;
            case EFeedState.Feed:
                color = Color.yellow;
                break;
        }
        FeedTexture.SetPixel(x, y, color);
        FeedTexture.Apply();
    }
}
