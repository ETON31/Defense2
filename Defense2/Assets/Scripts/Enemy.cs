using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : LivingEntity 
{
    // 에너미 상태 상수
    enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Damaged,
        Die
    }

    public float attackDistance = 1f;    // 공격 가능 범위
    public float moveSpeed = 3f;         // 이동 속도
    public int damage = 3;          // 에너미의 공격력
    public int hp = 100;                  // 에너미의 체력
    public float findDistance = 100f;    // 플레이어 발견 범위

    private EnemyState m_State; // 에너미 상태 변수
    private Transform playerTransform;   // 플레이어 트랜스폼
    private CharacterController playerCC;
    private CharacterController characterController;    // 캐릭터 컨트롤러 컴포넌트
    private Animator enemyAnimator;  // 애니메이터 변수
    private AudioSource enemyAudioPlayer; // 오디오 소스 컴포넌트
    private LivingEntity playerEntity;

    public GameObject[] items; // 생성할 아이템들

    private AudioSource zombieAudioPlayer;
    public AudioClip hitClip;  
    public AudioClip deathClip;
/*
    public ParticleSystem hitEffect; // 피격시 재생할 파티클 효과
    public AudioClip hitSound; // 피격시 재생할 소리
    public AudioClip deathSound; // 사망시 재생할 소리
*/
    float currentTime = 0;  // 누적 시간
    float attackDelay = 2f; // 공격 딜레이 시간

    private void Awake()
    {
        playerTransform = GameObject.Find("Player").transform;   // 플레이어의 트랜스폼 컴포넌트를 받아온다.
        playerCC = GameObject.Find("Player").GetComponent<CharacterController>();
        characterController = GetComponent<CharacterController>();  // 캐릭터 컨트롤러 컴포넌트를 받아온다.
        enemyAnimator = GetComponent<Animator>();    // 애니메이터 변수를 받아온다.
        playerEntity = GameObject.Find("Player").GetComponent<LivingEntity>();
        zombieAudioPlayer = GetComponent<AudioSource>();
    }

    private void Start()
    {
        m_State = EnemyState.Idle;  // 최초의 에너미 상태는 대기(Idle)로 한다.  
    }

    private void Update()
    {
        switch(m_State)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Damaged:
                break;
            case EnemyState.Die:
                break;
        }
    }
//--------------------------------------------------------------------------------------------------
    private void Idle()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) < findDistance)
        {
            m_State = EnemyState.Move;
            enemyAnimator.SetTrigger("IdleToMove");  // 이동 애니메이션으로 전환하기
        }
    }
//--------------------------------------------------------------------------------------------------
    private void Move()
    {
        // 이동 방향 설정 후 이동
        if (Vector3.Distance(transform.position, playerTransform.position) > attackDistance)
        {
            StartCoroutine(UpdatePath());
        }
        else
        {
            m_State = EnemyState.Attack;
       
            currentTime = attackDelay;  // 누적 시간을 공격 딜레이 시간만큼 미리 진행시켜 놓는다.
            enemyAnimator.SetTrigger("MoveToAttackDelay");   // 공격 대기 애니메이션 플레이
        }
    }

    private IEnumerator UpdatePath()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        characterController.Move(direction * moveSpeed * Time.deltaTime);
            
        transform.forward = direction;  // 플레이어를 향해 방향을 전환한다.

        yield return new WaitForSeconds(0.25f);
    }
