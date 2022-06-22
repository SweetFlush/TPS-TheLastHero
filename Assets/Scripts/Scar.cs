using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Scar : AbstractGun
{
    public Transform bulletPrefab;
    public Transform spawnBulletPosition;
    public WFX_LightFlicker muzzleLight;
    public MultiAimConstraint gunAimConstraint;
    public TwoBoneIKConstraint secondHandConstraint;
    public AudioClip shootSound;
    public ParticleSystem muzzleFlash;

    public int damage = 20;
    public int currentBullet = 30;
    public int maxBullet = 30;
    public float fireRate = 0.1f;

    private void Start()
    {
        //fireRate = 0.1f;
        //damage = 20;
        //currentBullet = 30;
        //maxBullet = 30;
    }

    public override int GetCurrentBullet()
    {
        return currentBullet;
    }

    public override int GetMaxBullet()
    {
        return maxBullet;
    }

    public override int GetDamage()
    {
        return damage;
    }

    public override Transform GetBulletPrefab()
    {
        return bulletPrefab;
    }

    public override float GetFireRate()
    {
        return fireRate;
    }

    public override Transform GetMuzzlePosition()
    {
        return spawnBulletPosition;
    }

    public override MultiAimConstraint GetGunAimConstraint()
    {
        return gunAimConstraint;
    }

    public override TwoBoneIKConstraint GetSecondHandConstraint()
    {
        return secondHandConstraint;
    }

    public override WFX_LightFlicker GetFlashLight()
    {
        return muzzleLight;
    }

    public override ParticleSystem GetMuzzleFlash()
    {
        return muzzleFlash;
    }

    public override AudioClip GetShootSound()
    {
        return shootSound;
    }

    public override void SetCurrentBullet(int cb)
    {
        currentBullet = cb;
    }
}
