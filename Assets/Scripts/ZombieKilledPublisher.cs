using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieKilledPublisher : MonoBehaviour
{
    public event EventHandler OnZombieKilled;

    public void KilledZombie()
    {
        OnZombieKilled?.Invoke(this, EventArgs.Empty);
    }
}
