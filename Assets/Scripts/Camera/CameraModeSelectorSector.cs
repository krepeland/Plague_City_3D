using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraModeSelectorSector : CustomButton
{
    [SerializeField] Image SectorImage;
    [SerializeField] Image SectorOutlineImage;

    [SerializeField] Sprite SelectedSectorSprite;
    [SerializeField] Sprite SelectedOutlineSprite;
    [SerializeField] Sprite NormalSectorSprite;
    [SerializeField] Sprite NormalOutlineSprite;

    [SerializeField] Transform ScaleTransform;

    public int Index;
    CameraModeCircleSelector selector;
    bool isSelected;

    public void SetIsSelected(bool isSelected) {
        if (this.isSelected == isSelected)
            return;

        this.isSelected = isSelected;
        if (isSelected)
        {
            //SectorImage.sprite = SelectedSectorSprite;
            //SectorOutlineImage.sprite = SelectedOutlineSprite;
            ScaleTransform.localScale = new Vector3(1.1f, 1.1f, 1f);
        }
        else
        {
            //SectorImage.sprite = NormalSectorSprite;
            //SectorOutlineImage.sprite = NormalOutlineSprite;
            ScaleTransform.localScale = Vector3.one;
        }
    }

    private void Start()
    {
        selector = CameraModeCircleSelector.singleton;
    }

    public override void OnHover()
    {
        base.OnHover();
        selector.SetIndex(Index);
    }

    public override void OutHover()
    {
        base.OutHover();
    }
}
