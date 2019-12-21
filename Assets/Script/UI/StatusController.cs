using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    //체력
    [SerializeField] private int hp;
    [SerializeField] private Slider currentHpSlider;


    //스테미나
    [SerializeField] private int sp;
    [SerializeField] private Slider currentSpSlider;


    //스테미나 증가량과 증가까지 걸리는 시간
    [SerializeField] private int spIncreaseAmount;
    [SerializeField] private int spRechargeTime;
    private int currentSpRechargeTime;//시간 계산용

    //스테미나 감소 여부
    private bool spUsed;



    // Start is called before the first frame update
    void Start()
    {
        currentHpSlider.value = hp;
        currentSpSlider.value = sp;
    }

    // Update is called once per frame
    void Update()
    {
        SPRechargeTime();
        SPRecover();
    }

    public void IncreaseHP(int _count)
    {
        if (currentHpSlider.value + _count < hp)
            currentHpSlider.value += _count;
        else
            currentHpSlider.value = hp;
        
    }

    public void DecreaseHP(float _count)
    {
        if (currentSpSlider.value > 0)
        {
            DecreaseStamina((int)(_count*10));
            return;
        }
        else
        {
            currentHpSlider.value -= _count;
        }

        if (currentHpSlider.value - _count <= 0)
            Debug.Log("솔직헌 심정으루는 게임오버되는게 맞기는 헌디..");
        
            
    }

    //sp회복 시간 계산
    private void SPRechargeTime()
    {
        if (spUsed)
        {
            if (currentSpRechargeTime < spRechargeTime)
                currentSpRechargeTime++;
            else
                spUsed = false;
        }
    }

    private void SPRecover()
    {
        if(!spUsed && currentSpSlider.value < sp)
        {
            currentSpSlider.value += spIncreaseAmount;
        }   
    }

    //IncreaseHP와 함께 Slot에서 호출된다
    public void SPRecoverByPotion()
    {
        if (spUsed)
        {
            while(currentSpSlider.value!=sp)
                currentSpSlider.value += spIncreaseAmount;
        }
    }

    //DecreaseHP&PlayerController클래스-TryJump와 TryRun에서 호출된다
    public void DecreaseStamina(int _count)
    {
        spUsed = true;
        currentSpRechargeTime = 0;//후에 증가하여 spRechargeTime까지 되면 회복하는것

        if (currentSpSlider.value - _count > 0)
            currentSpSlider.value -= _count;
        else
            currentSpSlider.value = 0;
    }

    //현재 sp가0이하면 뛰거나 점프할 수 없다-마찬가지로 PlayerController에서 조건확인후
    public int GetCurrentSP()
    {
        return (int)currentSpSlider.value;
    }
}