//--------------------------------------------------------------------------------------------------
    private void Attack()
    {
        // 플레이어가 공격 범위 이내에 있다면 플레이어를 공격한다.
        if (Vector3.Distance(transform.position, playerTransform.position) < attackDistance)
        {
            currentTime += Time.deltaTime;
            if (currentTime > attackDelay)
            {
                enemyAnimator.SetTrigger("StartAttack"); // 공격 애니메이션 플레이
                currentTime = 0;
            }
        }
        // 그렇지 않다면, 현재 상태를 이동(Move)으로 전환한다.
        else
        {
            m_State = EnemyState.Move;
            enemyAnimator.SetTrigger("AttackToMove");    // 이동 애니메이션 플레이
            currentTime = 0;
        }
    }
    
    // 플레이어 스크립트의 데미지 처리 함수를 실행한다.
    public void AttackAction()
    {
        // 상대방의 피격 위치와 피격 방향을 근삿값으로 계산
        Vector3 hitPoint = playerCC.ClosestPoint(transform.position);
        Vector3 hitNormal = transform.position - playerTransform.position;

        // 공격 실행
        playerEntity.OnDamage(damage, hitPoint, hitNormal);
    }
//--------------------------------------------------------------------------------------------------
    // 데미지를 입었을때 실행할 처리
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal) 
    {
/*
        // 아직 사망하지 않은 경우엠나 피격 효과 재생
        if (!dead)
        {
            // 공격받은 지점과 방향으로 파티클 효과 재생
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();

            // 피격 효과음 재생
            enemyAudioPlayer.PlayOneShot(hitSound);
        }
*/
        if (!dead)
        {
            
        }
        // LivingEntity의 OnDamage()를 실행하여 데미지 적용
        base.OnDamage(damage, hitPoint, hitNormal);
    }

    // 데미지 실행 함수
    public void HitEnemy(int hitPower)
    {
        // 이미 피격 상태나 사망 상태라면 return
        if (m_State == EnemyState.Damaged || m_State == EnemyState.Die)
        {
            return;
        }

        Vector3 hitPoint = characterController.ClosestPoint(playerTransform.position);
        Vector3 hitNormal = playerTransform.position - transform.position;
        OnDamage(hitPower, hitPoint, hitNormal);

        // 에너미의 체력이 0보다 크면 피격 상태로 전환한다.
        if (hp > 0)
        {
            m_State = EnemyState.Damaged;
            enemyAnimator.SetTrigger("Damaged"); // 피격 애니메이션을 플레이한다.
            Damaged();
        }
        // 그렇지 않다면 죽음 상태로 전환한다.
        else
        {
            m_State = EnemyState.Die;
            enemyAnimator.SetTrigger("Die"); // 죽음 애니메이션을 플레이한다.
            Die();
        }
    }

    private void Damaged()
    {
        StartCoroutine(DamageProcess());    // 피격 상태를 처리하기 위한 코루틴을 실행한다.
    }

    // 데미지 처리용 코루틴 함수
    IEnumerator DamageProcess()
    {
        yield return new WaitForSeconds(1.0f);  // 피격 모션 시간만큼 기다린다.
        
        m_State = EnemyState.Move;  // 현재 상태를 이동 상태로 전환한다.
    }
//--------------------------------------------------------------------------------------------------
    // 사망 처리
    public override void Die() 
    {
        // LivingEntity의 Die()를 실행하여 기본 사망 처리 실행
        base.Die();

        // 죽은 자리에 아이템 생성
        Vector3 spawnPosition = gameObject.transform.position;
        GameObject selectedItem = items[Random.Range(0, items.Length)];
        GameObject item = Instantiate(selectedItem, spawnPosition, Quaternion.identity);

        // 다른 AI를 방해하지 않도록 자신의 모든 콜라이더를 비활성화
        Collider[] enemyColliders = GetComponents<Collider>();
        for (int i = 0; i < enemyColliders.Length; i++)
        {
            enemyColliders[i].enabled = false;
        }

        // 사망 애니메이션 재생
        enemyAnimator.SetTrigger("Die");
        // 사망 효과음 재생
        zombieAudioPlayer.PlayOneShot(deathClip);

        // 죽음 상태를 처리하기 위한 코루틴 실행
        StartCoroutine(DieProcess());
    }

    IEnumerator DieProcess()
    {
        characterController.enabled = false; // 캐릭터 컨트롤러 컴포넌트를 비활성화시킨다.
        // 2초 후 소멸
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}
