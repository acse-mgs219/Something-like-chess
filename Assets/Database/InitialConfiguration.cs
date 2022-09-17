using Chess.Pieces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Database
{
    [CreateAssetMenu(fileName = "New Initial Configuration", menuName = "Assets/Databases/Pieces")]
    public class InitialConfiguration : ScriptableObject
    {
        public List<Chesspiece> Chesspieces;
    }
}
