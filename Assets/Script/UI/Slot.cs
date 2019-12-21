using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;//인터페이스를 담당하는 기능이 있다 

public class Slot : MonoBehaviour,IPointerClickHandler,IBeginDragHandler,IDragHandler,IEndDragHandler,IDropHandler
{

    public Item item;   //획득한 아이템
    public Image itemImage;//아이템의 이미지

    private RaycastHit hitInfo;

    //WeaponType이 Equipment일때 장착을 위패
    private WeaponManager theWeaponManager;
    //HP와SP를 올려줄때 사용
    private StatusController theStatusController;

    void Start()
    {
        //프리팹은 기존에 있는 객체만 SerializeField로 받을 수 있어서
        theWeaponManager = FindObjectOfType<WeaponManager>();
        theStatusController = FindObjectOfType<StatusController>();
    }

    //이미지의 투명도 조절
    //인벤토리에서 색이 보이지 않다가 획득하면 색이 보이도록
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;//0이 넘어오면 안보이고 1이 넘어오면 다시 안보이게 된다
    }

    //아이템 획득
    public void AddItem(Item _item)
    {
        item = _item;
        itemImage.sprite = item.itemImage;
        SetColor(1);
    }

    //슬롯 초기화 혹은 사용?
    private void ClearSlot()
    {
        item = null;
        itemImage.sprite = null;
        SetColor(0);
    }


    /*상속받은 인터페이스들*/

    //슬롯 위에서 우클릭시 장비면 장비 교체 아니면 사용
    public void OnPointerClick(PointerEventData eventData)         //이 스크립트가 적용된 객체에
    {                                                              //우클릭을 하면
        if (eventData.button == PointerEventData.InputButton.Right)//이벤트 내용이 실행된다
        {
            if (item != null)
            {
                if (item.itemType == Item.ItemType.Equipment)//장비일경우 장착
                {
                    StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(item.weaponType));
                }
                else if (item.itemType == Item.ItemType.Ingredient)//포션일경우
                {
                    if(item.itemName=="PotionHP")
                    {
                        theStatusController.IncreaseHP(100);
                        theStatusController.SPRecoverByPotion();
                    }
                    Debug.Log(item.itemName + "을 사용했습니다");
                    ClearSlot();
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImage);
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isRightPlace(DragSlot.instance))                    //열쇠나 책일경우- 올바른 위치인지 확인
        {
            Debug.Log(item.itemName + "을 사용했습니다");
            ClearSlot();
        }
        DragSlot.instance.SetColor(0);
        DragSlot.instance.dragSlot = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.transform.position = eventData.position;
        }
    }


    //슬롯의 아이템이 제자리에 놓였는지 확인한다
    private bool isRightPlace(DragSlot ds)
    {
        if(Physics.Raycast(ds.transform.position, (ds.transform.forward), out hitInfo, 10.0f))
        {
            if (hitInfo.transform.name == "FirstDoor" && item.itemName=="WrenchKey")
            {
                //아냐 어쩌면 그냥 문이 열리는 애니메이션만 동작시킴
                Debug.Log("첫번째 키가 제자리에 왔군!");
                return true;
            }
            else if (hitInfo.transform.name == "Battery" && item.itemName == "Battery")
            {
                Debug.Log("베터리가 제자리에 왔군");
                //Instantiate(battery, col.bounds.center, Quaternion.identity);<-여기서 하거나 혹은 드래그할까ㅠㅠ
                return true;
            }
            else if (hitInfo.transform.name == "LastDoor" && item.itemName == "ButtonKey")
            {
                Debug.Log("조건이 만족된 상태에서 버튼을 누르면 끝이 난다");
                ;//Instantiate(buttonkey, col.bounds.center, Quaternion.identity);<-여기서 하거나 혹은 드래그할까ㅠㅠ
                return true;
            }
            else
            {
                //슬롯 제자리로 돌아가라
                return false;
            }
        }
        return false;
    }





}




/*else if (isRightPlace())                    //열쇠나 책일경우- 올바른 위치인지 확인
{
    Debug.Log(item.itemName + "을 사용했습니다");
    //Instantiate(go_effect_prefabs, col.bounds.center, Quaternion.identity);<-여기서 하거나 혹은 드래그할까ㅠㅠ
    ClearSlot();
}*/
