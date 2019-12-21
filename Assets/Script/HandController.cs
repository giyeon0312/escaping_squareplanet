using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : CloseWeaponController
{
    //활성화여부
    public static bool isActivate = true;//false==guncontroller인경우,피격이펙특,정조준 안되도록


    void Start()
    {
        WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();//public인 요소를 우선 여기서 초기화한다
        //WeaponManager.currentWeaponAnim = m_handAnimator;
    }

    void Update()
    {
        if (isActivate)//손일경우에만
            TryAttack();
    }

    protected override IEnumerator HitCoroutine()
    {
        Debug.Log("주먹히트코루틴");
        while (isSwing)
        {
            if (CheckObject())
            {
                Debug.Log("막쳐!");
                Debug.Log(hitinfo.transform.name);

                if (hitinfo.transform.tag == "Enemy")  //곡괭이와 에너비가 부딪혔을경우
                    hitinfo.transform.GetComponent<EnemyController>().Damage(1, transform.position);

                isSwing = false;
            }
            yield return null;
        }
    }

    public override void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        base.CloseWeaponChange(_closeWeapon);
        isActivate = true;
    }
}

