using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CloseWeaponController : MonoBehaviour
{
    //미완성 클래스-미완성 코루틴갖고있음

   
    //현재 장착된 Hand
    [SerializeField] protected CloseWeapon currentCloseWeapon;

    //현재 공격중인지 여부
    protected bool isAttack = false;
    protected bool isSwing = false;

    protected RaycastHit hitinfo;
    [SerializeField] protected LayerMask layerMask;

    // Update()<-추상클래스일때 게임오브젝트의 컴포넌트로 추가 못한다,따라서 불가

    protected void TryAttack()
    {
        if (!Inventory.inventoryActivated)
        {
            if (Input.GetButton("Fire1") && (HandController.isActivate || PickaxeController.isActivate))//GunController에도 좌클릭시 TryFire있으므로 
            {//마우스 좌클릭을 누르고 있는 동안 공격
                if (!isAttack)
                {
                    StartCoroutine(AttackCoroutine());//코루틴 실행
                }
            }
        }
      
    }

    IEnumerator AttackCoroutine()
    {
        
        isAttack = true;

        if (HandController.isActivate == true)
        {
            WeaponManager.currentWeaponAnim.SetTrigger("Hand_Attack");
        }
        else if (PickaxeController.isActivate == true)
        {
            WeaponManager.currentWeaponAnim.SetTrigger("Pickaxe_Attack");
        }
        //한번 코루틴 실행된 후 지연될 정도
        yield return new WaitForSeconds(currentCloseWeapon.attackDelayA);//0.5초
        isSwing = true;//공격 시작

        //공격 활성화 시점
        StartCoroutine(HitCoroutine());

        yield return new WaitForSeconds(currentCloseWeapon.attackDelayB);//0.1초
        isSwing = false;

        yield return new WaitForSeconds(currentCloseWeapon.attackDelay - currentCloseWeapon.attackDelayA - currentCloseWeapon.attackDelayB);//1초-0.5초-0.1초
                                                                                                                                                //재공격이 이루어질 수 있도록 isAttack=false;
        isAttack = false;
        
    }

    //공격 적중을 했는지 알려주는 추상 코루틴
    //충돌감지해서 true이면 충돌검사 끝내도록
    //데미지를 입히거나 뭔가를 뿌셔서 얻거나->자식클래스에서 완성한다
    protected abstract IEnumerator  HitCoroutine();


    protected bool CheckObject()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitinfo, currentCloseWeapon.range,layerMask))
        {
            return true;
        }
        return false;
    }

    //가상함수:완성함수이지만 추가 편집이 가능한 함수->자식 클래스에서 isActivate = true로 하려고
    public virtual void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        if (WeaponManager.currentWeapon != null)//이미 무언가를 들고 있는 모습
            WeaponManager.currentWeapon.gameObject.SetActive(false);

        currentCloseWeapon = _closeWeapon;

        WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();//currentWeapon은 Hand나 Gun둘 다 올 수 있도록 transform형
       
        if (_closeWeapon.gameObject.name == "Hand"){
            WeaponManager.currentWeaponAnim.SetTrigger("Hand_Weapon_In");
        } else if (_closeWeapon.gameObject.name == "Pickaxe") { 
            WeaponManager.currentWeaponAnim.SetTrigger("Pickaxe_Weapon_In");
        }   
        currentCloseWeapon.transform.localPosition = Vector3.zero;
        currentCloseWeapon.gameObject.SetActive(true);
  
    }
}
