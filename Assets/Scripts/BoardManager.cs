using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

	
public class BoardManager : MonoBehaviour
{
	// Using Serializable allows us to embed a class with sub properties in the inspector.
	[Serializable]
	public class Count
	{
		public int minimum; 			//Minimum value for our Count class.
		public int maximum; 			//Maximum value for our Count class.
		
		
		//Assignment constructor.
		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}

    public int columns = 5;
    public int rows = 5;
  	public GameObject exit;
    public GameObject[] floorTilesArray;
    public List<GameObject> floorTiles = new List<GameObject>();
    public List<GameObject> wallTiles = new List<GameObject>();
    public List<GameObject> outerWallTiles = new List<GameObject>();

    //public GameObject[] wallTiles;
    //public GameObject[] outerWallTiles;
    public GameObject chestTile;
  	public GameObject ExitLevel;

    public GameObject[] enemyTiles;

    private Transform boardHolder;
    private Transform stage1BoardHolder;
    private Transform stage2BoardHolder;
    private Dictionary<Vector2, Vector2> gridPositions = new Dictionary<Vector2, Vector2>();
    
    private Transform dungeonBoardHolder;
    private Dictionary<Vector2, Vector2> dungeonGridPositions = new Dictionary<Vector2, Vector2>();

     private void LoadFloorTiles(int stage)
    {
        try
        {
            Debug.Log("Loading FLOOR with Proper Method...");

            // This is the short hand version and requires that you include the "using System.Linq;" at the top of the file.
            //var loadedFloorObjects = Resources.LoadAll("Sprites / world0 / Floor", typeof(GameObject)).Cast<GameObject>();
            //foreach (var floorTile in loadedFloorObjects)
            //{
            //    Debug.Log(floorTile.name);
            //}

            var loadedObjects = Resources.LoadAll("Sprites/world0/Floor");
            if (stage == 1)
            {
                loadedObjects = Resources.LoadAll("Sprites/world1/Floor");
            }
          
            foreach (var loadedObject in loadedObjects)
            {
                floorTiles.Add(loadedObject as GameObject);
            }

            foreach (GameObject go in floorTiles)
            {
                Debug.Log(go.name);
            }
        }
        catch (Exception e)
        {
            Debug.Log("Proper Method failed with the following exception: ");
            Debug.Log(e);
        }
        floorTilesArray = floorTiles.ToArray();
    }

    private void LoadWallTiles(int stage)
    {
        try
        {
            Debug.Log("Loading WALL with Proper Method...");

            var loadedObjects = Resources.LoadAll("Sprites/world0/Wall");
            if (stage == 1)
            {
                loadedObjects = Resources.LoadAll("Sprites/world1/Wall");
            }

            foreach (var loadedObject in loadedObjects)
        {
            wallTiles.Add(loadedObject as GameObject);
        }

        foreach (GameObject go in wallTiles)
        {
            Debug.Log(go.name);
        }
    }
        catch (Exception e)
        {
            Debug.Log("Proper Method failed with the following exception: ");
            Debug.Log(e);
        }
    }
    private void LoadOuterWallTiles(int stage)
    {
        try
        {
            Debug.Log("Loading OuterWALL with Proper Method...");

            var loadedObjects = Resources.LoadAll("Sprites/world0/OuterWall");
            if (stage == 1)
            {
                loadedObjects = Resources.LoadAll("Sprites/world1/OuterWall");
            }

            foreach (var loadedObject in loadedObjects)
            {
                outerWallTiles.Add(loadedObject as GameObject);
            }

            foreach (GameObject go in outerWallTiles)
            {
                Debug.Log(go.name);
            }
        }
        catch (Exception e)
        {
            Debug.Log("Proper Method failed with the following exception: ");
            Debug.Log(e);
        }
    }

    public void BoardSetup()
    {
        //This is a placeholder for a rng seed, to be stored with the Player-profile. 
        //When the player saves the game, the seed is stored as well.
        int stagenumber = GameManager.instance.stageNumber;
        if (stagenumber == 0)
        {
            boardHolder = new GameObject("Board").transform;
        }
        
    
        LoadFloorTiles(stagenumber);
        LoadWallTiles(stagenumber);
        LoadOuterWallTiles(stagenumber);

        for (int x =0; x < columns; x++)
        {
            for(int y = 0; y < rows; y++)
            {
                gridPositions.Add(new Vector2(x, y), new Vector2(x, y));
          
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Count)];

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
    }

    private void addTiles(Vector2 tileToAdd)
    {
        if (!gridPositions.ContainsKey(tileToAdd))
        {
            gridPositions.Add(tileToAdd, tileToAdd);
            GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Count)];
            GameObject instance = Instantiate(toInstantiate, new Vector3(tileToAdd.x, tileToAdd.y, 0f), Quaternion.identity) as GameObject;

            instance.transform.SetParent(boardHolder);

