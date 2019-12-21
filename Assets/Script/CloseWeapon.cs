using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseWeapon : MonoBehaviour
{
    public string closeWeaponName;//근접 무기 이름

    //무기 유형
    public bool isHand;// 손인지
    public bool isPickaxe;//곡괭이인지

    public float range;//공격범위
    public int damage;//공격력
    public float workSpeed;//작업속도
    public float attackDelay;//공격딜레이
    public float attackDelayA;//공격활성화시점-주먹이 뻗는시점
    public float attackDelayB;//공격비활성화시점-주먹을 가져오는 시점

   // public Animator anim;
    //public BoxCollider boxCollider;//주먹에 넣어서 닿으면 부서지도록

}
