using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    //무기교체 중복 실행 방지
    public static bool isChangeWeapon = false;//상태를 모두 공유하기위해 static,클래스변수=정적변수

    //현재무기와 현재무기의 애니메이션,public인 둘 우선 Animator는 Start에서,currentWeaon은 HandController에서 초기화
    public static Transform currentWeapon;//GunController와 HandController각각 실행할 때 기존의것 transform컴표넌트 끄는역할
    public static Animator currentWeaponAnim;
    [SerializeField] public Animator m_handAnimator;
    [SerializeField] public Animator m_pickaxeAnimator;
    [SerializeField] public Animator m_gunAnimator;

    //현재 무기의 타입
    [SerializeField] string currentWeaponType;

    //무기교체 딜레이-손이 바뀌아야하므로,각 10프레임정도,,?
    [SerializeField]
    private float changeWeaponDelayTime;
    [SerializeField]
    private float changeWeaponEndDelayTime;//무기 교체가 완전히 끝나는 시간


    //손과 무기 종류들 전부 관리
    [SerializeField] private Gun gun;
    [SerializeField] private CloseWeapon hand;
    [SerializeField] private CloseWeapon pickaxe;



    //셋을 교체로 꺼가며 사용할 것
    [SerializeField] private GunController theGunController;
    [SerializeField] private HandController theHandController;
    [SerializeField] private PickaxeController thePickaxeController;


    
    // Start is called before the first frame update
    void Start()
    {
        currentWeaponAnim = m_handAnimator;
    }

    // Update is called once per frame
    void Update()
    {
        //퀵슬롯을 구현하기 전까지 번호를 눌러서 해당 번호에 맞는 hand나 gun이 나오도록
        if (!isChangeWeapon)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {   //무기교체 실행(맨손),코루틴으로
                StartCoroutine(ChangeWeaponCoroutine("HAND"));
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {   //무기교체 실행(서브머신건)
                StartCoroutine(ChangeWeaponCoroutine("GUN"));
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {   //무기교체 실행(Pickaxe)
                StartCoroutine(ChangeWeaponCoroutine("PICKAXE"));
            }
        }
    }



    //무기교체 코루틴
    public IEnumerator ChangeWeaponCoroutine(string _type)
    {
        isChangeWeapon = true;

        if (currentWeaponType == "HAND")
            currentWeaponAnim.SetTrigger("Hand_Weapon_Out");
        else if (currentWeaponType == "PICKAXE")
            currentWeaponAnim.SetTrigger("Pickaxe_Weapon_Out");
        else if (currentWeaponType == "GUN") 
            currentWeaponAnim.SetTrigger("Gun_Weapon_Out");
        

        yield return new WaitForSeconds(changeWeaponDelayTime);

        CancelPreWeaponAction();//정조준상태였으면 해제한다
        WeaponChange(_type);

        yield return new WaitForSeconds(changeWeaponEndDelayTime);
        isChangeWeapon = false;
        currentWeaponType = _type;
    }

    //이전에 연결되어있던 weapon의 종료액션 취하도록
    private void CancelPreWeaponAction()
    {
        switch (currentWeaponType)
        {
            case "GUN":
                theGunController.CancelFineSightMode();
                theGunController.CancelRelod();
                GunController.isActivate = false;//이전의 것 적용 취소
                break;
            case "HAND":
                HandController.isActivate = false;//이전의 것 적용 취소
                break;
            case "PICKAXE":
                PickaxeController.isActivate = false;//이전의 것 적용 취소
                break;
        }
    }

    private void WeaponChange(string _type)
    {
        if (_type == "GUN")
        {
            currentWeaponAnim = m_gunAnimator;
            theGunController.GunChange(gun);
        }else if (_type == "HAND")
        {
            currentWeaponAnim = m_handAnimator;
            theHandController.CloseWeaponChange(hand);
        }else if (_type == "PICKAXE")
        {
            currentWeaponAnim = m_pickaxeAnimator;
            thePickaxeController.CloseWeaponChange(pickaxe);
        }

    }


}
