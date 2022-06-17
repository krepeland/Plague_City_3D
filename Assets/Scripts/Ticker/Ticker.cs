using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Ticker : MonoBehaviour, IPauseable
{
    public static Ticker singleton = null;
    public static Dictionary<TickablePriority, HashSet<ITickable>> Tickables;
    public static HashSet<(ITickable, GameObject)> TickablesToDelete;

    public static bool IsPaused { get; private set; }
    public static bool IsControllPaused { get; private set; }

    public List<Button> SpeedButtons = new List<Button>();
    private List<bool> speedButtonsLockStates = new List<bool>() { false, false, false, false};
    public List<float> SecondsPerTickNormal = new List<float>() { 4.5f, 2.25f, 1f };
    public float TickPercent = 0;
    public int SpeedNow = 0;

    public float SecondsPerTick = 4.5f;
    public float TickTime;

    public long Ticks = 0;
    public int Day = 0;
    public int Hour = 0;

    [SerializeField] private int pausingCount;
    [SerializeField] private int controllPausingCount;
    public HashSet<IPauseable> Pauseables = new HashSet<IPauseable>();

    void Awake()
    {
        IsPaused = false;
        IsControllPaused = false;
        singleton = this;
        IsPaused = false;
        Tickables = new Dictionary<TickablePriority, HashSet<ITickable>>();

        foreach (var priority in GetTickablePriorities())
            Tickables[priority] = new HashSet<ITickable>();

        TickablesToDelete = new HashSet<(ITickable, GameObject)>();

        AddTickable(BuildingInfo.singleton, TickablePriority.Low);
    }

    void Start() {
        SetSpeedIndex(SpeedNow);
    }

    public void SetSpeedIndex(int index) {
        if (index == 3)
        {
            if (SpeedNow != 3)
                AddPausingCount(1, false);
            SpeedNow = index;
            SwitchSelectedSpeedButton();
            return;
        }
        else {
            if (SpeedNow == 3)
                AddPausingCount(-1, false);
        }
        var oldSpeedValue = SecondsPerTick;
        SpeedNow = index;
        SecondsPerTick = SecondsPerTickNormal[index];

        if (oldSpeedValue != 0)
        {
            var c = SecondsPerTick / oldSpeedValue;
            TickTime *= c;
        }
        TickPercent = TickTime / SecondsPerTick;

        SwitchSelectedSpeedButton();
    }

    void SwitchSelectedSpeedButton() {
        for (var i = 0; i < SpeedButtons.Count; i++) {
            SpeedButtons[i].interactable = (!speedButtonsLockStates[i] && i != SpeedNow);
        }
    }

    public void SetSpeedButtonLock(bool isLocked, int index) {
        speedButtonsLockStates[index] = isLocked;
        SwitchSelectedSpeedButton();
    }

    void Update()
    {
        if (IsPaused)
        {
            return;
        }

        TickTime += Time.deltaTime;
        if (TickTime > SecondsPerTick)
        {
            TickTime -= SecondsPerTick;
            Tick();
        }
        TickPercent = TickTime / SecondsPerTick;
    }

    public static bool AddTickable(ITickable tickable, TickablePriority priority)
    {
        if (Tickables.Any(x => x.Value.Contains(tickable)))
            return false;

        Tickables[priority].Add(tickable);
        return true;
    }

    public static bool TryDeleteTickable(ITickable tickable, GameObject objectToDestroy = null)
    {
        if (!Tickables.Any(x => x.Value.Contains(tickable)))
            return false;
        
        TickablesToDelete.Add((tickable, objectToDestroy));
        return true;
    }

    void ProcessDeleteTickables()
    {
        foreach (var e in TickablesToDelete)
        {
            if (e.Item1 != null)
            {
                foreach (var priority in GetTickablePriorities()) { 
                    if(Tickables[priority].Contains(e.Item1))
                        Tickables[priority].Remove(e.Item1);
                }
            }

            if (e.Item2 != null)
                Destroy(e.Item2);
        }
        TickablesToDelete = new HashSet<(ITickable, GameObject)>();
    }

    public void Tick()
    {
        ProcessDeleteTickables();
        Ticks++;
        if (++Hour >= 24)
        {
            Hour = 0;
            Day++;
            TaskManager.UpdatedETask(ETask.Days);
        }

        foreach (var priority in GetTickablePriorities())
        {
            foreach (var tickable in Tickables[priority])
            {
                if (tickable != null)
                {
                    //Debug.Log($"Ticked ({priority}) {tickable}");
                    tickable.Tick(0);
                }
            }
        }
    }

    public void SetPause(bool isPaused)
    {
        Debug.Log("Game pause: " + isPaused);
        IsPaused = isPaused;
        List<IPauseable> pauseablesToDelete = new List<IPauseable>();
        foreach (var e in Pauseables)
        {
            if (e != null)
            {
                e.SetPause(isPaused);
            }
            else {
                pauseablesToDelete.Add(e);
            }
        }
        while (pauseablesToDelete.Count > 0) {
            DeletePauseable(pauseablesToDelete[0]);
            pauseablesToDelete.RemoveAt(0);
        }
    }

    IEnumerable<TickablePriority> GetTickablePriorities() {
        yield return TickablePriority.Main;
        yield return TickablePriority.High;
        yield return TickablePriority.Medium;
        yield return TickablePriority.Low;
    }

    public void AddPausingCount(int count, bool isControllPausing)
    {
        pausingCount += count;
        
        if (isControllPausing)
        {
            controllPausingCount += count;
            IsControllPaused = controllPausingCount > 0;
        }

        if (pausingCount > 0)
        {
            SetPause(true);
        }
        else { 
            pausingCount = 0;
            SetPause(false);
        }
    }

    public void AddPausable(IPauseable pauseable)
    {
        Pauseables.Add(pauseable);
        pauseable.SetPause(IsPaused);
    }

    public bool DeletePauseable(IPauseable pauseable)
    {
        if (Pauseables.Contains(pauseable))
        {
            Pauseables.Remove(pauseable);
            return true;
        }
        return false;
    }
}
