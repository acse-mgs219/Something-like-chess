using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notes : MonoBehaviour
{
    [TextArea]
    [Tooltip("Doesn't do anything. Just comments shown in inspector")]
    public string DevelopmentLog = @" 
    Working on ability to restart the game after it has ended. We can re-generate the grid easily enough, but we still need a way to robustly destroy the previous grid.
    ";
}
