using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public string gunName;//총의 이름(하나 이상일경우 필요)->후에 다른 무기들이 상속할것이므로
    public float range;//총의 사정거리
    public float accuracy;//정확도
    public float fireRate;//연사속도-높을수록 느려진다
    public float reloadTime;//재장전 속도

    public int damage;//총의 데미지
    /*여러개 지워야지*/
    public int reloadBulletCount;//한번에 장전 할 수 있는개수
    public int currentBulletCount;//현재 탄알집에 남아있는 총알의 개수
    public int maxBulletCount;//최대 소유 가능 총알개수
    public int carryBulletCount;//현재 소유하고 있는 총알의 개수

    public float retroActionForce;//반동세기
    public float retroActionFineSightForce;//정조준세의 반동세기

    public Vector3 fineSigntOriginPos;//정조준시의 위치
    public ParticleSystem muzzleFlash;//총구섬광

    public AudioClip fireSound;//총 발사 할 때 나오는 소리

}
