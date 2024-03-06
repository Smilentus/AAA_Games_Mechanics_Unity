using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenuItemView : MonoBehaviour
{
    [SerializeField]
    private Image iconImage;

    [SerializeField]
    private TMP_Text titleTMP;


    public void SetData(RadialMenuItem item)
    {
        if (titleTMP != null)
            titleTMP.text = item.Title;

        if (iconImage != null)
            iconImage.sprite = item.Icon;
    }
}
