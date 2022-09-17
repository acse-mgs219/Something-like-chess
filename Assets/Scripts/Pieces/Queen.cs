using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Pieces
{
    public class Queen : Chesspiece
    {
        public override void Activate()
        {
            base.Activate();

            this._type = Type.Queen;
        }
    }
}
