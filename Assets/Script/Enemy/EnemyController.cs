using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private string enemeyName; //에너미 이름
    [SerializeField] private int hp; // 에너미 체력.

    [SerializeField] private float walkSpeed; // 걷기 스피드.

    public Vector3 direction; // 방향-맞았을경우 반대방향으로&EnemyVeiwAngle에서 Player가 시야각에 들어왔을때 Player쪽으로 향하도록


    // 상태변수
    private bool isAction; // 행동중인지 아닌지 판별
    private bool isWalking; // 걷는지 안 걷는지 판별
    private bool isDead;    //죽었는지 판별

    [SerializeField] private float walkTime; // 걷는 시간
    [SerializeField] private float waitTime; // 대기 시간.
    private float currentTime;


    // 필요한 컴포넌트
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private MeshCollider col;
    private StatusController theStatusController;//공격시 hp를 깎기 위해서
    private AudioSource audioSource;
    [SerializeField] private AudioClip sound_dying;
    [SerializeField] private AudioClip sound_raser_shooting;


    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentTime = waitTime;//currentTime을 이용해 행동을 반복한다
        isAction = true;
        isDead = false;
        theStatusController = FindObjectOfType<StatusController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            Move();     //RandomAction()에서 TryWalk()가 호출되면 isWalking이 True되서 작동한다
            Rotation(); 
            ElapseTime();
        }
       
    }

    private void Move()
    {
        if (isWalking)
        {
            rigid.MovePosition(transform.position + (transform.forward * walkSpeed * Time.deltaTime));
            //this.transform.position = Vector3.MoveTowards(transform.position,transform.forward,walkSpeed);
            //Debug.Log("걷기");
        }
            
    }

    private void Rotation()
    {
        if (isWalking)
        {
            Vector3 _rotation = Vector3.Lerp(transform.eulerAngles, new Vector3(0f, direction.y, 0f), 0.01f);//direction값은 ReSet()과 Damage()에서 변경된다
            rigid.MoveRotation(Quaternion.Euler(_rotation));
        }
    }

    private void ElapseTime()
    {
        if (isAction)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
                ReSet();
        }
    }

    //에너미의 시야에 있으면 호출된다
    public void Attack()
    {
        //player의 hp깎기,에너미의 에니메이터호출
        //PlaySE(sound_raser_shooting);
        anim.SetTrigger("Alien_Shoot");
        theStatusController.DecreaseHP(0.1f);
    }


    private void ReSet()
    {
        isWalking = false; isAction = true;
        anim.SetBool("Alien_Walking", isWalking);//종전에 걸었다면 걷기를 멈춘다
        direction.Set(0f, Random.Range(0f, 360f), 0f);//회전을 위해 랜덤으로 y축 값 얻는다
        RandomAction();
    }

    private void RandomAction()
    {
        int _random = Random.Range(0, 3); // 대기, 돌기, 걷기.

        if (_random == 0)
            Wait();
        else if (_random == 1)
            Turn();
        else if (_random == 2)
            TryWalk();
    }

    private void Wait()
    {
        currentTime = waitTime;
        Debug.Log("대기");
    }

    private void Turn()
    {
        currentTime = waitTime;
        anim.SetTrigger("Alien_Turn");
        Debug.Log("돌기");
    }

    private void TryWalk()
    {
        isWalking = true;
        anim.SetBool("Alien_Walking", isWalking);
        currentTime = walkTime;
        //Debug.Log("걷기");
    }

    //Damage()는 PickaxeController나 HandController,GunController에서 호출된다
    public void Damage(int _dmg, Vector3 _targetPos)
    {
        if (!isDead)
        {
            hp -= _dmg;

            if (hp <= 0)//에너미의 체력이 0이하면
            {
                Dying();
                return;
            }
            direction = Quaternion.LookRotation(transform.position - _targetPos).eulerAngles;//Roatation에 있는 direction을 바꿔준다,공격받은쪽 반대쪽으로
            anim.SetBool("Alien_Hurt_Walking", true);
            isWalking = true;
        }
    }

    private  void Dying()
    {
        PlaySE(sound_dying);
        isWalking = false;
        isDead = true;
        anim.SetBool("Alien_Hurt_Walking", false);
        anim.SetBool("Alien_Dead",true);
        Invoke("Dead", 3.0f);
    }

    private void Dead()
    {
        this.GetComponent<Transform>().gameObject.SetActive(false);
    }

    //클립여러개일때 
    private void PlaySE(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play(); 
    }
}


