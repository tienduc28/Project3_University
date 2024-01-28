using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public CharacterStatData statData;
    public float health 
    { protected set; get; }

    public delegate void OnTakingDamage(GameObject damageSource);
    public OnTakingDamage takingDamageCallBack;
    public delegate void OnDead();
    public OnDead deadCallBack;

    protected virtual void Start()
    {
        health = statData.maxHealth.GetValue();    
    }

    public virtual void TakeDamage(GameObject damageSource, float damageTaken)
    {
        damageTaken = damageTaken * 100 / (statData.armor.GetValue() + 100); //damage caculator

        health -= damageTaken;
        if (health <= 0)
            OutOfHealth();

        Debug.Log(gameObject.name + "takes " + damageTaken + " damage");

        if (takingDamageCallBack != null)
            takingDamageCallBack.Invoke(damageSource);
    }

    public virtual void Heal()
    {
        health = statData.maxHealth.GetValue();
    }


    public virtual void Heal(float amount)
    {
        health += amount;
    }

    public virtual void OutOfHealth()
    {
        //Do something here
        if (deadCallBack != null)
            deadCallBack.Invoke();
    }
}
