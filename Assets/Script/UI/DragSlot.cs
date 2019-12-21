using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{

    static public DragSlot instance;

    //드래그가 시작되면 슬롯을 아래에 넣는다(Slot에서 호출됨)
    public Slot dragSlot;

    //드래그하는 슬롯의 아이템 이미지
    [SerializeField]
    private Image imageItem;

    void Start()
    {
        instance = this;
    }

    //드래그 하면 호출되는 이미지
    public void DragSetImage(Image _itemImage)
    {
        imageItem.sprite = _itemImage.sprite;
        SetColor(1);
    }

    //드래그할때만 보여주도록
    public void SetColor(float _alpha)
    {
        Color color = imageItem.color;
        color.a = _alpha;
        imageItem.color = color;
    }
}
