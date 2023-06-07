using System;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Floor,
    Wall
}

public class RoomGenerator : IRoomGenerator
{
    public TileType[,] TileMap { get; }
    public int CurrentFloors { get; set; } = MaxFloors;
    public int MapSize { get; } //The square dimensions of the tile map in terms of tiles.

    // This is how many floors are generated before walls and single tiles are accounted for.
    private const int MaxFloors = 3500; 
    private List<Eater> eaters;
    private bool EnoughWallsRemoved => CurrentFloors <= 0;

    public RoomGenerator(int mapSize = 100)
    {
        MapSize = mapSize;
        TileMap = new TileType[MapSize, MapSize];
        eaters = new List<Eater>();
        Generate();
    }

    public void Generate()
    {
        Reset();

        while (!EnoughWallsRemoved)
        {
            MoveEaters();
        }
        
        RemoveSingleTiles();
        AddWalls();
        PrintTileCount();
    }

    /// <summary>
    /// Remove stray tiles from our map.
    /// </summary>
    private void RemoveSingleTiles()
    {
        for (int i = 1; i < MapSize - 1; i++)
        {
            for (int j = 1; j < MapSize - 1; j++)
            {
                if (TileMap[i, j] == TileType.Wall && TileMap[i - 1, j] == TileType.Floor
                                                   && TileMap[i + 1, j] == TileType.Floor
                                                   && TileMap[i, j - 1] == TileType.Floor
                                                   && TileMap[i, j + 1] == TileType.Floor)
                    TileMap[i, j] = TileType.Floor;

            }
        }
    }

    private void AddWalls()
    {
        for (int i = 0; i < MapSize; i++)
        {
            TileMap[0, i] = TileType.Wall;
            TileMap[MapSize-1, i] = TileType.Wall;
            TileMap[i, 0] = TileType.Wall;
            TileMap[i, MapSize - 1] = TileType.Wall;
        }
    }

    public Boolean IsLegalPosition(Vector2 position)
    {
        // We use 1 and -2 instead of 0 and -1 here because we add walls, which can lead to inaccessible areas
        // unless we pad by one more tile.
        if (position.x < 1 || position.y < 1
                           || position.x > MapSize - 2 || position.y > MapSize - 2)
        {
            return false;
        }

        return true;
    }

    public Boolean AvailableForPlayer(Vector2 position)
    {
        if (IsLegalPosition(position)
            && TileMap[(int)position.x, (int)position.y] != TileType.Wall)
            return true;

        return false;
    }

    public void Reset()
    {
        //Set all tiles back to wall.
        for (int i = 0; i < TileMap.GetLength(0); i++)
        {
            for (int j = 0; j < TileMap.GetLength(1); j++)
            {
                TileMap[i, j] = TileType.Wall;
            }
        }

        CurrentFloors = MaxFloors;
        eaters.Clear();

        eaters.Add(new Eater(new Vector2(MapSize / 2, MapSize / 2), this));
    }

    private void PrintTileCount()
    {
        int trueCount = 0;

        for (int i = 0; i < TileMap.GetLength(0); i++)
        {
            for (int j = 0; j < TileMap.GetLength(1); j++)
            {
                if (TileMap[i, j] == TileType.Floor)
                    trueCount++;
            }
        }

        Debug.Log($"True floor count: {trueCount}");
    }

    public void PrintRoom()
    {
            for (int i = 0; i < TileMap.GetLength(0); i++)
            {
                string row = "";
                for (int j = 0; j < TileMap.GetLength(1); j++)
                {
                    row += TileMap[i,j] + " ";
                }
                Debug.Log(row);
                Debug.Log("");
            }
    }

    private void MoveEaters()
    {
        AddChildEaters();
        foreach (var eater in eaters)
        {
            //End condition check placed here to ensure too many walls are not removed.
            if (EnoughWallsRemoved)
                return;

            eater.TryMove();
        }
    }

    private void AddChildEaters()
    {
        for (int i = 0; i < eaters.Count; i++)
        {
            if (!eaters[i].HasChild)
                return;

            Debug.Log($"Adding a child eater for a total of {eaters.Count} eaters");
            eaters.Add(new Eater(eaters[i].Position, this));
            eaters[i].HasChild = false;
        }
    }
}