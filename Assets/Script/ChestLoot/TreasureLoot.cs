using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TreasureLoot : MonoBehaviour
{
    private Vector3 endScale;
    private float LerpTime = 1.5f;
    private float elapsedTime = 0f;
    private void Start()
    {
        endScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (elapsedTime < LerpTime)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, endScale, Mathf.Clamp01(elapsedTime / LerpTime));
        }
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GetLoot(other))
            Destroy(gameObject);
    }

    protected abstract bool GetLoot(Collider other);

}
