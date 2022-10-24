using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notes : MonoBehaviour
{
    [TextArea]
    [Tooltip("Doesn't do anything. Just comments shown in inspector")]
    public string DevelopmentLog = @" 
    Recently changed how moves are finished.
    Now, they get informed by their piece once it has reached its destination. Unless the moves are instant.
    This fixes the issue of pieces being eaten before the capturing piece has reached their square.
    However, it creates an issue that rook moves in castle are no longer shown on screen.
    Also, the check / move sounds are a bit broken, it plays the same sound every time (sounds like the check sound?).
    Finally, promotion is still broken despite my best attempts to fix it. It gives you a double turn. Investigate why. (Likely, a promoting move already also ends your turn).
    ";
}
