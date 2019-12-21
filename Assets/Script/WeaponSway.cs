using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    //기존 위치
    private Vector3 originPos;

    //현재 위치
    private Vector3 currentPos;

    //sway한계 무기가 흔들리는 최대치
    [SerializeField] private Vector3 limitForce;

    //정조준 sway 한계
    [SerializeField] private Vector3 fineSightLimitPos;

    //부드러운 움직임 정도
    [SerializeField] private Vector3 smoothSway;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
