using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITypes
{
    public enum AIType
    {
        [ConstructableEnum(typeof(RandomPiece))]
        RandomPiece,
        [ConstructableEnum(typeof(RandomMove))]
        RandomMove
    }
}
