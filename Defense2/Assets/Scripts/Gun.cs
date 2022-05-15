using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

// 총을 구현
public class Gun : MonoBehaviour
{
    // 총의 상태를 표현하는 열거형
    public enum GunState
    {
        Ready,      // 발사 준비됨
        Empty,      // 탄창이 빔
        Reloading   // 재장전 중
    }

    public GunState gunState { get; private set; }  // 현재 총의 상태
    
    public Transform fireTransform;                 // 탄알이 발사될 위치
    
    public ParticleSystem muzzleFlashEffect;        // 총구 화염 효과
    public ParticleSystem shellEjectEffect;         // 탄피 배출 효과

    private LineRenderer bulletLineRenderer;    // 탄알 궤적을 그리기 위한 렌더러
    
    private AudioSource gunAudioPlayer;         // 총 소리 재생기
    public AudioClip shotClip;                  // 발사 소리
    public AudioClip reloadClip;                // 재장전 소리

    public float damage = 25;           // 공격력
    private float fireDistance = 50f;   // 사정거리

    public int ammoRemain = 100;    // 남은 전체 탄알
    public int magCapacity = 25;    // 탄창 용량
    public int magAmmo;             // 현재 탄창에 남아 있는 탄알

    public float timeBetFire = 0.12f;   // 탄알 발사 간격
    public float reloadTime = 1.8f;     // 재장전 소요 시간
    private float lastFireTime;         // 총을 마지막으로 발사한 시점

    private void Awake()
    {
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();

        bulletLineRenderer.positionCount = 2;   // 라인 렌더러에서 사용할 점을 두 개로 변경
        bulletLineRenderer.enabled = false;

        magAmmo = magCapacity;  // 현재 탄창을 가득 채우기
        gunState = GunState.Ready;
        lastFireTime = 0;
    }

    // 발사 시도
    public void Fire()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            // 발사 가능
            if (gunState == GunState.Ready && Time.time >= lastFireTime + timeBetFire)
            {
                lastFireTime = Time.time;   // 마지막 발사 시간을 현재 시간으로 갱신
                Shot(); // 실제 발사 처리 진행
            }
        }
    }

    // 실제 발사 처리
    private void Shot()
    {
        RaycastHit hit;    // 레이캐스트에 의한 충돌 정보를 저장하는 변수
        Vector3 hitPosition = Vector3.zero; // 탄알이 맞은 곳을 저장할 변수

        // 레이캐스트(시작 지점, 방향, 충돌 정보 컨테이너, 사정거리)
        // 레이가 어떤 물체와 충돌한 경우
        if (Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance))
        {
            // 상대방으로부터 IDamageable 오브젝트 가져오기 시도
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null)
            {
                target.OnDamage(damage, hit.point, hit.normal);
            }
            hitPosition = hit.point;    // 레이가 충돌한 위치 저장
        }
        else // 레이가 충돌하지 않았다면
        {
            // 최대 사정거리까지 날아갔을 때의 위치를 충돌 위치로 사용
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
        }

        StartCoroutine(ShotEffect(hitPosition));    // 발사 이펙트 재생을 위한 코루틴 실행
        
        magAmmo--;
        if (magAmmo <= 0)
        {
            gunState = GunState.Empty;
        }
    }

    // 발사 이펙트와 소리를 재생하고 탄알 궤적을 그림
    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        muzzleFlashEffect.Play();   // 특수효과 재생
        shellEjectEffect.Play();
        gunAudioPlayer.PlayOneShot(shotClip);   // 총격 소리 재생

        // 총구 -> 사정거리 끝 혹은 맞은 위치로 라인을 그린다
        bulletLineRenderer.SetPosition(0, fireTransform.position);
        bulletLineRenderer.SetPosition(1, hitPosition);

        bulletLineRenderer.enabled = true;      // 탄알 궤적 활성화
        yield return new WaitForSeconds(0.03f); // 대기
        bulletLineRenderer.enabled = false;     // 탄알 궤적 비활성화
    }

    // 재장전 시도
    public bool Reload()
    {
        // 재장전 중이거나, 남은 탄알이 없거나, 탄창이 가득한 경우 재장전 불가능
        if (gunState == GunState.Reloading || ammoRemain <= 0 || magAmmo >= magCapacity)
        {
            return false;
        }
        
        // 재장전 처리 시작
        StartCoroutine(ReloadRoutine());
        return true;
    }

    // 실제 재장전 처리를 진행
    private IEnumerator ReloadRoutine()
    {
        gunState = GunState.Reloading;
        gunAudioPlayer.PlayOneShot(reloadClip);

        yield return new WaitForSeconds(reloadTime);

        int ammoToFill = magCapacity - magAmmo;

        // 탄창에 채워야 할 탄알이 남은 탄알보다 많은 경우
        // 채워야 할 탄알 수를 남은 탄알 수에 맞춰줄임
        if (ammoRemain < ammoToFill)
        {
            ammoToFill = ammoRemain;
        }

        magAmmo += ammoToFill;      // 탄창을 채움
        ammoRemain -= ammoToFill;    // 남은 탄알에서 탄창에 채운만큼 뺌

        gunState = GunState.Ready;
    }

    public void AddAmmo(int pAmmo)
    {
        ammoRemain += pAmmo;
    }
}
