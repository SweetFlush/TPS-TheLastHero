using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private Rig aimRig;
    [SerializeField] private CinemachineVirtualCamera animVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;

    [SerializeField] private Transform bulletPrefab;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private WFX_LightFlicker muzzleLight;
    [SerializeField] private MultiAimConstraint gunAimConstraint;
    [SerializeField] private TwoBoneIKConstraint secondHandConstraint;

    public Transform respawnPoint;

    public int health = 100;
    public bool powerfulMode;

    public AudioClip ShootSound;
    public ParticleSystem muzzleFlash;

    private Animator anim;
    private AudioSource audioSource;
    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;

    private bool isAim = false;
    public bool isReloading = false;

    private float aimRigWeight = 0;
    private float instantShootTimer = 0f;

    private float fireTimer = 0f;
    private float fireRate = 0.1f;

    public int currentBullet = 30;
    public int maxBullet = 30;

    public GameObject[] guns;   //0 AR 1 MachineGun 2 RocketLauncher
    public int gunNumber = 0;

    public int unlocked = 0;    //1. MG 2. Launcher 3. Sniper
    public bool isDead = false;

    private Vector3 mouseWorldPosition;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        gunNumber = 0;
        guns[0].SetActive(true);
        guns[1].SetActive(false);
        guns[2].SetActive(false);
        guns[3].SetActive(false);

        SetWeapon(0);

        if (powerfulMode)
            unlocked = 3;
    }

    private void Start()
    {
        isDead = false;
    }

    private void Update()
    {
        if(!isDead)
        {
            PositionMouseCursor();
            ManageCamera();

            anim.SetBool("isAim", isAim);

            //조준 시 무기 Constraint 설정
            if (isAim)
            {
                gunAimConstraint.weight = 1f;
            }
            else
            {
                gunAimConstraint.weight = 0f;
            }

            CheckInputs();

            if (!isReloading)
                aimRig.weight = Mathf.Lerp(aimRig.weight, aimRigWeight, Time.deltaTime * 20f);
            else
                aimRig.weight = 0f;

            if (fireTimer < fireRate)
            {
                fireTimer += Time.deltaTime;
            }
        }        
     
    }

    private void CheckInputs()
    {
        //발사
        if (starterAssetsInputs.shoot)
        {
            Shoot();
        }
        //재장전
        if (starterAssetsInputs.reload)
        {
            starterAssetsInputs.reload = false;
            if (isReloading && currentBullet == 30) return;
            DoReload();
        }

        if(starterAssetsInputs.ar)
        {
            starterAssetsInputs.ar = false;
            if(gunNumber != 0)
            {
                saveWeaponBullet(gunNumber);

                guns[0].SetActive(true);
                guns[1].SetActive(false);
                guns[2].SetActive(false);
                guns[3].SetActive(false);

                SetWeapon(0);

                anim.SetBool("isAR", true);
                anim.SetBool("isM249", false);
                anim.SetBool("isLauncher",false);

                audioSource.volume = 0.2f;
            }
        }

        if (starterAssetsInputs.machineGun && unlocked >= 1)
        {
            starterAssetsInputs.machineGun = false;
            if (gunNumber != 1)
            {
                saveWeaponBullet(gunNumber);

                guns[0].SetActive(false);
                guns[1].SetActive(true);
                guns[2].SetActive(false);
                guns[3].SetActive(false);

                SetWeapon(1);

                anim.SetBool("isAR", false);
                anim.SetBool("isM249", true);
                anim.SetBool("isLauncher", false);

                audioSource.volume = 0.4f;
            }
        }

        if (starterAssetsInputs.launcher && unlocked >= 2)
        {
            starterAssetsInputs.launcher = false;
            if (gunNumber != 2)
            {
                saveWeaponBullet(gunNumber);

                guns[0].SetActive(false);
                guns[1].SetActive(false);
                guns[2].SetActive(true);
                guns[3].SetActive(false);

                SetWeapon(2);

                anim.SetBool("isAR", false);
                anim.SetBool("isM249", false);
                anim.SetBool("isLauncher", true);

                audioSource.volume = 0.5f;
            }
        }

        if (starterAssetsInputs.barrett && unlocked >= 3)
        {
            starterAssetsInputs.barrett = false;
            if (gunNumber != 3)
            {
                saveWeaponBullet(gunNumber);

                guns[0].SetActive(false);
                guns[1].SetActive(false);
                guns[2].SetActive(false);
                guns[3].SetActive(true);

                SetWeapon(3);

                anim.SetBool("isAR", true);
                anim.SetBool("isM249", false);
                anim.SetBool("isLauncher", false);

                audioSource.volume = 0.4f;
            }
        }
    }

    private void PositionMouseCursor()
    {
        mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

        //에임이 향한 곳에 물체가 찍히는지 테스트
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }


    }

    private void ManageCamera()
    {
        //우클릭하면 에임카메라 전환
        if (starterAssetsInputs.aim)
        {
            animVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            if (gunNumber == 2)
                secondHandConstraint.weight = 1f;

            //재장전 중이면 rigWeight 0.
            if (isReloading)
                aimRigWeight = 0f;
            else
                aimRigWeight = 1f;

            //에임이 향한 곳 바라보기
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);

            isAim = true;
        }
        //평상 시 카메라
        else
        {
            animVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(true);

            if (gunNumber == 2)
                aimRig.weight = 0f;

            //비조준 사격 시 잠깐동안만 isAim 활성화
            if (instantShootTimer < .5f)
            {
                instantShootTimer += Time.deltaTime;
                isAim = true;

                if (!isReloading)
                    aimRigWeight = 1f;
            }
            //비조준 사격이 끝났으면
            else
            {
                aimRigWeight = 0f;
                isAim = false;
            }
        }
    }

    private void Shoot()
    {
        //공속을 만족하지 않았거나, 총알이 없으면 발사불가
        if (fireTimer < fireRate || currentBullet <= 0 || isReloading) return;

        Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
        Instantiate(bulletPrefab, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));

        if (!isAim)
            transform.LookAt(mouseWorldPosition);

        instantShootTimer = 0f;
        fireTimer = 0f;
        currentBullet -= 1;

        switch(gunNumber)
        {
            case 0:
                anim.CrossFadeInFixedTime("Shoot", 0.1f);
                break;
            case 1:
                anim.CrossFadeInFixedTime("MachineGunShoot", 0.1f);
                break;
            case 2:
                anim.CrossFadeInFixedTime("LauncherShoot", 0.1f);
                break;
            case 3:
                anim.CrossFadeInFixedTime("Shoot", 0.1f);
                break;
        }
        audioSource.PlayOneShot(ShootSound);
        muzzleFlash.Play();
        if(muzzleLight)
            muzzleLight.LightCoroutine();
    }

    private void DoReload()
    {
        isReloading = true;
        anim.SetTrigger("doReload");
    }

    public void ReloadComplete()
    {
        isReloading = false;
        currentBullet = maxBullet;
    }

    //총기 바꾸면 총기의 프로퍼티 값 모두 가져옴
    public void SetWeapon(int i)
    {
        AbstractGun absgun = guns[i].GetComponent<AbstractGun>();

        currentBullet = absgun.GetCurrentBullet();
        maxBullet = absgun.GetMaxBullet();
        fireRate = absgun.GetFireRate();
        spawnBulletPosition = absgun.GetMuzzlePosition();
        gunAimConstraint = absgun.GetGunAimConstraint();
        secondHandConstraint = absgun.GetSecondHandConstraint();
        bulletPrefab = absgun.GetBulletPrefab();
        muzzleFlash = absgun.GetMuzzleFlash();
        muzzleLight = absgun.GetFlashLight();
        ShootSound = absgun.GetShootSound();

        gunNumber = i;
        isReloading = false;
    }

    //총알 썼던 만큼 저장되는 함수
    public void saveWeaponBullet(int i)
    {
        AbstractGun absgun = guns[i].GetComponent<AbstractGun>();
        absgun.SetCurrentBullet(currentBullet);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0 && !isDead)
            Dead();
    }

    public void Dead()
    {
        isDead = true;
        anim.SetBool("isDead", isDead);
        anim.CrossFadeInFixedTime("Death", 0.1f);
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f);
        transform.position = respawnPoint.position;
        isDead = false;
        health = 100;
        anim.SetBool("isDead", isDead);
    }
}
