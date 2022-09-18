using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static UnityEssentials.Extensions.ColorHelper;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    [SerializeField] private int _width;
    public int Width => _width;
    [SerializeField] private int _height;
    public int Height => _height;

    [SerializeField] private Transform _cam;
    // Can make into a list later if want various tile types.
    [SerializeField] private Tile _tilePrefab;

    private Tile[,] _board;
    public Tile[,] Board => _board;

    private Tile[,] _predictionBoard;
    public Tile[,] PredictionBoard => _predictionBoard;

    void Awake()
    {
        instance = this;
    }

    public void Init()
    {
        _board = new Tile[Width, Height];
        _predictionBoard = new Tile[Width, Height];
        // Generate both the real and prediction boards.
        GenerateGrid(real: true);
        GenerateGrid(real: false);

        _cam.position = new Vector3(_width / 2f - 0.5f, _height / 2f - .5f, -10);
        PlayerManager.instance.Init();
    }

    void GenerateGrid(bool real)
    {
        Tile[,] _tiles = real ? _board : _predictionBoard;
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                // We hide a prediction board behind the real board (very sneaky).
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y, real ? 0 : 10), Quaternion.identity);
                spawnedTile.Init(x, y);
                spawnedTile.name = $"{(char) (x + 65)}{y + 1}";
                _tiles[x, y] = spawnedTile;
            }
        }

        if (real) ReadPieces();
    }

    public void ResetPredictionBoard()
    {
        foreach (Player player in PlayerManager.instance.Players)
        {
            foreach (Chesspiece piece in player.Pieces)
            {
                piece.PlaceCopyOnPredicitonBoard();
            }
        }
    }

    public void ReadPieces(bool real = true)
    {
        string piecesLocation = real ? @"Assets\Database\board.csv" : @"Assets\Database\prediction_board.csv";
        using (var reader = new StreamReader(piecesLocation))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                if (values.Count() != 3)
                {
                    throw new System.Exception("Expected exactly 3 values per line in board.csv file - 1 int for player index, 1 string for piece type, 1 string for tile name.");
                }

                // Transform 1-based to 0-based index.
                int playerIndex = int.Parse(values[0]) - 1;
                Player player = PlayerManager.instance.Players[playerIndex];

                string pieceName = values[1].Trim();
                PieceType type = (PieceType) Enum.Parse(typeof(PieceType), pieceName);

                string tileName = values[2].Trim();
                Tile tile = GetTileByName(tileName);

                PieceManager.instance.SpawnPieceAtTile(player, type, tile);
            }
        }
    }

    public void SaveGridAsCSV(bool real)
    {
        Tile[,] _tiles = real ? _board : _predictionBoard;

        using (var writer = new StreamWriter(@"Assets\Database\prediction_board.csv"))
        {
            foreach (Tile tile in _tiles)
            {
                Chesspiece pieceOnTile = tile.OccupyingPiece;
                if (pieceOnTile != null)
                {
                    int playerIndex = PlayerManager.instance.Players.IndexOf(pieceOnTile.Player) + 1;
                    string pieceName = pieceOnTile.GetType().Name;
                    string tileName = tile.name;

                    writer.WriteLine($"{playerIndex}, {pieceName}, {tileName}");
                }
            }
        }
    }

    public Tile GetTileByName(string name, bool real = true)
    {
        Tile[,] _tiles = real ? _board : _predictionBoard;

        return _tiles.Cast<Tile>().First(t => t.name == name);
    }

    public Tile GetTileAtPosition(int x, int y, bool real = true)
    {
        Tile[,] _tiles = real ? _board : _predictionBoard;

        if (x < _tiles.GetLength(0) && y < _tiles.GetLength(1))
        {
            return _tiles[x, y];
        }
        else
        {
            Debug.Log($"Trying to access tile at ({x}, {y}) but it does not exist as far as I can see!");
            return null;
        }
    }

    #region UnusedUtilityFunctions
    public List<Tile> GetTilesFittingConstraint(Func<Tile, Boolean> constraint, bool real = true)
    {
        Tile[,] _tiles = real ? _board : _predictionBoard;

        return _tiles.Cast<Tile>().Where(t => constraint(t)).ToList();
    }

    public Tile GetRandomTileFittingConstraint(Func<Tile, Boolean> constraint)
    {
        return GetTilesFittingConstraint(constraint).RandomElement();
    }

    public List<Tile> GetNeighborsOfTile(Tile tile, int order = 1, bool real = true)
    {
        Tile[,] _tiles = real ? _board : _predictionBoard;

        int x = tile.X;
        int y = tile.Y;
        int rowLimit = _tiles.GetLength(0) - 1;
        int columnLimit = _tiles.GetLength(1) - 1;

        List<Tile> neighbors = new List<Tile>();

        for (var i = Math.Max(0, x - order); i <= Math.Min(x + order, rowLimit); i++)
        {
            for (var j = Math.Max(0, y - order); j <= Math.Min(y + order, columnLimit); j++)
            {
                if ((i != x || j != y) && _tiles[i, j] != null)
                {
                    neighbors.Add(_tiles[i, j]);
                }
            }
        }

        return neighbors;
    }

    public List<Tile> GetAvailableTilesAround(Tile tile, int order = 1, Func<Tile, Boolean> constraint = null)
    {
        return GetNeighborsOfTile(tile, order).Where(t => constraint == null || constraint(t as Tile)).ToList();
    }
    #endregion
}
