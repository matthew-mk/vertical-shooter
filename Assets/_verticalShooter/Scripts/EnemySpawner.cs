using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class WaveBehaviour
{
    public Action<int> OnWaveCleared;
    public WaveData waveData;
    public List<Enemy> enemyList;

    private float waveStimulatedCounter = 0f;
    private float secondsTillStimulate = 0f;
    private bool randomValueCalculated = false;
    private int levelEventId;

    public WaveBehaviour(WaveData waveData, int enemyListSize, int levelEventId)
    {
        enemyList = new List<Enemy>(enemyListSize);
        this.waveData = waveData;
        this.levelEventId = levelEventId;
    }

    public void ActivateWave(List<Enemy> enemyListToCopy)
    {
        enemyList = enemyListToCopy;
        foreach (var enemy in enemyList)
        {
            enemy.Activate();
        }
    }

    public void EnemyDestroyed(Enemy en)
    {
        if (enemyList.Count <= 0)
        {
            OnWaveCleared?.Invoke(levelEventId);
        }
    }

    public void UpdateBehaviour()
    {
        if (!randomValueCalculated)
        {
            secondsTillStimulate = UnityEngine.Random.Range(waveData.secondsToBestimulatedMin, waveData.secondsToBestimulatedMax);
            randomValueCalculated = true;
        }
        waveStimulatedCounter += Time.deltaTime;
        if (waveStimulatedCounter >= secondsTillStimulate)
        {
            int enemiesToBeStimulated = UnityEngine.Random.Range(waveData.enemiesToBestimulatedMin, waveData.enemiesToBeStimulatedMax + 1);
            if (enemiesToBeStimulated > enemyList.Count)
            {
                enemiesToBeStimulated = enemyList.Count;
                foreach (var en in enemyList)
                {
                    en.Stimulate();
                }
                randomValueCalculated = false;
                waveStimulatedCounter = 0;
                return;
            }
            //randomly pick shit
            List<int> indexes = new List<int>();
            for (int i = 0; i < enemyList.Count; i++)
            {
                indexes.Add(i);
            }
            //shuffles indexes list
            System.Random rng = new System.Random();
            int n = indexes.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = indexes[k];
                indexes[k] = indexes[n];
                indexes[n] = value;
            }
            //create 
            indexes.RemoveRange(enemiesToBeStimulated, indexes.Count - enemiesToBeStimulated);

            for(int i = 0; i < indexes.Count; i++)
            {
                enemyList[i].Stimulate();
            }

            randomValueCalculated = false;
            waveStimulatedCounter = 0;
        }
    }
}

public class EnemySpawner : MonoBehaviour
{
    public GameObject leftLimit;
    public GameObject rightLimit;
    public GameObject lowerLimit;

    private WaveData waveToSpawn;

    private float uppwerYSpawnValue;
    private float lowerYSpawnValue;

    private float xLeftSpawnValue;
    private float xRightSpawnValue;

    private List<Enemy> enemyList;
    private WaveBehaviour currentWaveBehaviour;

    private PlayerShip playerShip;

    //pool
    private ObjectPool<Enemy> enemyPool;

    //spawn matrix
    private int matrixRows = 5;
    private int matrixCols = 4;
    private Vector3[][] enemySpawnMatrix;

    //boss related variables
    private GenericBossBehaviour currentBoss;
    private int bossEventID;

    private void Awake()
    {
        uppwerYSpawnValue = leftLimit.transform.position.y;
        lowerYSpawnValue = lowerLimit.transform.position.y;
        xLeftSpawnValue = leftLimit.transform.position.x;
        xRightSpawnValue = rightLimit.transform.position.x;

        enemyList = new List<Enemy>();

        //matrix init
        //x length
        float xMiniSegmentLength = (xRightSpawnValue - xLeftSpawnValue) / (matrixCols);
        float yMiniSegmentLength = (uppwerYSpawnValue - lowerYSpawnValue) / (matrixRows + 1);
        Vector3 centerOffset = new Vector3(xMiniSegmentLength / 2, -yMiniSegmentLength / 2, 0f);
        Vector3 topLeftCorner = leftLimit.transform.position;

        enemySpawnMatrix = new Vector3[matrixCols][];
        for (int i = 0; i < matrixCols; i++)
        {
            enemySpawnMatrix[i] = new Vector3[matrixRows];
            for (int j = 0; j < matrixRows; j++)
            {
                enemySpawnMatrix[i][j] = topLeftCorner + new Vector3(xMiniSegmentLength * i, -yMiniSegmentLength * j, 0f) + centerOffset;
            }
        }
    }

