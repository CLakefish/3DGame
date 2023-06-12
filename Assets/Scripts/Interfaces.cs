/*
Author: Preston Campbell
Date: 6/1/2023
Desc: Unused interace for health
*/

public interface IHealth
{
    public bool isInvulnerable { get; set; }
    public int health { get; set; }
    public int maxHealth { get; set; }
    void Hit(int damage);
}
