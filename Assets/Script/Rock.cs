using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField]
    private int hp; // 바위의 체력.

    [SerializeField]
    private float destroyTime; // 파편 제거 시간

    [SerializeField]
    private SphereCollider col; // 구체 컬라이더,파괴되고 없어지도록


    // 필요한 게임 오브젝트
    [SerializeField]
    private GameObject go_rock; // 일반 바위-파괴되면 없어진다
    [SerializeField]
    private GameObject go_debris; // 깨진 바위-파괴되면 생긴다
    [SerializeField]
    private GameObject go_effect_prefabs; // 채굴 이펙트.
    [SerializeField]
    private GameObject what_to_get_prefab;//채굴하고 얻게 되는 프리팹

    //필요한 사운드 이름
    [SerializeField]
    private string strike_Sound;
    [SerializeField]
    private string destroy_Sound;

    //곡괭이로 찍으면 호출되므로 PickaxeController에서 호출된다
    public void Mining()
    {
        SoundManager.instance.PlaySE(strike_Sound);
        var clone = Instantiate(go_effect_prefabs, col.bounds.center, Quaternion.identity);//go_effect_prefabs파편을 collider의 중심에서 만들어냄다
        Destroy(clone, destroyTime);

        hp--;

        if (hp <= 0)
            Destruction();
    }

    private void Destruction()
    {

        SoundManager.instance.PlaySE(destroy_Sound);

        col.enabled = false;
        Instantiate(what_to_get_prefab,transform.position,Quaternion.identity);//바위안에있던 프리팹 생김
        Destroy(go_rock);		//바로 없애고

        go_debris.SetActive(true);
        Destroy(go_debris, destroyTime);//일정시간 있다가 없앤다
    }

}


