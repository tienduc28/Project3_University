using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterControl
{
    //Moving variables
    private Vector3 move;
    private float moveX, moveY;
    public float speed;

    private float jumpHeight = 1.0f;
    private float dashTimer = 0.2f;
    private bool isDashing = false;
    private float dashingSpeed = 40.0f;
    private bool isRunning = false;

    //Stamina variables
    public float maxStamina
    {  get; private set; }
    public float stamina
    {  get; private set; }
    private float staminaRecover = 10.0f;
    private float runningStamina = 10.0f;
    private float dashingStamina = 30.0f;

    //Power variables
    private bool isDashUnlocked = false;

    protected override void Start()
    {
        base.Start();
        speed = walkingSpeed;
        maxStamina = characterCombat.myStats.statData.maxStamina.GetValue();
        stamina = maxStamina;
    }

    protected override void Update()
    {
        if (((PlayerCombat)characterCombat).isDead) return;
        base.Update();

        //Move
        if (!isDashing)
            move = Vector3.zero;

        //Get Horizontal input
        if (!InventoryUI.instance.isInventoryOpen && !isDashing)
        {
            moveX = Input.GetAxis("Horizontal");
            moveY = Input.GetAxis("Vertical");

            move = transform.right * moveX + transform.forward * moveY;

            if (Input.GetKeyDown(KeyCode.LeftShift) && stamina > 0)
            {
                speed = runningSpeed;
                isRunning = true;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                speed = walkingSpeed;
                isRunning = false;
            }
            if (Input.GetKeyDown(KeyCode.R) && isDashUnlocked && stamina >= dashingStamina)
                Dash();
        }

        Vector3.Normalize(move);

        //Fall and Jump
        if (isGround && Input.GetKey(KeyCode.Space))
            yVelocity = - Mathf.Sqrt(2 * gravity * jumpHeight);

        if (isDashing)
        {
            yVelocity = 0;
            if (move == Vector3.zero)
                move = transform.forward;
            controller.Move(move * Time.deltaTime * dashingSpeed);
            //Debug.Log("Dashing in direction " + move);
        }

        //Knockback check
        if (!isKnockBack && !isDashing)
            controller.Move(move * Time.deltaTime * speed + Vector3.down * yVelocity * Time.deltaTime);

        if (!isRunning && !isDashing)
        {
            //Debug.Log("Reconvering stamina");
            if (stamina < maxStamina)
                stamina += staminaRecover * Time.deltaTime;
            else
                stamina = maxStamina;
        }
        else if (isRunning)
            stamina -= runningStamina * Time.deltaTime;
    }

    private void Dash()
    {
        Debug.Log("Player dashes");
        isDashing = true;
        stamina -= dashingStamina;
        StartCoroutine(DashCoutDownCouroutine()); 
    }

    private IEnumerator DashCoutDownCouroutine()
    {
        yield return new WaitForSeconds(dashTimer);
        isDashing = false;
    }

    public void UnlockDash()
    {
        isDashUnlocked = true;
    }
}
