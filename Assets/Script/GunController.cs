using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public static bool isActivate = false;//true==handcontroller인경우,피격이펙특,정조준 안되도록


    [SerializeField]
    private Gun currentGun;                //현재 장착된 총

    private float currentFireRate;        //연사속도계산,1초마다 -1,0이되면 발사가능

    //상태변수
    private bool isReload = false;        //재장전중에는 발사못하도록 상태감지
    private bool isFineSightMode = false; //정조준상태

    private Vector3 originPos;            //정조준 후에 다시 돌아오는 본래 포지션,SubMachineGun처럼 초기값(0,0,0)

    private AudioSource audioSource;      //효과음

    private RaycastHit hitInfo;           //레이저 충돌정보 받아옴
    [SerializeField] private LayerMask layerMask;

    //필요한 컴포넌트
    [SerializeField]
    private Camera theCam;                //총알이 맞는?발사되는 크로스헤어있는 지접

    [SerializeField]
    private GameObject hit_effect_prefab;//피격이페트(파티클시스템)



    // Start is called before the first frame update
    void Start()
    {
        originPos = Vector3.zero;
        audioSource = GetComponent<AudioSource>();//같은계층의 컴포넌트 가져옴
    }

    // Update is called once per frame
    void Update()
    {
        if (isActivate)//총일경우에만(손이면 false)
        {
            GunFireRateCalc();//연사속도계산
            TryFire();        //발사시도
            TryReload();      //수동으로 재장전(currentBullet<0이면 자동재장전)
            TryFineSight();   //정조준시도
        }
    }




    //연사속도 재계산
    private void GunFireRateCalc()
    {
        if (currentFireRate > 0)
            currentFireRate -= Time.deltaTime;
        //매프레임마다 호출되므로 대략 한번에 1/60정도->1초에 60/60=1씩 감소

    }

    //발사시도(방아쇠 당기나 감지)
    private void TryFire()
    {   //Hand와 동일하게 왼쪽 마우스를 누르고 있으면 발사,재장전중에는 발사 불가
        if (!Inventory.inventoryActivated)
        {
            if (Input.GetButton("Fire1") && currentFireRate <= 0 && isReload == false && isActivate == true)
            {
                Fire();
            }
        }
    }

    private void Fire()//방아쇠 당긴후 발사 전
    {
        if (isReload == false)
        {
            //총 내부의 총알이 있어야만 발사 가능
            if (currentGun.currentBulletCount > 0)
            {
                Shoot();
            } //총 내부의 총알이 없으면 재장전
            else
            {
                CancelFineSightMode();            //재정전 아닐경우에만 정조준 가능하도록,정조준푼다
                StartCoroutine(ReloadCoroutine());//코루틴으로 구현해 재장전(Reload)과 발사(Shoot)동시에 일어나지 않도록
            }
        }
    }

    private void Shoot()//방아쇠 당긴후 발사 후
    {
        currentGun.currentBulletCount -= 1;   //총알하나 소비
        currentFireRate = currentGun.fireRate;//연사 속도 재계산,할당해준 현재 Gun의 fireRate로 초기화,다시0이될때까지 재계산하도록
        PlaySE(currentGun.fireSound);         //소리효과
        currentGun.muzzleFlash.Play();        //총구섬광효과
        Hit();
        StopAllCoroutines();                   //기존의 코루틴 멈춰주고(아래꺼와 겹치는것 방지)
        StartCoroutine(RetroActionCoroutine());//총기반동 코루틴 실행

    }
    //발사족족 맞추는 방법<->한번발사시마다 object만들어서 발사하려면 빠르게 나가기 위해 오브젝트풀링 필요
    //총알이 명중확인,미리 총알을 많이 만들어서 활성화-비활성화하는 방식 이용할것=오브젝트풀링(한번발사시마다 object만들면 렉걸릴 확률->)
    private void Hit()
    {
        if (Physics.Raycast(theCam.transform.position, theCam.transform.forward, out hitInfo, currentGun.range,layerMask))
        {//theCam의 위치에서 정면으로 사정거리 range만큼 레이저를 쏴서 충돌해 반환되는 정보가 있으면 hirInfo에 저장 
            var clone=Instantiate(hit_effect_prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            //피격에펙트를 활성화한다.레이저 충돌이일어난 곳에서,레이저충돌이 일어난 표면 방향으로
            if (hitInfo.transform.tag == "Enemy")  //곡괭이와 에너비가 부딪혔을경우
                hitInfo.transform.GetComponent<EnemyController>().Damage(1, transform.position);
            Destroy(clone, 2f);//반환되는 타입을 모를 때 var,하지만 여기선 object지모,,
        }
    }

    //수동 재장전시도
    private void TryReload()
    {   //R이 눌리고 현재 재장전중이 아니고 총 내부에 총알이 꽉차있지 않아야 가능
        if (Input.GetKeyDown(KeyCode.R) && isReload == false && currentGun.currentBulletCount < currentGun.reloadBulletCount)
        {
            CancelFineSightMode();          //재정전 아닐경우에만 정조준 가능하도록,정조준푼다
            StartCoroutine(ReloadCoroutine());
        }
    }

    public void CancelRelod()
    {
        if (isReload)
        {
            StopAllCoroutines();
            isReload = false;
        }
    }

    //자동,재장전 활성화
    IEnumerator ReloadCoroutine()
    {
        if (currentGun.carryBulletCount > 0)//여유총알 있으면
        {
            isReload = true;
            WeaponManager.currentWeaponAnim.SetTrigger("Gun_Reload");

            //(수동으로 재장전시)총내부에 총알이 있는도중 재장전시도하면 있던것 carry로빼고 새로 재장전
            currentGun.carryBulletCount += currentGun.currentBulletCount;
            currentGun.currentBulletCount=0;

            yield return new WaitForSeconds(currentGun.reloadTime);

            if (currentGun.carryBulletCount >= currentGun.reloadBulletCount)
            {
                currentGun.currentBulletCount = currentGun.reloadBulletCount;
                currentGun.carryBulletCount -= currentGun.reloadBulletCount; //현재 소유개수 재장전한만큼 뺀다
            }
            else
            {
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;
            }

            isReload = false;
        }
        else
        {
            Debug.Log("총알이 없는걸,,");
        }
    }

    //정조준 시도(우클릭으로 이루어지는) 
    private void TryFineSight()
    {
        if (Input.GetButtonDown("Fire2")&&!isReload)//재장전중이 아닐경우에만 정조준가능(코루틴 겹치는거 막기위해)
        {
            FineSight();
        }
    }

    //정조준상태에서 재장전하면 정조준 풀리도록
    //->정조준상태가 아닐때만 재장전 가능하도록,TryReload()시작부분에 넣는다,WeaponManager에서도 호출하도록 public으로 바꿈
    public void CancelFineSightMode()
    {
        if (isFineSightMode == true)
            FineSight();//한번 더이뤄지면 FineSightMode가 false로 바뀌는것
    }

    private void FineSight()//정조준로직가동
    {
        isFineSightMode = !isFineSightMode;
        WeaponManager.currentWeaponAnim.SetBool("Gun_FineSightMode", isFineSightMode);
        if (isFineSightMode == true)
        {   //Lerp를 사용하면 완전히 보간되지 않아서 while문 못빠져나가고 activate와 deactivate코루틴
            //동시에 진행되서 계속 움직이고 정조준 잘 안되는것 막기위해 StopAllCoroutines()
            StopAllCoroutines();
            StartCoroutine(FineSightActivateCoroutine());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FineSightDeactivateCoroutine());
        }
    }
    
    IEnumerator FineSightActivateCoroutine()//정조준 활성화
    {   //현재 총의 위치가 정조준할 때의 위치가 아닌경우 될때까지 반복
        while (currentGun.transform.localPosition != currentGun.fineSigntOriginPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSigntOriginPos,0.2f);
            yield return null;
        }
    }
    
    IEnumerator FineSightDeactivateCoroutine()//정조준 비활성화
    {   //현재 총의 위치가 원래 위치가 아닌경우 될때까지 반복
        while (currentGun.transform.localPosition != originPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition,originPos, 0.2f);
            yield return null;
        }
    }

    //Shoot()안에서 호출되는 반동코루틴
    IEnumerator RetroActionCoroutine()
    {
        Vector3 recoilBack=new Vector3(currentGun.retroActionForce,originPos.y,originPos.z);           //정조준안했을시 최대반동
        Vector3 retroActionRecoilBack = new Vector3(currentGun.retroActionFineSightForce,currentGun.fineSigntOriginPos.y, currentGun.fineSigntOriginPos.z);//정조준 했을시 최대반동
        //GunHolder가 90도 돌아가 있으므로 (z값 대신)x값을 이용해 앞뒤 움직임준다
        if (!isFineSightMode)//정조준상태가 아닐때
        {
            currentGun.transform.localPosition = originPos;//반동후 돌아오는위치
            //반동시작
            while (currentGun.transform.localPosition.x <= currentGun.retroActionForce-0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }
            //원위치
            while (currentGun.transform.localPosition != originPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
                yield return null;
            }
        }
        else//정조준상태일 때
        {
            currentGun.transform.localPosition = currentGun.fineSigntOriginPos;//반동후 돌아오는위치-다시 정조준
            //반동시작
            while (currentGun.transform.localPosition.x <= currentGun.retroActionFineSightForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null;
            }
            //원위치
            while (currentGun.transform.localPosition != currentGun.fineSigntOriginPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSigntOriginPos, 0.1f);
                yield return null;
            }
        }
    }

    //사운드재생
    private void PlaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    //WeaponManager에서 사용되는 것들,GetFineSigntMode()<-이거 뭐이ㅑ
    public Gun GetGun()
    {
        return currentGun;
    }

    public void GunChange(Gun _gun)
    {
        if (WeaponManager.currentWeapon != null)//이미 무언가를 들고 있는 모습
            WeaponManager.currentWeapon.gameObject.SetActive(false);
       
        currentGun = _gun;
        WeaponManager.currentWeapon = currentGun.GetComponent<Transform>();//currentWeapon은 Hand나 Gun둘 다 올 수 있도록 transform형,끄고 킬수 있도록
        WeaponManager.currentWeaponAnim.SetTrigger("Gun_Weapon_In");

        currentGun.transform.localPosition = Vector3.zero;
        currentGun.gameObject.SetActive(true);
        isActivate = true;

    }
}
