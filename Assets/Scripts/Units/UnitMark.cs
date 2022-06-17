using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitMark : MonoBehaviour
{
    UnitBase unit;
    [SerializeField] Image UnitImage;
    [SerializeField] Image StaminaFiller;
    [SerializeField] Image HealthFiller;

    [SerializeField] GameObject ActionObject;
    [SerializeField] Image ActionImage;
    [SerializeField] Image ActionFiller;

    Transform cameraTransform;
    [SerializeField] UnitMarkButton button;
    public float TargetAngle = 0;
    public float RotationSpeed = 60f;
    public UnitTeam UnitTeam;
    public bool IsInteractable;

    public void Init(UnitBase unit, UnitCard unitCard, UnitTeam unitTeam) {
        this.unit = unit;
        cameraTransform = UIManager.singleton.mainCamera.transform;
        if (unitCard.MarkSprite != null)
            UnitImage.sprite = unitCard.MarkSprite;

        button.Index = transform.GetSiblingIndex();

        UnitTeam = unitTeam;
        button.SetIsInteractable(UnitTeam == UnitTeam.Player);

        //if (UnitTeam != UnitTeam.Player) {
        //    transform.localScale = Vector3.one / 1.5f;
        //}
    }

    public void SetIsInteractable(bool isInteractable)
    {
        IsInteractable = isInteractable && UnitTeam == UnitTeam.Player;
        button.SetIsInteractable(IsInteractable);
    }

    public void SetStaminaFiller(float value) {
        StaminaFiller.fillAmount = value;
    }

    public void SetHealthFiller(float value)
    {
        if (HealthFiller == null)
            return;
        HealthFiller.enabled = (value != 1);
        HealthFiller.fillAmount = value;
    }

    public void SetActionSprite(Sprite sprite)
    {
        ActionImage.sprite = sprite;
    }

    public void SetActionFiller(float value)
    {
        ActionFiller.fillAmount = value;
    }

    public void SetActionEnabled(bool isActive)
    {
        ActionObject.SetActive(isActive);
    }

    public bool IsActionObjectEnabled() {
        return ActionObject.activeSelf;
    }

    void OnGUI()
    {
        var ray = (cameraTransform.position - unit.MarkPoser.position).normalized;
        transform.position = cameraTransform.position - ray * 1.5f;
        //var ray2 = UIManager.singleton.mainCamera.ViewportPointToRay(UIManager.singleton.mainCamera.WorldToViewportPoint(unit.MarkPoser.position));
        //transform.position = ray2.origin + ray2.direction;
    }

    private void Update()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 0, TargetAngle), Time.deltaTime * RotationSpeed);
    }

    public void RecalculateMarkDestroyed()
    {
        button.RecalculateOther(button);
    }

    public void Holded(int mouseButton) {
        UnitManager.singleton.UnitSelected(unit);
    }
}
