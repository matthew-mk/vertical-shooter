using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossEncounter", menuName = "Custom/Boss", order = 300)]
public class BossSpawnData : LevelEvent
{
    public GenericBossBehaviour BossPrefab;
}
