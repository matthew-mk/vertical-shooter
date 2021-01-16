using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveFile", menuName = "Custom/Wave", order = 100)]
public class WaveData : LevelEvent
{
    public Enemy enemyToSpawnPrefab;

    public List<float> row0;
    public List<float> row1;
    public List<float> row2;
    public List<float> row3;
    public List<float> row4;

    //behaviour data
    public float secondsToBestimulatedMin = 2f;
    public float secondsToBestimulatedMax = 4f;

    public int enemiesToBestimulatedMin = 3;
    public int enemiesToBeStimulatedMax = 5;
}
