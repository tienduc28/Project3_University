using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArrowController : CharacterControl
{
    private float arrowVelocity = 20.0f;
    private Vector2 direction;
    Vector3 moveVelocity;
    float rotateSpeed = 2.0f;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
        moveVelocity = (arrowVelocity * new Vector3(direction.x, 0, direction.y) + Vector3.down * yVelocity) * Time.deltaTime;
        RotateArrow();
        controller.Move(moveVelocity);
    }

    public void SetDirection(Vector2 direction)
    {
        this.direction = direction;
    }

    void RotateArrow()
    {
        Quaternion rotateDirection = Quaternion.LookRotation(moveVelocity.normalized);
        transform.rotation = rotateDirection;
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotateDirection, rotateSpeed);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Player")
        {
            Debug.Log("Hit " + hit.transform.name);
            CharacterStats target = hit.transform.GetComponentInParent<CharacterStats>();
            characterCombat.DealDamage(target);
        }
        Destroy(gameObject);
    }

    protected override void GroundCheck()
    {
        isGround = false;
    }

}
