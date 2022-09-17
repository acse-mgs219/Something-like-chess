using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;

[CreateAssetMenu(fileName = "New Piece", menuName = "Game/Scriptable Piece")]
public class ScriptablePiece : ScriptableObject
{
    [SerializeField] Chesspiece _piece;
    public Chesspiece Piece => _piece;
}
