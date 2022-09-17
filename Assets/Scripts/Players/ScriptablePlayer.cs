using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player", menuName = "Game/Scriptable Player")]
public class ScriptablePlayer : ScriptableObject
{
    public Player PlayerPrefab;
    public ScriptablePiece[,] StartingPieces;
}
