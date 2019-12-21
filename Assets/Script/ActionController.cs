using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float range;                  //습득 가능 최대 거리 

    private bool pickupActivated = false;//습득 가능할 시 true

    private RaycastHit hitInfo;          //충돌체 정보 저장

    [SerializeField]
    private LayerMask layerMask;        //아이템의 레이어(Item)에 대해서만 반응하도록 레이어마스크 설정

    //필요한 컴포넌트
    [SerializeField]
    private Text actionText;
    [SerializeField]
    private Inventory theInventory;
 
    // Update is called once per frame
    void Update()
    {
        CheckItem();
        TryAction();       
    }

    private void TryAction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckItem();
            CanPickUp();
        }
    }

    private void CanPickUp()
    {
        if (pickupActivated)
        {
            if (hitInfo.transform != null)
            {
                Debug.Log(hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + "획득함!");
                theInventory.AcquireItem(hitInfo.transform.GetComponent<ItemPickUp>().item);
                Destroy(hitInfo.transform.gameObject);
                InfoDisapper();
            }
        }
    }

    private void CheckItem()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, range, layerMask))
        {
            if (hitInfo.transform.tag == "Item")
            {
                ItemInfoAppear();
            }
        }
        else
        {
            InfoDisapper();
        }
    }

    private void ItemInfoAppear()
    {
        pickupActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + "획득" + "<color=yellow>" + "(E)" + "</color>";
    }

    private void InfoDisapper()
    {
        pickupActivated = false;
        actionText.gameObject.SetActive(false);
    }
}
