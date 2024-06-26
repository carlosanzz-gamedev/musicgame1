using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Note
{
    public float time;
    public List<Key> keys;
    public float duration;
    public float holdDuration;
    public bool isSpecial;
    public int points;
}

public enum Key
{
    NONE,
    UP,
    DOWN,
    LEFT,
    RIGHT,
    SPACE
}
