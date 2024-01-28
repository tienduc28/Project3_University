using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
public class CharacterCombat : MonoBehaviour
{
    private float knockBackDuration = 0.2f;

    public CharacterStats myStats
    { get; private set; }
    public AttackPos attackPos;
    public string targetTag;

    public CharacterControl characterControl;

    public delegate void OnInitiateAttack();
    public OnInitiateAttack initiateAttackCallback;
    protected float attackImpactDuration = 0.1f;
    protected float attackStartUpDuration = 0.5f;
    protected float attackRecoveryDuration = 1.5f;

    protected virtual void Awake()
    {
        myStats = GetComponent<CharacterStats>();
        attackPos = GetComponentInChildren<AttackPos>();
        characterControl = GetComponent<CharacterControl>();
    }

    protected virtual void Start()
    {
        SetTargetTag();
        myStats.takingDamageCallBack += GetKnockBack;
    }

    protected virtual void Update()
    {
        if (gameObject.transform.position.y < -50)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void GetKnockBack(GameObject source)
    {
        Vector3 generalDirection = this.transform.position - source.transform.position;
        generalDirection = new Vector3(generalDirection.x, 0, generalDirection.z);
        Vector3 knockDirection = Vector3.Normalize(generalDirection);
        float knockDistance = source.GetComponent<CharacterStats>().statData.attackKnockbackDistance.GetValue();
        //Debug.Log(gameObject.name + "got knock back");
        characterControl.KnockBack(knockDirection, knockDistance, knockBackDuration);
    }

    public virtual void DealDamage(CharacterStats target)
    {
        if (target != null)
            target.TakeDamage(gameObject, myStats.statData.damage.GetValue());
    }

    protected void Attack()
    {
        characterControl.StartAttack();
        if (initiateAttackCallback != null)
            initiateAttackCallback();
        StartCoroutine(AttackCoroutine());
    }

    protected virtual IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(attackStartUpDuration);
        //attackPos.GetComponent<Collider>().enabled = true;
        yield return new WaitForSeconds(attackImpactDuration);
        yield return new WaitForSeconds(attackRecoveryDuration);
        attackPos.GetComponent<Collider>().enabled = false;
        characterControl.StopAttack();
    }

    protected virtual void SetTargetTag()
    {
        // Set target tag
    }
}
