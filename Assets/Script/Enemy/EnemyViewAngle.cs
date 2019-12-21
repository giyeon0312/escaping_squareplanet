using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyViewAngle : MonoBehaviour
{

    [SerializeField] private float viewAngle; // 시야각 (120도);
    [SerializeField] private float viewDistance; // 시야거리 (10미터);
    [SerializeField] private LayerMask targetMask; // 타겟 마스크 (플레이어)

    private EnemyController theEnemy;

    void Start()
    {
        theEnemy = GetComponent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        View();
    }

    private Vector3 BoundaryAngle(float _angle)
    {
        _angle += transform.eulerAngles.y;//y축을 중심으로 회전했을때 z축이 바라보는 좌표를 구한다
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));//각도로 구한 좌표 
    }

    private void View()
    {
        Vector3 _leftBoundary = BoundaryAngle(-viewAngle * 0.5f);
        Vector3 _rightBoundary = BoundaryAngle(viewAngle * 0.5f);

        Debug.DrawRay(transform.position + transform.up, _leftBoundary, Color.red);//씬뷰에서만 보이는 디버깅용 레이져
        Debug.DrawRay(transform.position + transform.up, _rightBoundary, Color.red);

        //일정 반경내에있는 것들을 _target에 저장한다
        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance, targetMask);

        for (int i = 0; i < _target.Length; i++)
        {
            Transform _targetTf = _target[i].transform;
            if (_targetTf.name == "Player")
            {

                Vector3 _direction = (_targetTf.position - transform.position).normalized;//플레이어와 에너미 사이의 거리
                float _angle = Vector3.Angle(_direction, transform.forward);//플레이어와 에너미 사이의 각도

                if (_angle < viewAngle * 0.5f)//시야 내에 있으면
                {
                    RaycastHit _hit;
                    if (Physics.Raycast(transform.position - transform.up, _direction, out _hit, viewDistance))//약간 아래로 raycast를 쏴서
                    {
                        if (_hit.transform.name == "Player")
                        {
                            Debug.Log("플레이어가 에너미의 시야 내에 있습니다");
                            Debug.DrawRay(transform.position - transform.up, _direction, Color.blue);
                            theEnemy.direction = Quaternion.LookRotation(_targetTf.position).eulerAngles;
                            theEnemy.Attack();
                        }
                    }
                }

            }
        }
    }
}
