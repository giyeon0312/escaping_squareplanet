using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static bool inventoryActivated = false;


    // 필요한 컴포넌트
    [SerializeField]
    private GameObject go_InventoryBase;
    [SerializeField]
    private GameObject go_SlotsParent;//슬롯들 한번에 가져오려고

    // 슬롯들
    private Slot[] slots;


    // Use this for initialization
    void Start()
    {
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();
    }

    // Update is called once per frame
    void Update()
    {
        TryOpenInventory();
    }

    //인벤토리 활성화와 비활성화 I키로
    private void TryOpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryActivated = !inventoryActivated;

            if (inventoryActivated)
                go_InventoryBase.SetActive(true);
            else
                go_InventoryBase.SetActive(false);
        }
    }

    //maincamera에 있는 ActionController에서 호출된다
    public void AcquireItem(Item _item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)//슬롯의 빈 공간에 획득한 아이템을 넣어준다
            {
                slots[i].AddItem(_item);
                return;
            }
        }
    }


}
