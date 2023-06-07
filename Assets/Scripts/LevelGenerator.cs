using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject floor;
    public GameObject wall;
    public int offset;
    
    private RoomGenerator rg;
    private List<GameObject> placedPrefabs; // used to remove walls/floors on reset.

    // Start is called before the first frame update
    void Start()
    {
        rg = new RoomGenerator();
        placedPrefabs = new List<GameObject>();
        PlacePrefabs();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            DestroyAllTiles();
            rg.Generate();
            PlacePrefabs();
        }
    }
    
    private void PlacePrefabs()
    {
        for (int i = 0; i < rg.TileMap.GetLength(0); i++)
        {
            for (int j = 0; j < rg.TileMap.GetLength(1); j++)
            {
                if (rg.TileMap[i, j] == TileType.Floor)
                {
                    placedPrefabs.Add(Instantiate(floor, 
                        new Vector3(i * offset, floor.transform.position.y, j * offset), 
                        Quaternion.identity));
                }
                else if (rg.TileMap[i, j] == TileType.Wall)
                {
                    placedPrefabs.Add(Instantiate(wall, 
                        new Vector3(i * offset, wall.transform.position.y, j * offset), 
                        Quaternion.identity));
                }
            }
        }
    }

    private void DestroyAllTiles()
    {
        foreach (GameObject tile in placedPrefabs)
        {
            Destroy(tile);
        }
    }
}
