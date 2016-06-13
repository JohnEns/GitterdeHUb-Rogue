using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public enum TileType
{
    essential, random, empty, chest, enemy, wall
}

public class DungeonManager : MonoBehaviour {

    [Serializable]
    public class PathTile
    {
        public TileType type;
        public Vector2 position;
        public List<Vector2> adjacentPathTiles;

        public PathTile (TileType t, Vector2 p, int min, int max, Dictionary<Vector2, TileType> currentTiles)
        {
            type = t;
            position = p;
            adjacentPathTiles = getAdjacentPath(min, max, currentTiles);
        }

        public List<Vector2> getAdjacentPath(int minBound, int maxBound, Dictionary<Vector2, TileType> currentTiles)
        {
            List<Vector2> pathTiles = new List<Vector2>();
            if(position.y + 1 < maxBound && !currentTiles.ContainsKey(new Vector2(position.x, position.y + 1)))
            {
                pathTiles.Add(new Vector2(position.x, position.y + 1));
            }

            if(position.x + 1 < maxBound && !currentTiles.ContainsKey(new Vector2(position.x + 1, position.y)))
            {
                pathTiles.Add(new Vector2(position.x + 1, position.y));
            }

            if (position.y - 1 > minBound && !currentTiles.ContainsKey(new Vector2(position.x, position.y - 1)))
            {
                pathTiles.Add(new Vector2(position.x, position.y - 1));
            }

            if (position.x - 1 >= minBound && !currentTiles.ContainsKey(new Vector2(position.x - 1, position.y)) && type != TileType.essential)
            {
                pathTiles.Add(new Vector2(position.x - 1, position.y));
            }
            return pathTiles;
        }

    }

    public Dictionary<Vector2, TileType> gridPositions = new Dictionary<Vector2, TileType>();
    public int minBound = 0, maxBound;
    public static Vector2 startPos;
    public Vector2 endPos;

    public void StartDungeon()
    {
        //Random.seed = 1;
        gridPositions.Clear();
        maxBound = Random.Range(50, 101);

        BuildEssentialPath();

        BuildRandomPath();
    }
        
    private void BuildEssentialPath()
    {
        int randomY = Random.Range(0, maxBound + 1);
        PathTile ePath = new PathTile(TileType.essential, new Vector2(0, randomY), minBound, maxBound, gridPositions);
        startPos = ePath.position;

        //boundTracker keeps track when we have reached the Far Right
        int boundTracker = 0;

        while (boundTracker < maxBound)
        {
            //Add present position ePath to gridPositions
            gridPositions.Add(ePath.position, TileType.empty);

            //Select random surrounding tile
            int adjacentTileCount = ePath.adjacentPathTiles.Count;
            int randomIndex = Random.Range(0, adjacentTileCount);
            Vector2 nextEPathPos;

            if(adjacentTileCount > 0)
            {
                nextEPathPos = ePath.adjacentPathTiles[randomIndex];
            }
            else
            {
                break;
            }
            PathTile nextEPath = new PathTile(TileType.essential, nextEPathPos, minBound, maxBound, gridPositions);
            if(nextEPath.position.x > ePath.position.x || (nextEPath.position.x == maxBound - 1 && Random.Range(0, 2) == 1))
            {
                ++boundTracker;
            }

            ePath = nextEPath;
        }

        if (!gridPositions.ContainsKey(ePath.position))
        {
            gridPositions.Add(ePath.position, TileType.empty);
        }

        endPos = new Vector2(ePath.position.x, ePath.position.y);
    }

    private void BuildRandomPath()
    {
        //Create a pathQueue as a List
        List<PathTile> pathQueue = new List<PathTile>();

        //Copy the essentialPath into pathQueue
        foreach (KeyValuePair<Vector2, TileType> tile in gridPositions)
        {
            Vector2 tilePos = new Vector2(tile.Key.x, tile.Key.y);
            pathQueue.Add(new PathTile(TileType.random, tilePos, minBound, maxBound, gridPositions));
        }

        //Check if there are adjacent Tiles
        //Every tile has (20%) 1 in 9 chance of becoming a Chamber
        pathQueue.ForEach(delegate (PathTile tile)
       {
           int adjacentTileCount = tile.adjacentPathTiles.Count;
           if (adjacentTileCount != 0)
           {
               if (Random.Range(0, 9) == 1)
               {
                  BuildRandomChamber(tile);
               }

               //Create winding random paths
               else if (Random.Range(0, 1) == 1 || (tile.type == TileType.random && adjacentTileCount > 1))
               {
                   int randomIndex = Random.Range(0, adjacentTileCount);

                   Vector2 newRPathPos = tile.adjacentPathTiles[randomIndex];

                   //If the tile is not yet in the Dictionary -> Add it
                   if (!gridPositions.ContainsKey(newRPathPos))
                   {
                       if(Random.Range(0, 70)  == 1)
                       {
                           gridPositions.Add(newRPathPos, TileType.enemy);
                       }
                       else if (Random.Range(0, 25) == 1)
                       {
                           gridPositions.Add(newRPathPos, TileType.wall);
                       }
                       else
                       {
                           gridPositions.Add(newRPathPos, TileType.empty);
                       }

                       PathTile newRPath = new PathTile(TileType.random, newRPathPos, minBound, maxBound, gridPositions);
                       pathQueue.Add(newRPath);
                   }
               }
           }
       });
    }

    private void BuildRandomChamber(PathTile tile)
    {
        //Every chamber is 3x3
        int chamberSize = 3,
            adjacentTileCount = tile.adjacentPathTiles.Count,
            randomIndex = Random.Range(0, adjacentTileCount);
        Vector2 chamberOrigin = tile.adjacentPathTiles[randomIndex];
        
        for(int x = (int)chamberOrigin.x; x < chamberOrigin.x + chamberSize; x++)
        {
            for(int y = (int)chamberOrigin.y; y < chamberOrigin.y + chamberSize; y++)
            {
                Vector2 chamberTilePos = new Vector2(x, y);
                if(!gridPositions.ContainsKey(chamberTilePos) && 
                    chamberTilePos.x < maxBound && chamberTilePos.x > 0 && 
                    chamberTilePos.y < maxBound && chamberTilePos.y > 0)
                {
                    if(Random.Range(0, 4) == 1)
                    {
                        gridPositions.Add(chamberTilePos, TileType.chest);

                    }
                    else
                    {
                        gridPositions.Add(chamberTilePos, TileType.empty);

                    }
                }
            } 
        }
    }

}
