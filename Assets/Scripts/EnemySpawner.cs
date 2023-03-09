using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float spawnTime;
    public LayerMask collisionLayers;

    public Transform followTarget;
    [Serializable]
    public class EnemySpawnSettings
    {
        public GameObject enemyPrefab;
        public int chanceOfSpawn;

        public bool GetRandomSpawEnemy()
        {
            int i = UnityEngine.Random.Range(0, 100);
            return i <= chanceOfSpawn;
        }
    }
    [SerializeField]
    public List<EnemySpawnSettings> enemys = new List<EnemySpawnSettings>();
    public int maxPoolSize;
    private List<GameObject> enemysFollowPool = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < maxPoolSize; i++)
        {
            GameObject enemyToSpawn = null;
            while (enemyToSpawn == null)
            {
                int listRandomNumber = UnityEngine.Random.Range(0, enemys.Count);
                if (enemys[listRandomNumber].GetRandomSpawEnemy()) enemyToSpawn = enemys[listRandomNumber].enemyPrefab;
            }
            GameObject gc = Instantiate(enemyToSpawn, transform);
            enemysFollowPool.Add(gc);
            gc.SetActive(false);
        }
        StartCoroutine(SpawnEnemy(enemysFollowPool, spawnTime));
    }

    private void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.activeInHierarchy)
            {
                if (!_GameController.gameController.player.enemyStack.Contains(child.transform))
                {
                    float spawnDistance = Vector3.Distance(followTarget.position, Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0))) * 1.5f;
                    if (Vector3.Distance(child.transform.position, followTarget.position) > spawnDistance && !CheckIfPoinsIsInsidedCameraView(child.transform.position))
                    {
                        do NewSpawnPosition(child);
                        while (CheckIfPoinsIsInsidedCameraView(child.transform.position) || Physics.CheckSphere(child.transform.position, 1, collisionLayers));
                    }
                }
            }
        }
    }

    IEnumerator SpawnEnemy(List<GameObject> spawnList, float timeForSpawn)
    {
        while (true)
        {
            yield return new WaitForSeconds(timeForSpawn);
            foreach (GameObject gc in spawnList)
            {
                if (!gc.activeInHierarchy)
                {
                    gc.SetActive(true);
                    gc.GetComponent<Animator>().enabled = true;
                    gc.layer = 0;

                    do NewSpawnPosition(gc);
                    while (CheckIfPoinsIsInsidedCameraView(gc.transform.position) || Physics.CheckSphere(gc.transform.position, 1, collisionLayers));

                    break;
                }
            }
        }
    }
    public void NewSpawnPosition(GameObject gc)
    {
        float spawnDistance = Vector3.Distance(followTarget.position, Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0))) * 1.1f;
        Vector3 spawnPos = new Vector3(
            followTarget.position.x + UnityEngine.Random.Range(-spawnDistance, spawnDistance), gc.transform.position.y,
            followTarget.position.z + UnityEngine.Random.Range(-spawnDistance, spawnDistance));
        gc.transform.position = spawnPos;
        gc.transform.forward = (followTarget.position - gc.transform.position).normalized;
    }

    private bool CheckIfPoinsIsInsidedCameraView(Vector3 point)
    {
        Vector3 cameraPoint = Camera.main.WorldToViewportPoint(point);
        if (cameraPoint.x > 1 || cameraPoint.x < 0 ||
            cameraPoint.y > 1 || cameraPoint.y < 0) return false;
        return true;
    }
}
