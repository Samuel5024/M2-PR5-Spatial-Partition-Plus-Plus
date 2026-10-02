using UnityEngine;
using System.Collections;

namespace SpatialPartitionPattern
{
    public class Grid
    {
        // Need this to convert from world coordinate position to cell position
        int cellSize;
        
        // This is the actual grid, where a soldier is in each cell
        // Each individual soldier links to other soldiers in the same cell
        Soldier[,] cells;

        // Init the grid
        public Grid(int mapWidth, int cellSize)
        {
            this.cellSize = cellSize;
            int numberOfCells = mapWidth / cellSize;
            cells = new Soldier[numberOfCells, numberOfCells];
        }

        public void Clear()
        {
            System.Array.Clear(cells, 0, cells.Length);
        }

        // Add a unity to the grid
        public void Add(Soldier soldier)
        {
            // Determine which grid cell the soldier is in
            int cellX = (int)(soldier.soldierTrans.position.x / cellSize);
            int cellZ = (int)(soldier.soldierTrans.position.z / cellSize);

            // Add the soldier to the front of the list for the cell it's in
            soldier.previousSoldier = null;
            soldier.nextSoldier = cells[cellX, cellZ];

            // Associate this cell with this soldier
            cells[cellX, cellZ] = soldier;

            if(soldier.nextSoldier != null)
            {
                // Set this soldier to be the previous soldier of the next soldier of this soldier (linked lists ftw)
                soldier.nextSoldier.previousSoldier = soldier;
            }
        }

        // Get the closest enemy from the grid
        public Soldier FindClosestEnemy(Soldier friendlySoldier, bool useSpatialPartition)
        {
            if(!useSpatialPartition)
            {
                return null;
            }

            // Determine which grid cell the friendly soldier is in
            int centerCellX = (int)(friendlySoldier.soldierTrans.position.x / cellSize);
            int centerCellZ = (int)(friendlySoldier.soldierTrans.position.z / cellSize);

            // Get the first enemy in grid
            // Soldier enemy = cells[cellX, cellZ];

            // Find the closest soldier of all in the linked list
            Soldier closestSoldier = null;
            float bestDistSqr = Mathf.Infinity;

            // Loop through a 3*3 neighborhood of cells
            for(int x = -1; x <= 1; x++)
            {
                for(int z = -1; z <= 1; z++)
                {
                    int targetCellX = centerCellX + x;
                    int targetCellZ = centerCellZ + z;

                    // GUARD CLAUSE: if the cell is out of bounds, continue
                    if(targetCellX < 0 || targetCellX >= cells.GetLength(0) ||
                        targetCellZ < 0 || targetCellZ >= cells.GetLength(1))
                        {
                            continue;
                        }

                    Soldier enemy = cells[targetCellX, targetCellZ];

                    // Loop through linked list in this cell
                    while(enemy != null)
                    {
                        float distSqr = (enemy.soldierTrans.position - friendlySoldier.soldierTrans.position).sqrMagnitude;
                        
                        if(distSqr >=  bestDistSqr)
                        {
                            enemy = enemy.nextSoldier;
                            continue;
                        }

                        // If code reaches here, we found a closer enemy
                        bestDistSqr = distSqr;
                        closestSoldier = enemy;

                        enemy = enemy.nextSoldier;
                    }
                }
            }
            return closestSoldier;
        }

        // A soldier in the grid has moved, so see if we need to update in which grid the soldier is
        public void Move(Soldier soldier, Vector3 oldPos)
        {

            // See which cell it was in
            int oldCellX = (int)(oldPos.x / cellSize);
            int oldCellZ = (int)(oldPos.z / cellSize);

            // See which cell it is in now
            int cellX = (int)(soldier.soldierTrans.position.x / cellSize);
            int cellZ = (int)(soldier.soldierTrans.position.z / cellSize);

            if(oldCellX == cellX && oldCellZ == cellZ)
            {
                return;
            }

            // Unlink it from the list of its old cell
            if(soldier.previousSoldier != null)
            {
                soldier.previousSoldier.nextSoldier = soldier.nextSoldier;
            }
            if(soldier.nextSoldier != null)
            {
                soldier.nextSoldier.previousSoldier = soldier.previousSoldier;
            }

            // If it's the head of a list, remove it
            if (cells[oldCellX, oldCellZ] == soldier)
            {
                cells[oldCellX, oldCellZ] = soldier.nextSoldier;
            }

            // Add it back to the grid at its new cell
            Add(soldier);
        }
    }
}