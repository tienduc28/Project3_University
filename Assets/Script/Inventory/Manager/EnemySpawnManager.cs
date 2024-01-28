using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField]
    private Transform enemyGroup;
    private float minDistance = 20.0f;
    private float maxDistance = 25.0f;

    private GameObject player;

    public List<GameObject> spawnList = new List<GameObject>();
    private GameObject enemy;
    private int maxEnemyNum = 10;
    private float enemySpawnTimer;
    private float enemySpawnRateMin = 2.0f;
    private float enemySpawnRateMax = 3.0f;

    void Start()
    {
        player = PlayerManager.instance.player;
    }

    void Update()
    {
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        if (DayNightManager.instance.timePeriod == DayNightManager.TimeOfDay.NightTime)
        {
            if (enemySpawnTimer <= 0)
            {
                if (enemyGroup.childCount <= maxEnemyNum)
                {
                    int enemyToSpawn = Random.Range(0, spawnList.Count);
                    Vector3 spawnPosition = Random.insideUnitCircle * Random.Range(minDistance, maxDistance);
                    NavMeshHit hit;
                    int attemp = 0;
                    while (!NavMesh.SamplePosition(player.transform.position + spawnPosition, out hit, spawnList[enemyToSpawn].GetComponent<NavMeshAgent>().height * 2, 1) && attemp < 10)
                    {
                        attemp++;
                    }

                    if (attemp == 10)
                    {
                        Debug.Log("Cant place enemy on NavMesh Surface");
                        return;
                    }

                    NavMeshAgent newEnemy;
                    newEnemy = Instantiate(spawnList[enemyToSpawn], player.transform.position + spawnPosition, spawnList[enemyToSpawn].transform.rotation, enemyGroup).GetComponent<NavMeshAgent>();
                    newEnemy.Warp(hit.position);
                    enemySpawnTimer = Random.Range(enemySpawnRateMin, enemySpawnRateMax);
                }
            }
            else
            {
                enemySpawnTimer -= Time.deltaTime;
            }
        }
    }
}
