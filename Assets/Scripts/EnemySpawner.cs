using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Mathematics;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject squareEnemyPrefab;
    [SerializeField] private GameObject seekerEnemyPrefab;
    [SerializeField] private GameObject boss1Prefab;
    [SerializeField] private GameObject enemySpawn1;
    [SerializeField] private GameObject enemySpawn2;
    [SerializeField] private GameObject enemySpawn3;
    [SerializeField] private GameObject enemySpawn4;
    [SerializeField] private GameObject enemySpawn5;
    [SerializeField] private GameObject enemySpawn6;
    [SerializeField] private GameObject enemySpawn7;
    [SerializeField] private TMPro.TextMeshProUGUI enemiesAliveText;
    [SerializeField] private TMPro.TextMeshProUGUI waveText;
    public List<String> enemiesAliveNames = new List<String>();
    private bool initialSpawnDone = false;
    private bool wave2Spawned = false;
    private bool wave3Spawned = false;
    private bool boss1Spawned = false;
    private bool isTransitioning = false;


    private void Start()
    {
        StartCoroutine(StartSpawning());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            initialSpawnDone = true;
            wave2Spawned = true;
            wave3Spawned = true;
        }
        enemiesAliveText.text = "Enemies Remaining: " + enemiesAliveNames.Count;
        waveText.text = "Wave: " + (1 + Convert.ToInt32(wave2Spawned) + Convert.ToInt32(wave3Spawned) + Convert.ToInt32(boss1Spawned)).ToString();

        if (!isTransitioning && enemiesAliveNames.Count == 0 && initialSpawnDone && !wave2Spawned)
        {
            wave2Spawned = true;
            isTransitioning = true;
            StartCoroutine(SpawnWave2());
        }
        else if (!isTransitioning && enemiesAliveNames.Count == 0 && wave2Spawned && !wave3Spawned)
        {
            wave3Spawned = true;
            isTransitioning = true;
            StartCoroutine(SpawnWave3());
        }
        else if (!isTransitioning && enemiesAliveNames.Count == 0 && wave3Spawned && !boss1Spawned)
        {
            boss1Spawned = true;
            isTransitioning = true;
            StartCoroutine(SpawnBoss1());
        }
    }

    private IEnumerator SpawnWave2()
    {
        yield return new WaitForSeconds(5f);
        SpawnCircle();
        yield return new WaitForSeconds(4.5f);
        SpawnWedge();
        yield return new WaitForSeconds(4.5f);
        SpawnLine();
        isTransitioning = false; // enemies are now alive, safe to re-enable checks
    }

    private IEnumerator SpawnWave3()
    {
        yield return new WaitForSeconds(5f);
        SpawnCircle();
        yield return new WaitForSeconds(4.5f);
        SpawnLine();
        yield return new WaitForSeconds(4.5f);
        SpawnCircle();
        isTransitioning = false;
    }

    private IEnumerator SpawnBoss1()
    {
        yield return new WaitForSeconds(5f);
        Instantiate(boss1Prefab, enemySpawn1.transform.position, Quaternion.identity);
        enemiesAliveNames.Add(boss1Prefab.name);
        isTransitioning = false;
    }
    private IEnumerator StartSpawning()
    {
        yield return new WaitForSeconds(2f); // Initial delay before the first spawn
        SpawnWedge();
        yield return new WaitForSeconds(5f); // Delay between spawns
        SpawnCircle();
    }

    private IEnumerator WedgeDelay1()
    {
        yield return new WaitForSeconds(0.3f);
        Instantiate(squareEnemyPrefab, enemySpawn3.transform.position, Quaternion.identity);
        SpawnSeeker();
        // yield return new WaitForSeconds(1f);
        Instantiate(squareEnemyPrefab, enemySpawn4.transform.position, Quaternion.identity);
        enemiesAliveNames.Add(squareEnemyPrefab.name);
        enemiesAliveNames.Add(squareEnemyPrefab.name);
    }
    private IEnumerator WedgeDelay2()
    {
        yield return new WaitForSeconds(0.6f);
        Instantiate(squareEnemyPrefab, enemySpawn5.transform.position, Quaternion.identity);
        Instantiate(squareEnemyPrefab, enemySpawn2.transform.position, Quaternion.identity);
        enemiesAliveNames.Add(squareEnemyPrefab.name);
        enemiesAliveNames.Add(squareEnemyPrefab.name);
    }
    private IEnumerator CircleDelay1()
    {
        yield return new WaitForSeconds(0.3f);
        SpawnSeeker();
        Instantiate(squareEnemyPrefab, enemySpawn3.transform.position, Quaternion.identity);
        Instantiate(squareEnemyPrefab, enemySpawn4.transform.position, Quaternion.identity);
        enemiesAliveNames.Add(squareEnemyPrefab.name);
        enemiesAliveNames.Add(squareEnemyPrefab.name);
        yield return new WaitForSeconds(1.3f);
        Instantiate(squareEnemyPrefab, enemySpawn1.transform.position, Quaternion.identity);
        Instantiate(squareEnemyPrefab, enemySpawn3.transform.position, Quaternion.identity);
        Instantiate(squareEnemyPrefab, enemySpawn4.transform.position, Quaternion.identity);
        enemiesAliveNames.Add(squareEnemyPrefab.name);
        enemiesAliveNames.Add(squareEnemyPrefab.name);
        enemiesAliveNames.Add(squareEnemyPrefab.name);
    }
    private IEnumerator CircleDelay2()
    {
        yield return new WaitForSeconds(0.6f);
        // SpawnSeeker();
        Instantiate(squareEnemyPrefab, enemySpawn5.transform.position, Quaternion.identity);
        Instantiate(squareEnemyPrefab, enemySpawn2.transform.position, Quaternion.identity);
        enemiesAliveNames.Add(squareEnemyPrefab.name);
        enemiesAliveNames.Add(squareEnemyPrefab.name);
        yield return new WaitForSeconds(0.6f);
        Instantiate(squareEnemyPrefab, enemySpawn2.transform.position, Quaternion.identity);
        enemiesAliveNames.Add(squareEnemyPrefab.name);
        Instantiate(squareEnemyPrefab, enemySpawn5.transform.position, Quaternion.identity);
        enemiesAliveNames.Add(squareEnemyPrefab.name);
        initialSpawnDone = true;
    }

    private void SpawnLine()
    {
        SpawnSeeker();
        Instantiate(squareEnemyPrefab, enemySpawn1.transform.position, Quaternion.identity);
        enemiesAliveNames.Add(squareEnemyPrefab.name);
        Instantiate(squareEnemyPrefab, enemySpawn2.transform.position, Quaternion.identity);
        enemiesAliveNames.Add(squareEnemyPrefab.name);
        Instantiate(squareEnemyPrefab, enemySpawn3.transform.position, Quaternion.identity);
        enemiesAliveNames.Add(squareEnemyPrefab.name);
        Instantiate(squareEnemyPrefab, enemySpawn4.transform.position, Quaternion.identity);
        enemiesAliveNames.Add(squareEnemyPrefab.name);
        Instantiate(squareEnemyPrefab, enemySpawn5.transform.position, Quaternion.identity);
        enemiesAliveNames.Add(squareEnemyPrefab.name);
        Instantiate(squareEnemyPrefab, enemySpawn6.transform.position, Quaternion.identity);
        enemiesAliveNames.Add(squareEnemyPrefab.name);
        Instantiate(squareEnemyPrefab, enemySpawn7.transform.position, Quaternion.identity);
        enemiesAliveNames.Add(squareEnemyPrefab.name);
    }

    private void SpawnWedge()
    {
        Instantiate(squareEnemyPrefab, enemySpawn1.transform.position, Quaternion.identity);
        enemiesAliveNames.Add(squareEnemyPrefab.name);
        StartCoroutine(WedgeDelay1());
        StartCoroutine(WedgeDelay2());
    }

    private void SpawnCircle()
    {
        Instantiate(squareEnemyPrefab, enemySpawn1.transform.position, Quaternion.identity);
        enemiesAliveNames.Add(squareEnemyPrefab.name);
        StartCoroutine(CircleDelay1());
        StartCoroutine(CircleDelay2());
    }

    private void SpawnSeeker()
    {
        Instantiate(seekerEnemyPrefab, enemySpawn1.transform.position, quaternion.identity);
        enemiesAliveNames.Add(seekerEnemyPrefab.name);
    }
}
