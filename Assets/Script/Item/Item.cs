using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Hierachy창에서 만들수 있도록
[CreateAssetMenu(fileName ="New Item",menuName ="New Item/item")]
public class Item : ScriptableObject
{
   
    public string itemName;//아이템의 이름
    public ItemType itemType;//아이템의 유형
    public Sprite itemImage;//아이템의 이미지<-sprite여야 world상에서 볼수 있지
    public GameObject itemPrefab;//아이템의 프리팹

    public string weaponType;//무기유형:GUN.PICKAXE

    public enum ItemType
    {
        Equipment,  //장비
        Use,        //소모품
        Ingredient, //재료
        ETC         //기타
    }

}
