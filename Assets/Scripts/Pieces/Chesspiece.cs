using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Pieces
{
    public enum Type
    {
        Rook,
        Knight,
        Bishop,
        King,
        Queen,
        Pawn
    }

    public enum Colour
    {
        White,
        Black
    }
    public abstract class Chesspiece : MonoBehaviour
    {
        // References.
        public GameObject Controller;
        public GameObject MovePlate;

        // Positions.
        protected int _xBoard;
        protected int _yBoard;

        public virtual int XBoard
        {
            get => _xBoard;
            set
            {
                _xBoard = value;
            }
        }
        public virtual int YBoard
        {
            get => _yBoard;
            set
            {
                _yBoard = value;
            }
        }

        // Descriptions that make this piece unique.
        protected Colour _colour;
        protected Type _type;
        public Sprite image;

        public virtual void Activate()
        {
            Controller = GameObject.FindGameObjectWithTag("GameController");

            // Take the instantiated location and adjust the transform.
            SetCoordinates();

            this.GetComponent<SpriteRenderer>().sprite = image;
        }

        public void SetCoordinates()
        {
            float x = _xBoard;
            float y = _yBoard;

            x *= 0.66f;
            y *= 0.66f;

            x -= 2.3f;
            y -= 2.3f;

            this.transform.position = new Vector3(x, y, -1);
        }
    }
}
