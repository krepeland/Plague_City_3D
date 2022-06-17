using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CardsLimitDisplay : MonoBehaviour
{
    private bool IsMovingToTargetPos;
    private Vector2 AnchoredTargetPos = Vector2.zero;
    private RectTransform rectTransform;

    [HideInInspector] public float MoveToTargetSpeed = 100;
    public Text MaxCardText;
    public RectTransform Fixer;
    public RectTransform Fixer2;
    public CardsPack BuildingsPack;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {

        if (IsMovingToTargetPos)
        {
            Fixer2.transform.position = Fixer.transform.position;

            var dist = Vector2.Distance(rectTransform.anchoredPosition, AnchoredTargetPos);
            if (dist <= 0.1f || Time.deltaTime * MoveToTargetSpeed / dist >= 1)
            {
                IsMovingToTargetPos = false;
                rectTransform.anchoredPosition = AnchoredTargetPos;
            }
            else
            {
                rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, AnchoredTargetPos, Time.deltaTime * MoveToTargetSpeed / dist);
            }

            if (!BuildingsPack.IsHidden)
            {
                if (rectTransform.anchoredPosition.x < Fixer2.anchoredPosition.x)
                {
                    rectTransform.anchoredPosition = new Vector2(Fixer2.anchoredPosition.x, 10);
                }
            }
        }
    }

    public void SetPackTargetMove(Vector2 anchoredTargetPos, float speed)
    {
        AnchoredTargetPos = anchoredTargetPos;
        MoveToTargetSpeed = speed;
        IsMovingToTargetPos = true;

        var flag = true;
        foreach (var pack in CardsManager.singleton.AllCardsPacks)
            if (!pack.IsHidden)
                flag = false;

        if (flag)
            AnchoredTargetPos -= new Vector2Int(60, 0);
    }

    public void SetCardsCount(int count, int maxCount) {
        MaxCardText.text = $"{count}/{maxCount}\nCARDS";
    }
}
