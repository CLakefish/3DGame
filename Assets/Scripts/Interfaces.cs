using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    public bool isInvulnerable { get; set; }
    public int health { get; set; }
    public int maxHealth { get; set; }
    void Hit(int damage);
}
