using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    private float locomotionSmooth = 0.1f;
    CharacterControl characterControl;
    protected Animator characterAnimator;

    protected virtual void Start()
    {
        characterControl = GetComponent<CharacterControl>();
        characterAnimator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        float speedPercent = UtilFunc.GetHorizontalSpeed(characterControl.velocity) / characterControl.runningSpeed;
        characterAnimator.SetFloat("speedPercent", speedPercent);
    }
}