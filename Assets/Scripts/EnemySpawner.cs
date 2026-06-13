using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Mathematics;

public class EnemySpawner : MonoBehaviour
{
    [Header("Level data")]
    [SerializeField] private LevelConfig config;

    [Header("Enemy prefabs")]
    [SerializeField] private GameObject squareEnemyPrefab;
    [SerializeField] private GameObject seekerEnemyPrefab;

    [Header("Scene references")]
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
    private int currentWave = 0;
    private bool isTransitioning = false;

    private void Start()
    {
        if (config == null)
            Debug.LogError($"{name}: EnemySpawner has no LevelConfig assigned — no enemies will spawn.", this);
    }

    private void Update()
    {
        if (config == null) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            // Dev skip: fast-forward so the final (boss) wave runs next.
            currentWave = Mathf.Max(currentWave, config.waves.Count - 1);
        }

        enemiesAliveText.text = "Enemies Remaining: " + enemiesAliveNames.Count;
        waveText.text = "Wave: " + Mathf.Max(1, currentWave);

        // A wave starts when the previous one is fully cleared. isTransitioning guards the
        // window between "last wave cleared" and "next wave's first enemy spawned".
        if (!isTransitioning && enemiesAliveNames.Count == 0 && currentWave < config.waves.Count)
        {
            isTransitioning = true;
            StartCoroutine(RunWave(config.waves[currentWave]));
            currentWave++;
        }
    }

    private IEnumerator RunWave(Wave wave)
    {
        foreach (WaveStep step in wave.steps)
        {
            yield return new WaitForSeconds(step.delay);
            SpawnFormation(step.formation);
        }
        isTransitioning = false; // enemies are now alive, safe to re-enable checks
    }

    private void SpawnFormation(Formation formation)
    {
        switch (formation)
        {
            case Formation.Wedge:  SpawnWedge();  break;
            case Formation.Circle: SpawnCircle(); break;
            case Formation.Line:   SpawnLine();   break;
            case Formation.Seeker: SpawnSeeker(); break;
            case Formation.Boss:   SpawnBoss();   break;
        }
    }

    private void SpawnBoss()
    {
        Instantiate(config.bossPrefab, enemySpawn1.transform.position, Quaternion.identity);
        enemiesAliveNames.Add(config.bossPrefab.name);
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
