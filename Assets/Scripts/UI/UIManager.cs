using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager singleton;

    public Transform CursorContainer;
    public Transform UnitMarksContainer;

    public Camera mainCamera;
    CustomButton selectedCustomButton;
    CameraController cameraController;
    GraphicRaycasterUI graphicRaycasterUI;

    bool isButtonHold;
    bool isSendedHolded;
    int mouseButton;
    float buttonHoldTime;
    float timeToHold;

    private CardsManager cardsManager;

    public bool IsMenuOpened;
    public GameObject MenuObject;
    public void Awake()
    {
        singleton = this;
        graphicRaycasterUI = GetComponent<GraphicRaycasterUI>();
    }

    private void Start()
    {
        cameraController = CameraController.singleton;
        mainCamera = Camera.main;

        cardsManager = CardsManager.singleton;
    }

    void Update()
    {
        UpdatedMousePosition(cameraController.Input_MousePosition);
        if (isButtonHold)
        {
            buttonHoldTime += Time.deltaTime;
            if (!isSendedHolded && buttonHoldTime >= timeToHold)
            {
                selectedCustomButton?.OnHold(mouseButton);
                isSendedHolded = true;
            }
        }
    }

    public void SwitchMenu()
    {
        IsMenuOpened = !IsMenuOpened;
        Ticker.singleton.AddPausingCount(IsMenuOpened ? 1 : -1, true);
        MenuObject.SetActive(IsMenuOpened);
    }

    public void ExitToMenu() {
        GameManager.IsWin = false;
        var e = SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }

    public void UpdatedMousePosition(Vector2 pos)
    {
        CursorContainer.transform.position = mainCamera.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 1.5f));
    }

    public void RaycasterUpdated(List<RaycastResult> raycastResults)
    {
        var customButtons = raycastResults.Where(x => x.gameObject.TryGetComponent<CustomButton>(out var e)).ToList();

        if (cardsManager.IsCardInHand)
        { 
            customButtons = raycastResults.Where(x => 
            x.gameObject != cardsManager.SelectedCard.gameObject && 
            x.gameObject.TryGetComponent<CustomButton>(out var e)).ToList();

            cardsManager.UpdateSelectedCard();
        }
        if (customButtons.Count == 0)
        {
            selectedCustomButton?.OutHover();
            selectedCustomButton = null;
        }
        else {
            var value = customButtons.Max(x => x.depth);
            var newButton = customButtons.Where(x => x.depth == value).First().gameObject.GetComponent<CustomButton>();
            if (selectedCustomButton == newButton)
                return;
            selectedCustomButton?.OutHover();
            selectedCustomButton = newButton;
            newButton.OnHover();
        }
    }

    public void MouseDown(int mouseButton)
    {
        switch (mouseButton)
        {
            case 0:
                isButtonHold = true;
                cameraController.CheckIsStillOnGUI();
                if (selectedCustomButton != null)
                    timeToHold = selectedCustomButton.GetPressedTime();
                this.mouseButton = mouseButton;

                break;
            case 1:

                break;
        }
    }

    public void MouseUp(int mouseButton)
    {
        if (mouseButton == 0)
        {
            if (!cameraController.IsMouseOnGUI && cardsManager.SelectedCard == null && BuildingInfo.singleton.IsOpened && buttonHoldTime < 0.2f)
            {
                BuildingInfo.singleton.Close();
            }

            if (buttonHoldTime < 0.2f)
            {
                if (!cameraController.IsMouseOnGUI && cardsManager.SelectedCard == null)
                {
                    var selected = Selector.singleton.SelectedObject;
                    if (selected != null)
                    {
                        if (selected.GetSelectedObject().TryGetComponent<PlacedBuilding>(out var placedBuilding))
                        {
                            if (GameCameraMode.CameraMode == EGameCameraMode.Resources)
                            {
                                if (cardsManager.SelectedCard == null)
                                {
                                    if (placedBuilding.TryGetComponent<B_House>(out var house))
                                    {
                                        house.ChangeFeedState();
                                    }
                                }
                            }
                            else
                            {
                                if (placedBuilding.IsHaveBuildingInfo())
                                {
                                    BuildingInfo.singleton.LoadBuilding(placedBuilding);
                                }
                            }
                        }
                    }
                }

                if (cardsManager.SelectedCard != null)
                {
                    //Try put card back
                    if (cardsManager.SelectedCard.cardsPack.IsHasEmptySpace)
                    {
                        cardsManager.PutCardBackFromHand();
                        return;
                    }

                    //Try use card
                    cardsManager.UseCard();
                }
                else
                {
                    if (Selector.singleton.SelectedType == SelectedType.Unit)
                    {
                        UnitManager.singleton.UnitSelected(Selector.singleton.SelectedObject);
                    }
                }
            }
        }

        if (buttonHoldTime < timeToHold)
        {
            selectedCustomButton?.OnClicked(mouseButton);
        }
        else
        {
            selectedCustomButton?.OnRelease(mouseButton);
        }
        selectedCustomButton = null;
        buttonHoldTime = 0;
        isButtonHold = false;
        isSendedHolded = false;
        cameraController.CheckIsStillOnGUI();

        if (mouseButton == 1)
        { 
            cardsManager.PutCardBackFromHand();
        }
    }
}
