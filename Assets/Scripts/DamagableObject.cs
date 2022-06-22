using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableObject : MonoBehaviour
{
    public Zombie zombie;
    public ThirdPersonShooterController player;
    public Civillian civillian;

    private void Start()
    {
        zombie = GetComponent<Zombie>();
        player = GetComponent<ThirdPersonShooterController>();
        civillian = GetComponent<Civillian>();
    }

    public void TakeDamage(int damage)
    {
        if (zombie)
            zombie.TakeDamage(damage);
        if (player)
            player.TakeDamage(damage);
        if (civillian)
            civillian.TakeDamage(damage);
    }
}
