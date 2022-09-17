using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Pieces
{
    public class Bishop : Chesspiece
    {
        public override void Activate()
        {
            base.Activate();

            this._type = Type.Bishop;
        }
    }
}