    private void OnEnable()
    {
        BusSystem.General.OnEnemyDestroyed += HandleEnemyDestroy;

        //level events
        BusSystem.LevelEvents.OnSpawnWave += HandleSpawnWave;
        BusSystem.LevelEvents.OnSpawnBoss += HandleSpawnBoss;
        BusSystem.General.OnBossDefeated += HandleCleanUpBoss;
    }

    private void OnDisable()
    {
        BusSystem.General.OnEnemyDestroyed -= HandleEnemyDestroy;

        //level events
        BusSystem.LevelEvents.OnSpawnWave -= HandleSpawnWave;
        BusSystem.LevelEvents.OnSpawnBoss -= HandleSpawnBoss;
        BusSystem.General.OnBossDefeated -= HandleCleanUpBoss;
    }

    private void Update()
    {
        if(currentWaveBehaviour != null)
        {
            currentWaveBehaviour.UpdateBehaviour();
        }
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    SpawnWave(waveToSpawn);
        //}
    }

    //custom functions
    public void Init(PlayerShip playerShip)
    {
        this.playerShip = playerShip;
    }

    private void SpawnEnemy(Vector3 spawnPos, float spawnDelay)
    {
        if (enemyList.Count >= 30)
            return;
        StartCoroutine(SpawnShipWithDelay(spawnPos, spawnDelay));
    }

    private void SpawnWave(int levelEventId, WaveData wave)
    {
        float maxSpawnDelay = -1;
        int enemyCounter = 0;
        List<float> tempFloatList;
        for (int i = 0; i < matrixRows; i++)
        {
            switch (i)
            {
                case 0: tempFloatList = wave.row0; break;
                case 1: tempFloatList = wave.row1; break;
                case 2: tempFloatList = wave.row2; break;
                case 3: tempFloatList = wave.row3; break;
                case 4: tempFloatList = wave.row4; break;
                default: tempFloatList = new List<float>(); break;
            }

            for (int j = 0; j < matrixCols; j++)
            {
                if (tempFloatList[j] > 0)
                {
                    SpawnEnemy(enemySpawnMatrix[j][i], tempFloatList[j]);
                    enemyCounter++;
                    if (maxSpawnDelay < tempFloatList[j])
                    {
                        maxSpawnDelay = tempFloatList[j];
                    }
                }
            }
            StartCoroutine(ActivateWave(maxSpawnDelay));
        }
        currentWaveBehaviour = new WaveBehaviour(wave, enemyCounter, levelEventId);
        currentWaveBehaviour.OnWaveCleared = (lvlEvntId) =>
        {
            BusSystem.LevelEvents.LevelEventFinished(lvlEvntId);
            currentWaveBehaviour.OnWaveCleared = null;
        };
    }

    //handlers
    private void HandleEnemyDestroy(Enemy en)
    {
        if (en.enemyType != waveToSpawn.enemyToSpawnPrefab.enemyType)
            return;
        enemyList.Remove(en);
        enemyPool.Destroy(en);
        currentWaveBehaviour.EnemyDestroyed(en);
    }

    private void HandleSpawnWave(int levelEventId, WaveData waveToSpawn)
    {
        this.waveToSpawn = waveToSpawn;

        //creating pool
        enemyPool = new ObjectPool<Enemy>(waveToSpawn.enemyToSpawnPrefab, 30);
        enemyPool.Init();

        SpawnWave(levelEventId, waveToSpawn);
    }

    private void HandleSpawnBoss(int levelEventId, BossSpawnData bossData)
    {
        currentBoss = Instantiate(bossData.BossPrefab, new Vector3(0f, 14f, 0), Quaternion.identity);
        bossEventID = levelEventId;
    }

    private void HandleCleanUpBoss(GenericBossBehaviour genericBossBehaviour)
    {
        Destroy(currentBoss.gameObject);
        BusSystem.LevelEvents.LevelEventFinished(bossEventID);
    }

    //IEnumerators
    IEnumerator SpawnShipWithDelay(Vector3 spawnPos, float spawnDelay)
    {
        yield return new WaitForSeconds(spawnDelay);
        Vector3 spawnPosition = spawnPos;
        //Instantiate(AsteroidPrefab, spawnPosition, Quaternion.identity);
        Enemy en = enemyPool.Instantiate(transform.root);
        en.gameObject.transform.position = spawnPosition;
        en.Init(xLeftSpawnValue, xRightSpawnValue, playerShip);
        enemyList.Add(en);
    }

    IEnumerator ActivateWave(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentWaveBehaviour.ActivateWave(enemyList);
    }
}
