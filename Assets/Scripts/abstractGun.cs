using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public abstract class AbstractGun : MonoBehaviour
{
    public abstract int GetDamage();

    public abstract int GetCurrentBullet();
    public abstract int GetMaxBullet();

    public abstract float GetFireRate();

    public abstract Transform GetMuzzlePosition();

    public abstract MultiAimConstraint GetGunAimConstraint();

    public abstract TwoBoneIKConstraint GetSecondHandConstraint();

    public abstract Transform GetBulletPrefab();

    public abstract WFX_LightFlicker GetFlashLight();

    public abstract ParticleSystem GetMuzzleFlash();

    public abstract AudioClip GetShootSound();

    public abstract void SetCurrentBullet(int cb);

}
