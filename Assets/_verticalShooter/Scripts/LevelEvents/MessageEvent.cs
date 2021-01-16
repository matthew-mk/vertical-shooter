using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MessageFile", menuName = "Custom/Message", order = 101)]
public class MessageEvent : LevelEvent
{
    public string messageToSay;
}
