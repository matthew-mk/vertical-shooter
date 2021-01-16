using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class BusSystem
{
    public static class General
    {
        public static Action<Enemy.EnemyType, int, float> OnSpawnEnemies;
        public static void SpawnEnemies(Enemy.EnemyType enemyID, int enemyAmount, float spawnInterval) { OnSpawnEnemies?.Invoke(enemyID, enemyAmount, spawnInterval); }

        public static Action<int, int> OnShipHit;
        public static void ShipGotHit(int currentHealth, int maxHealth) { OnShipHit?.Invoke(currentHealth, maxHealth); }        

        public static Action<GameObject> OnShipDied;
        public static void ShipGotDead(GameObject go) { OnShipDied?.Invoke(go); }

        public static Action<Enemy> OnEnemyDestroyed;
        public static void DestroyEnemy(Enemy enemy) { OnEnemyDestroyed?.Invoke(enemy); }

        public static Action<GenericBossBehaviour> OnBossDefeated;
        public static void DefeatBoss(GenericBossBehaviour genericBossBehaviour) { OnBossDefeated?.Invoke(genericBossBehaviour); }

        public static Action OnGamePaused;
        public static void PauseGame() { OnGamePaused?.Invoke(); }
        public static Action OnGameResumed;
        public static void ResumeGame() { OnGameResumed?.Invoke(); }

        public static Action<string, float> OnDisplayMessageGeneric;
        public static void CallDisplayGenericMessage(string message, float timeToShow) { OnDisplayMessageGeneric?.Invoke(message, timeToShow); }
    }

    public static class LevelEvents
    {
        public static Action<int, MessageEvent> OnDisplayMessageEvent;
        public static void DisplayMessageEvent(int eventId, MessageEvent messEvnt) { OnDisplayMessageEvent?.Invoke(eventId, messEvnt); }

        public static Action<int, WaveData> OnSpawnWave;
        public static void SpawnWave(int eventId, WaveData waveData) { OnSpawnWave?.Invoke(eventId, waveData); }

        public static Action<int, BossSpawnData> OnSpawnBoss;
        public static void SpawnBoss(int eventId, BossSpawnData bossData) { OnSpawnBoss?.Invoke(eventId, bossData); }

        public static Action<int> OnLevelEventFinished;
        public static void LevelEventFinished(int eventId) { OnLevelEventFinished?.Invoke(eventId); }
    }

    public static class Effects
    {
        public static Action<Vector3> OnBulletImpact;
        public static void BulletHit(Vector3 impactPosition) { OnBulletImpact?.Invoke(impactPosition); }
    }

    public static class UI
    {
        public static Action OnGamePauseClicked;
        public static void PauseGameRequest() { OnGamePauseClicked?.Invoke(); }
        public static Action OnGameResumeClicked;
        public static void ResumeGameRequest() { OnGameResumeClicked?.Invoke(); }
    }
}
