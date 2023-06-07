using System;
using UnityEngine;

public interface IRoomGenerator
{
    public Boolean IsLegalPosition(Vector2 position);
    public void Generate();

    public TileType[,] TileMap { get; }
    public int CurrentFloors { get; set; }
    public int MapSize { get; }
}