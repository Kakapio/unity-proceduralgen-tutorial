using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    private const int MaxFloors = 5000;
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
    }

    public Boolean IsLegalPosition(Vector2 position)
    {
        if (position.x < 0 || position.y < 0
                           || position.x > MapSize - 1 || position.y > MapSize - 1)
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

        Debug.Log($"True Count: {trueCount}");
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

            eaters.Add(new Eater(eaters[i].Position, this));
            eaters[i].HasChild = false;
        }
    }
}