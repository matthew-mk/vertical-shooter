using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelFile", menuName = "Custom/Level", order = 200)]
public class LevelData : ScriptableObject
{
    public List<LevelEvent> levelEvents;
}
