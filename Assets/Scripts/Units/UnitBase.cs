using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour, ITickable, ISelectable, IPauseable
{
    public SelectedType TargetType;
    public ISelectable Target;

    public UnitTeam UnitTeam;

    [HideInInspector] public CardData UnitCardData;
    [HideInInspector] public UnitCard UnitCard;

    [HideInInspector] public bool IsMoving;
    [HideInInspector] public Vector3 nextPoint = Vector3.zero;
    [HideInInspector] public Vector3 startPoint = Vector3.zero;
    private List<Vector3> way;

    private float Speed = 5;
    private float staminaMax;
    private float staminaNow;

    private float value;
    public UnitMark Mark;

    public Transform MarkPoser;
    bool isPaused;

    public bool IsInFight;
    [HideInInspector] public float FightTimeTillAttack;
    [HideInInspector] public float TimePerAttack;
    [HideInInspector] public int DodgePercent;
    [HideInInspector] public int Damage;
    [HideInInspector] public int Health;
    [HideInInspector] public int MaxHealth;
    [HideInInspector] bool isUnitMarkWasEnabled;
    [HideInInspector] public bool IsDied;

    public void Init(UnitCard unitCard, List<Vector2Int> coordsWay, ISelectable target, SelectedType targetType) {
        Ticker.singleton.AddPausable(this);
        if (coordsWay == null || coordsWay.Count == 0)
        {
            return;
        }

        UnitCard = unitCard;
        UnitCardData = CardsManager.singleton.GetCardData(UnitCard.CardName);
        Speed = UnitCard.Speed;

        UnitTeam = unitCard.UnitTeam;
        TimePerAttack = 1f / unitCard.AttacksPerHour;
        DodgePercent = unitCard.DodgePercent;
        Damage = unitCard.Damage;
        MaxHealth = unitCard.Health;
        Health = MaxHealth;

        SpawnUnitMark();

        var newWay = new List<Vector3>();
        foreach (var e in coordsWay)
            newWay.Add(new Vector3(e.x, GeneratorSpawner.singleton.GetHeigthAt(new Vector2Int(e.x, e.y)), e.y));

        SetWay(newWay, target, targetType);

        transform.position = newWay[0];
        nextPoint = newWay[0];
        startPoint = newWay[0];
        IsMoving = true;

        staminaMax = UnitCard.StaminaMax;
        staminaNow = staminaMax;

        UnitManager.singleton.UnitMoved(this);

        Inited();
    }

    public void InitEnemy(UnitCard unitCard, Vector2Int pos) {
        Ticker.singleton.AddPausable(this);

        UnitCard = unitCard;
        UnitCardData = CardsManager.singleton.GetCardData(UnitCard.CardName);
        Speed = UnitCard.Speed;

        UnitTeam = unitCard.UnitTeam;
        TimePerAttack = 1f / unitCard.AttacksPerHour;
        DodgePercent = unitCard.DodgePercent;
        Damage = unitCard.Damage;
        MaxHealth = unitCard.Health;
        Health = MaxHealth;

        SpawnUnitMark();

        var newPos = new Vector3(pos.x, GeneratorSpawner.singleton.GetHeigthAt(new Vector2Int(pos.x, pos.y)), pos.y);

        transform.position = newPos;
        nextPoint = newPos;
        startPoint = newPos;
        IsMoving = false;

        staminaMax = UnitCard.StaminaMax;
        staminaNow = staminaMax;

        UnitManager.singleton.UnitMoved(this);

        Inited();
    }

    public virtual void Inited() { 
    
    }

    public Vector2Int GetPosNow() {
        return new Vector2Int(Mathf.RoundToInt(startPoint.x), Mathf.RoundToInt(startPoint.z));
    }

    public void SetIsUnitInFight(bool isInFight) {
        if (IsDied)
            return;
        IsInFight = isInFight;
        Mark.SetIsInteractable(!IsInFight);

        if (isInFight)
        {
            isUnitMarkWasEnabled = Mark.IsActionObjectEnabled();
            Mark.SetActionEnabled(false);
        }
        else
        {
            Mark.SetActionEnabled(isUnitMarkWasEnabled);
        }
    }

    public void ResetUnitFightTime() {
        FightTimeTillAttack = TimePerAttack * Random.Range(0.9f, 1.1f);
    }

    public bool UpdateUnitFightTime(float delta) {
        FightTimeTillAttack -= delta;
        if (FightTimeTillAttack <= 0) {
            FightTimeTillAttack += TimePerAttack * Random.Range(0.9f, 1.1f);
            return true;
        }
        return false;
    }

    public void Heal(int heal) {
        Health = Mathf.Clamp(Health + heal, 0, MaxHealth);
        Mark.SetHealthFiller(Health / (float)MaxHealth);
    }

    public bool TryGetDamage(int damage, bool isIgnoreDodge = false) {
        if (isIgnoreDodge || Random.Range(0, 100) >= DodgePercent)
        {
            GetDamage(damage);
            Mark.SetHealthFiller(Health / (float)MaxHealth);
            return true;
        }
        return false;
    }

    public virtual void GetDamage(int damage) { 
        Health -= damage;
        if (Health <= 0) {
            Die(true);
        }
    }

    public virtual void Die(bool spawnDead) {
        if (IsDied)
            return;
        enabled = false;
        if (UnitManager.singleton.SelectedUnit == this)
        {
            UnitManager.singleton.Unselected();
        }
        UnitManager.singleton.UnitDeletedFromBoard(this);
        if (UnitTeam == UnitTeam.Player)
        {
            CardsManager.singleton.TotalCardsCount -= 1;
            CardsManager.singleton.RecalculateTotalCardsCount(true);
        }
        Ticker.TryDeleteTickable(this);
        Mark.RecalculateMarkDestroyed();
        Destroy(Mark.gameObject);
        Destroy(gameObject);
        IsDied = true;
    }

    public void ReinitValues(int health, int maxHealth, float timePerAttack, int dodgePercent, int damage, float deltaStamina) {
        if (IsDied)
            return;
        Health = health;
        MaxHealth = maxHealth;
        TimePerAttack = timePerAttack;
        DodgePercent = dodgePercent;
        Damage = damage;

        staminaNow += deltaStamina;
        staminaMax += deltaStamina;
        Mark.SetHealthFiller(Health / (float)MaxHealth);
        ResetUnitFightTime();
    }

    void SpawnUnitMark()
    {
        var mark = Instantiate(PrefabManager.GetPrefab("UI_UnitMark"), UIManager.singleton.UnitMarksContainer);
        mark.transform.position = new Vector3(4000, 4000, 4000);
        Mark = mark.GetComponent<UnitMark>();
        Mark.Init(this, UnitCard, UnitTeam);
    }

    void Start() {
        startPoint = transform.position;
        nextPoint = transform.position;
        Ticker.AddTickable(this, TickablePriority.High);
        //NextPoint();
    }

    public static (UnitActionType, string)[] GetActionTypeOnSelectedType(UnitCard unitCard, ISelectable selectable) {
        if (selectable == null || unitCard == null || UnitManager.singleton.Way == null || UnitManager.singleton.Way.Count == 0)
            return new[] { (UnitActionType.None, ""), (UnitActionType.None, "") };

        switch (unitCard.CardName)
        {
            case "Doctor":
                return Unit_Doctor.GetActionTypeOnSelectedType(unitCard, selectable);
            case "Guard":
                return Unit_Guard.GetActionTypeOnSelectedType(unitCard, selectable);
        }

        return new[] { (UnitActionType.Walk, (UnitManager.singleton.Way.Count / (float)unitCard.Speed).ToString("0.0")) };
    }

    public float GetMoveTime()
    {
        return (way.Count + value) / Speed;
    }

    protected virtual void DoAction()
    {
        //Debug.Log("ACTION!");
    }

    protected virtual void DoActionChangedTarget(ISelectable previousTarget, ISelectable newTarget)
    {
        //Debug.Log("ACTION changed target!");
    }

    protected virtual void DoActionUpdate()
    {
        //Debug.Log("ACTION update!");
    }

    protected virtual void DoActionTick()
    {
        //Debug.Log("ACTION tick!");
    }


    void Update()
    {
        transform.LookAt(CameraController.singleton.cameraTransform);
        if (isPaused || UnitCard == null || IsInFight)
            return;

        if (UnitCard.UseStamina)
        {
            staminaNow -= Time.deltaTime / Ticker.singleton.SecondsPerTick;
            if (staminaMax > 0)
            {
                Mark.SetStaminaFiller(staminaNow / staminaMax);
            }

            if (staminaNow <= 0)
            {
                UnitTired();
            }
        }

        if (IsMoving)
        {
            Move();
        }

        DoActionUpdate();
    }

    public void Tick(int luck)
    {
        if (IsInFight) 
            return;
        DoActionTick();
    }

    void NextPoint()
    {
        if (way.Count == 0) {
            DoAction();
        }

        if (way != null && way.Count > 0)
        {
            startPoint = nextPoint;
            nextPoint = way[0];
            way.RemoveAt(0);
        }
        else
        {
            IsMoving = false;
            transform.position = nextPoint;
            nextPoint = transform.position;
            startPoint = transform.position;
        }

        UnitManager.singleton.UnitMoved(this);
    }

    public virtual bool DoActionUnitMoved(Vector2Int pos) {
        return false;
    }

    void Move()
    {
        value += Time.deltaTime / Ticker.singleton.SecondsPerTick * Speed;
        if (value > 1)
        {
            if (way.Count > 0)
            {
                value -= 1;
            }
            NextPoint();
        }

        transform.position = Vector3.Lerp(startPoint, nextPoint, value);
    }

    void UnitTired()
    {
        if (UnitCard.GoInHandWhenTired)
        {
            var card = CardsManager.singleton.SpawnGameCard(UnitCardData);
            card.SetStamina(UnitCard.RestTime, UnitCard.RestTime);
            card.cardsPack.PutCardBack(card);
        }

        enabled = false;

        if (UnitManager.singleton.SelectedUnit == this)
        {
            UnitManager.singleton.Unselected();
        }
        UnitManager.singleton.UnitDeletedFromBoard(this);
        if (UnitTeam == UnitTeam.Player)
        {
            CardsManager.singleton.TotalCardsCount -= 1;
            CardsManager.singleton.RecalculateTotalCardsCount(true);
        }
        Ticker.TryDeleteTickable(this);
        Mark.RecalculateMarkDestroyed();
        Destroy(Mark.gameObject);
        Destroy(gameObject);
    }

    void OnDestroy() {
        Ticker.singleton.DeletePauseable(this);
        if (UnitTeam != UnitTeam.Player)
            UnitManager.singleton.EnemyCount -= 1;
        IsDied = true;
    }

    public void SetWay(List<Vector3> way, ISelectable target, SelectedType targetType)
    {
        if (IsInFight)
            return;

        value = 0;
        var previous = Target;
        TargetType = targetType;
        Target = target;
        this.way = way;
        if (way.Count > 0)
        {
            IsMoving = true;
            NextPoint();
        }
        else
        {
            IsMoving = false;
        }

        DoActionChangedTarget(previous, target);
    }

    public bool IsCanBeSelected()
    {
        return UnitTeam == UnitTeam.Player;
    }

    public void Selected()
    {

    }

    public GameObject GetSelectedObject()
    {
        return gameObject;
    }

    public SelectedType GetSelectedType()
    {
        return SelectedType.Unit;
    }

    public bool IsCanBeVisitedByUnits()
    {
        return false;
    }

    public List<GameObject> GetMeshObjects()
    {
        return new List<GameObject>();
    }

    public void SetPause(bool isPaused)
    {
        this.isPaused = isPaused;
    }
}
