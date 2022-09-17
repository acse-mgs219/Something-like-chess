using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess.Pieces;
using Database;

namespace Chess
{
    public class Game : MonoBehaviour
    {
        public GameObject Chesspiece;
        public InitialConfiguration InitialConfiguration;

        // Positionss and team for each chess piece.
        private GameObject[,] positions = new GameObject[8, 8];
        private GameObject[] playerBlack = new GameObject[16];
        private GameObject[] playerWhite = new GameObject[16];

        private Colour currentPlayer = Colour.White;
        private bool gameOver = false;

        // Start is called before the first frame update
        void Start()
        {
            foreach (Chesspiece piece in InitialConfiguration.Chesspieces)
            {
                GameObject obj = Instantiate(piece, new Vector3(0, 0, -1), Quaternion.identity);
                Chesspiece cm = obj.GetComponent<Chesspiece>();
                cm.XBoard = piece.XBoard;
                cm.YBoard = piece.YBoard;
                cm.name = piece.name;
                cm.Activate();
            }
        }
        public void SetPosition(GameObject obj)
        {
            Chesspiece cm = obj.GetComponent<Chesspiece>();
            positions[cm.XBoard, cm.YBoard] = obj;
        }
    }
}