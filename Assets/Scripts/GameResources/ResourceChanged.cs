using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceChanged : MonoBehaviour
{
    public float LiveTime = 0.5f;
    public float DisapearTime = 0.5f;
    public float Speed = 1;

    public Image ResourceImage;
    public Image ResourceOutlineImage;
    public Text CountText;

    private float a;

    public Color negativeColor;

    private void Start()
    {
        a = 1 / DisapearTime;
    }

    public void SetData(Sprite sprite, Color spriteColor, Sprite outlineSprite, Color outlineSpriteColor, int count) {
        ResourceImage.sprite = sprite;
        ResourceImage.color = spriteColor;
        ResourceOutlineImage.sprite = outlineSprite;
        ResourceOutlineImage.color = outlineSpriteColor;

        CountText.text = count > 0 ? $"+{count}" : count.ToString();
        if (count < 0)
            CountText.color = negativeColor;
    }

    void Update()
    {
        transform.localPosition += new Vector3(0, -Speed * Time.deltaTime);

        LiveTime -= Time.deltaTime;
        if (LiveTime <= 0) {
            DisapearTime -= Time.deltaTime;
            if (DisapearTime <= 0)
                Destroy(gameObject);
            ResourceImage.color = new Color(1, 1, 1, DisapearTime * a);
            CountText.color = new Color(negativeColor.r, negativeColor.g, negativeColor.b, DisapearTime * a);
        }
    }
}
