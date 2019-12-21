using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeController : CloseWeaponController
{
    public static bool isActivate = false;//true==guncontroller인경우,피격이펙특,정조준 안되도록


    void Update()
    {
        if (isActivate)//손일경우에만
            TryAttack();
    }

    protected override IEnumerator HitCoroutine()
    {
        Debug.Log("pickaxe히트코루틴");
        while (isSwing)
        {
            if (CheckObject())
            {
                if (hitinfo.transform.tag == "Rock")        //곡괭이와 바위가 부딪혔을경우                
                    hitinfo.transform.GetComponent<Rock>().Mining();//Rock스크립트안의 Mining()을 호출
                else if (hitinfo.transform.tag == "Enemy")  //곡괭이와 에너비가 부딪혔을경우
                    hitinfo.transform.GetComponent<EnemyController>().Damage(1, transform.position);

                isSwing = false;
                Debug.Log(hitinfo.transform.gameObject.name);
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
