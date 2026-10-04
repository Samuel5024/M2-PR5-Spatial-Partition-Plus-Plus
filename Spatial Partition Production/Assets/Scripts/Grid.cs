using UnityEngine;
using System.Collections;

namespace SpatialPartitionPattern
{
    public class Grid
    {
        private int cellSize; // Need this to convert from world coordinate position to cell position
        Soldier[,] cells; // This is the actual grid, where a soldier is in each cell
                          // Each individual soldier links to other soldiers in the same cell

        public Grid(int mapWidth, int cellSize) // Init the grid
        {
            this.cellSize = cellSize;
            int numberOfCells = mapWidth / cellSize;
            cells = new Soldier[numberOfCells, numberOfCells];
        }

        public void Add(Soldier soldier) // Add a unity to the grid
        {
            int cellX = (int)(soldier.soldierTrans.position.x / cellSize); // Determine which grid cell the soldier is in
            int cellZ = (int)(soldier.soldierTrans.position.z / cellSize);

            soldier.previousSoldier = null; // Add the soldier to the front of the list for the cell it's in
            soldier.nextSoldier = cells[cellX, cellZ];

            cells[cellX, cellZ] = soldier; // Associate this cell with this soldier

            if(soldier.nextSoldier != null)
            {
                soldier.nextSoldier.previousSoldier = soldier; // Set this soldier to be the previous soldier of the next soldier of this soldier (linked lists ftw)
            }
        }

        public Soldier FindClosestEnemy(Soldier friendlySoldier)
        {
            int cellX = (int)(friendlySoldier.soldierTrans.position.x / cellSize); // Determine which grid cell the friendly soldier is in
            int cellZ = (int)(friendlySoldier.soldierTrans.position.z / cellSize);

            Soldier enemy = cells[cellX, cellZ]; // Get the first enemy in grid

            Soldier closestSoldier = null; // Find the closest soldier of all in the linked list
            float bestDistSqr = Mathf.Infinity;

            while(enemy != null) // Loop through the linked list
            {
                if(enemy == null || enemy.soldierTrans == null)
                {
                    enemy = enemy.nextSoldier;
                    continue;
                }

                float distSqr = (enemy.soldierTrans.position - friendlySoldier.soldierTrans.position).sqrMagnitude; // The distance sqr between the soldier and this enemy

                if(distSqr < bestDistSqr) // If this distance is better than the previous best distance then we have found an enemy that's closer
                {
                    bestDistSqr = distSqr;
                    closestSoldier = enemy;
                }

                enemy = enemy.nextSoldier; // Get next enemy in the list
            }

            return closestSoldier;
        }

        public void Move(Soldier soldier, Vector3 oldPos) // A soldier in the grid has moved, so see if we need to update in which grid the soldier is
        {
            int oldCellX = (int)(oldPos.x / cellSize); // See which cell it was in
            int oldCellZ = (int)(oldPos.z / cellSize);

            int cellX = (int)(soldier.soldierTrans.position.x / cellSize); // See which cell it is in now
            int cellZ = (int)(soldier.soldierTrans.position.z / cellSize);

            if(oldCellX == cellX && oldCellZ == cellZ)
            {
                return;
            }

            if(soldier.previousSoldier != null) // Unlink it from the list of its old cell
            {
                soldier.previousSoldier.nextSoldier = soldier.nextSoldier;
            }

            if(soldier.nextSoldier != null)
            {
                soldier.nextSoldier.previousSoldier = soldier.previousSoldier;
            }

            if (cells[oldCellX, oldCellZ] == soldier) // If it's the head of a list, remove it
            {
                cells[oldCellX, oldCellZ] = soldier.nextSoldier;
            }

            Add(soldier); // Add it back to the grid at its new cell
        }
    }
}