            //Choose a wallTile at random
            if(Random.Range(0, 3) == 1)
            {
                toInstantiate = wallTiles[Random.Range(0, wallTiles.Count)];
                instance = Instantiate(toInstantiate, new Vector3(tileToAdd.x, tileToAdd.y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }

            //Generate Exit tiles which lead to Dungeons
            else if(Random.Range(0, 15) == 1)
            {
                toInstantiate = exit;
                instance = Instantiate(toInstantiate, new Vector3(tileToAdd.x, tileToAdd.y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }

            //Generate Enemies 
            else if (Random.Range(0, GameManager.instance.enemySpawnRatio) == 1)
            {
                toInstantiate = enemyTiles[Random.Range(0, enemyTiles.Length)];
                instance = Instantiate(toInstantiate, new Vector3(tileToAdd.x, tileToAdd.y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }

            //Generate ExitLevel tiles which lead to Dungeons
            else if (GameManager.instance.XPPoints > 5 && Random.Range(0, 12) == 1)
            {
                toInstantiate = ExitLevel;
                instance = Instantiate(toInstantiate, new Vector3(tileToAdd.x, tileToAdd.y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    public void addToBoard(int horizontal, int vertical)
    {
        if (horizontal == 1)
        {
            //Check if tiles exist
            int x = (int)Player.position.x;
            int sightX = x + 2;
            for (x += 1; x <= sightX; x++)
            {
                int y = (int)Player.position.y;
                int sightY = y + 1;
                for (y -= 1; y <= sightY; y++)
                {
                    addTiles(new Vector2(x, y));
                }
            }
        }

        else if (horizontal == -1)
        {
            int x = (int)Player.position.x;
            int sightX = x - 2;
            for (x -= 1; x >= sightX; x--)
            {
                int y = (int)Player.position.y;
                int sightY = y + 1;
                for (y -= 1; y <= sightY; y++)
                {
                    addTiles(new Vector2(x, y));
                }
            }
        }

        else if (vertical == 1)
        {
            int y = (int)Player.position.y;
            int sightY = y + 2;
            for (y += 1; y <= sightY; y++)
            {
                int x = (int)Player.position.x;
                int sightX = x + 1;
                for (x -= 1; x <= sightX; x++)
                {
                    addTiles(new Vector2(x, y));
                }
            }
        }

        else if (vertical == -1)
        {
            int y = (int)Player.position.y;
            int sightY = y - 2;
            for (y -= 1; y >= sightY; y--)
            {
                int x = (int)Player.position.x;
                int sightX = x + 1;
                for (x -= 1; x <= sightX; x++)
                {
                    addTiles(new Vector2(x, y));
                }
            }
        }
    }

    public void SetDungeonBoard(Dictionary<Vector2, TileType> dungeonTiles, int bound, Vector2 endPos)
    {
        boardHolder.gameObject.SetActive(false);
        dungeonBoardHolder = new GameObject("Dungeon").transform;
        GameObject toInstantiate, instance;

        foreach (KeyValuePair<Vector2, TileType> tile in dungeonTiles)
        {
            toInstantiate = floorTiles[Random.Range(0, floorTiles.Count)];
            instance = Instantiate(toInstantiate, new Vector3(tile.Key.x, tile.Key.y, 0f), Quaternion.identity) as GameObject;
            instance.transform.SetParent(dungeonBoardHolder);

            if(tile.Value == TileType.chest)
            {
                toInstantiate = chestTile;
                instance = Instantiate(toInstantiate, new Vector3(tile.Key.x, tile.Key.y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(dungeonBoardHolder);

            }
            else if(tile.Value == TileType.enemy)
            {
                toInstantiate = enemyTiles[Random.Range(0, enemyTiles.Length)];
                instance = Instantiate(toInstantiate, new Vector3(tile.Key.x, tile.Key.y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(dungeonBoardHolder);
            }
            //Add random walltiles to the dungeons.
            else if (tile.Value == TileType.wall)
            {
                toInstantiate = wallTiles[Random.Range(0, wallTiles.Count)];
                instance = Instantiate(toInstantiate, new Vector3(tile.Key.x, tile.Key.y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(dungeonBoardHolder);
            }
        }

        for (int x = -1 ; x < bound + 1; x++)
        {
            for(int y = -1; y < bound + 1; y++)
            {
                    if (!dungeonTiles.ContainsKey(new Vector2(x, y)))
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Count)];
                    instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(dungeonBoardHolder);
                }
            }
        }

        toInstantiate = exit;
        instance = Instantiate(toInstantiate, new Vector3(endPos.x, endPos.y, 0f), Quaternion.identity) as GameObject;
        instance.transform.SetParent(dungeonBoardHolder);
    }

    public void SetWorldBoard()
    {
        Destroy(dungeonBoardHolder.gameObject);
        boardHolder.gameObject.SetActive(true);
    }

    public bool checkValidTile(Vector2 pos)
    {
        if (gridPositions.ContainsKey(pos))
        {
            return true;
        }
        return false;
    }

    public void transferScene()
    {
        gridPositions.Clear();
        floorTiles.Clear();
        wallTiles.Clear();
        outerWallTiles.Clear();
        boardHolder.gameObject.SetActive(false);

        stage1BoardHolder = new GameObject("Board").transform;
        //stage1BoardHolder.gameObject.SetActive(true);
    }
}
