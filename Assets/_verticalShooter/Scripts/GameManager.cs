using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public EnemySpawner enemySpawner;

    public PlayerShip playerShip;
    [Header("Audio Clips")]
    public AudioClip hitSound;
    [Header("Prefabs")]
    public HitParticle hitParticlePrefab;
    public HitParticle enemyDeathEffectPrefab;
    public HitParticle bossDieParticle;

    //temporary
    public LevelData levelToTest;

    //level event data
    private Dictionary<int, LevelEvent> levelDataDictionary;

    private AudioSource audioSource;

    //pools
    private ObjectPool<HitParticle> hitParticlesPool;
    private ObjectPool<HitParticle> enemyDeathEffectPool;

    private float spawnCounter = 0;

    private bool isPaused;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        spawnCounter = 0;
        //init pools
        hitParticlesPool = new ObjectPool<HitParticle>(hitParticlePrefab, 50, 50);
        hitParticlesPool.Init();

        enemyDeathEffectPool = new ObjectPool<HitParticle>(enemyDeathEffectPrefab, 50, 50);
        enemyDeathEffectPool.Init();

        //Initialize stuff
        enemySpawner.Init(playerShip);

        //handle our level
        levelDataDictionary = new Dictionary<int, LevelEvent>();

        for(int i = 0; i < levelToTest.levelEvents.Count; i++)
        {
            levelDataDictionary.Add(i, levelToTest.levelEvents[i]);
        }
    }

    private void Start()
    {
        //start game
        StartEvent(0);
    }

    private void OnEnable()
    {
        //ship related events
        BusSystem.General.OnShipDied += HandleShipDeath;

        BusSystem.UI.OnGamePauseClicked += PauseGame;
        BusSystem.UI.OnGameResumeClicked += ResumeFunction;

        //effect related events
        BusSystem.Effects.OnBulletImpact += HandleBulletImpact;
        BusSystem.General.OnEnemyDestroyed += HandleEnemyDied;
        BusSystem.General.OnBossDefeated += HandleBossDied;
        //level data
        BusSystem.LevelEvents.OnLevelEventFinished += ChainToNextLevelEvent;
    }

    private void OnDisable()
    {
        //ship related events
        BusSystem.General.OnShipDied -= HandleShipDeath;

        BusSystem.UI.OnGamePauseClicked -= PauseGame;
        BusSystem.UI.OnGameResumeClicked -= ResumeFunction;

        //effect related events
        BusSystem.Effects.OnBulletImpact -= HandleBulletImpact;
        BusSystem.General.OnEnemyDestroyed -= HandleEnemyDied;
        BusSystem.General.OnBossDefeated -= HandleBossDied;
        //level data
        BusSystem.LevelEvents.OnLevelEventFinished -= ChainToNextLevelEvent;
    }

    public void PauseGame()
    {
        if (isPaused)
            return;
        isPaused = true;
        Debug.Log("Paused game");
        Time.timeScale = 0f;

        BusSystem.General.PauseGame();
    }

    public void ResumeFunction()
    {
        if (!isPaused)
            return;
        isPaused = false;
        Debug.Log("Resumed game");
        Time.timeScale = 1f;

        BusSystem.General.ResumeGame();
    }

    private void StartEvent(int eventId)
    {
        if(levelDataDictionary[eventId] is WaveData)
        {
            BusSystem.LevelEvents.SpawnWave(eventId, levelDataDictionary[eventId] as WaveData);
            return;
        }

        if(levelDataDictionary[eventId] is BossSpawnData)
        {
            BusSystem.LevelEvents.SpawnBoss(eventId, levelDataDictionary[eventId] as BossSpawnData);
            return;
        }

        if(levelDataDictionary[eventId] is MessageEvent)
        {
            BusSystem.LevelEvents.DisplayMessageEvent(eventId, levelDataDictionary[eventId] as MessageEvent);
        }
    }

    private void ChainToNextLevelEvent(int eventId)
    {
        if(eventId >= levelDataDictionary.Count - 1)
        {
            Debug.Log("Congratulations, you win!");
        }
        else
        {
            StartEvent(eventId + 1);
        }
    }

    //handlers
    private void HandleShipDeath(GameObject ship)
    {
        ship.SetActive(false);
    }

    private void HandleBulletImpact(Vector3 bulletImpactPos)
    {
        HitParticle hitPart = hitParticlesPool.Instantiate();
        hitPart.transform.position = bulletImpactPos;
        hitPart.OnCleanMe = () => 
        {
            hitParticlesPool.Destroy(hitPart);
        };

        //play a sound
        audioSource.clip = hitSound;
        audioSource.Play();
    }

    private void HandleEnemyDied(Enemy enemyData)
    {
        Vector3 deathPos = enemyData.transform.position;
        HitParticle hitPart = enemyDeathEffectPool.Instantiate();
        hitPart.transform.position = deathPos;
        hitPart.OnCleanMe = () =>
        {
            enemyDeathEffectPool.Destroy(hitPart);
        };

        //play a sound
        audioSource.clip = hitSound;
        audioSource.Play();
    }

    private void HandleBossDied(GenericBossBehaviour gbb)
    {
        Vector3 deathPos = gbb.gameObject.transform.position;
        HitParticle hitPart = Instantiate(bossDieParticle, deathPos, Quaternion.identity);
        hitPart.transform.position = deathPos;
        hitPart.OnCleanMe = () =>
        {
            Destroy(hitPart.gameObject);
        };

        //play a sound
        audioSource.clip = hitSound;
        audioSource.Play();
    }

}
