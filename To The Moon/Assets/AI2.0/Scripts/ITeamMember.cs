using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITeamMember
{
    public int getTeamNumber();

    public bool canHaveFollowers(GameObject follower, out int index);

    public bool isAttackable(out AttackPath path, out int index);

    public void returnAttackIndex(int index);

    public void returnFollowIndex(int index);

    public void TakeDamage(float damage, GameObject shooter);
}
