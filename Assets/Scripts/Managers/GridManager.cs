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

    private Tile[,] _tiles;
    public Tile[,] Tiles => _tiles;

    void Awake()
    {
        instance = this;
    }

    public void Init()
    {
        _tiles = new Tile[Width, Height];
        GenerateGrid();

        _cam.position = new Vector3(_width / 2f - 0.5f, _height / 2f - .5f, -10);
        GameManager.instance.ChangeState(GameState.PositionPieces);
    }

    void GenerateGrid()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.Init(x, y);
                spawnedTile.name = $"{(char) (x + 65)}{y + 1}";
                _tiles[x, y] = spawnedTile;
            }
        }

        ReadPieces();
    }

    void ReadPieces()
    {
        using (var reader = new StreamReader(@"Assets\Database\board.csv"))
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

    public Tile GetTileByName(string name)
    {
        return _tiles.Cast<Tile>().First(t => t.name == name);
    }

    public Tile GetTileAtPosition(int x, int y)
    {
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

    public List<Tile> GetTilesFittingConstraint(Func<Tile, Boolean> constraint)
    {
        return _tiles.Cast<Tile>().Where(t => constraint(t)).ToList();
    }

    public Tile GetRandomTileFittingConstraint(Func<Tile, Boolean> constraint)
    {
        return GetTilesFittingConstraint(constraint).RandomElement();
    }

    public List<Tile> GetNeighborsOfTile(Tile tile, int order = 1)
    {
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
}
