using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Alter : Interactalble
{
    private float minDistance = 5.0f;
    private float maxDistance = 10.0f;
    public List<EnemyGroup> spawnList = new List<EnemyGroup>();
    private Transform enemyGroup;
    private bool isActivaed = false;
    public GameObject treasureChest;
    Rigidbody rb;
    protected override void Awake()
    {
        base.Awake();
        enemyGroup = transform.Find("EnemyGroup");
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (isActivaed)
        {
            if (enemyGroup.childCount == 0)
            {
                SpawnChest();
            }
        }
    }

    public override void Interact(Transform player)
    {
        while (spawnList.Count != 0)
        {
            if (spawnList[0].enemyCount == 0){ 
                spawnList.RemoveAt(0);
                continue;
            }
            Vector3 spawnPosition = Random.insideUnitCircle * Random.Range(minDistance, maxDistance);
            NavMeshHit hit;
            int attemp = 0;
            while (!NavMesh.SamplePosition(transform.position + spawnPosition, out hit, spawnList[0].enemyType.GetComponent<NavMeshAgent>().height * 5, 1) && attemp < 10)
            {
                attemp++;
            }

            if (attemp == 10)
            {
                Debug.Log("Cant place enemy on NavMesh Surface");
                spawnList[0].enemyCount--;
                continue;
            }

            NavMeshAgent newEnemy;
            newEnemy = Instantiate(spawnList[0].enemyType, transform.position + spawnPosition, spawnList[0].enemyType.transform.rotation, enemyGroup).GetComponent<NavMeshAgent>();
            newEnemy.Warp(hit.position);
            spawnList[0].enemyCount--;
        }
        isActivaed = true;
    }

    protected override void SetHoverText()
    {
        hoverText = "Active alter";
    }

    private void SpawnChest()
    {
        Instantiate(treasureChest, transform.position, Quaternion.identity, transform.parent);
        Destroy(gameObject);
    }
}

[System.Serializable]
public class EnemyGroup
{
    public GameObject enemyType;
    public int enemyCount;
}