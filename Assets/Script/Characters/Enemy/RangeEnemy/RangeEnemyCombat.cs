using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemyCombat : EnemyCombat
{
    public GameObject arrow;
    protected override IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(attackStartUpDuration);
        yield return new WaitForSeconds(attackRecoveryDuration);
        characterControl.StopAttack();
    }

    public void SpawnArrow()
    {
        GameObject newArrow = Instantiate(arrow, attackPos.transform.position, attackPos.transform.rotation, transform);
        Vector3 direction = (target.position - attackPos.transform.position).normalized;
        newArrow.GetComponent<ArrowController>().SetDirection(new Vector2(direction.x, direction.z));
    }
}
