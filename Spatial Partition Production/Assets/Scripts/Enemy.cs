using UnityEngine;
using System.Collections;

namespace SpatialPartitionPattern
{
    public class Enemy : Soldier // The enemy cube being chased by the spheres
    {
        Vector3 currentTarget; // The position the soldier is heading for when moving
        Vector3 oldPos; // The position the soldier had before it moved, so we can see if it should change cell
        float mapWidth; // The width of the map to generate random coordinates within the map
        Grid grid; // The grid

        public Enemy(GameObject soldierObj, float mapWidth, Grid grid) //Init enemy
        {
            // Save what we need to save
            this.soldierTrans = soldierObj.transform;
            this.soldierMeshRenderer = soldierObj.GetComponent<MeshRenderer>();
            this.mapWidth = mapWidth;
            this.grid = grid;

            grid.Add(this); // Add this unit to the grid

            oldPos = soldierTrans.position; // Init the old pos
            this.walkSpeed = 5f;

            GetNewTarget(); // Give it a random coordinate to move towards
        }

        public override void Move() // Move the cube randomly across the map
        {
            oldPos = soldierTrans.position;
            soldierTrans.Translate(Vector3.forward * Time.deltaTime * walkSpeed); // Move towards the target
            
            grid.Move(this, oldPos); // See if the cube has moved to another cell

            // Save the old position
            oldPos = soldierTrans.position;

            if((soldierTrans.position - currentTarget).magnitude < 1f) // If the soldier has reached the target, find a new target
            {
                GetNewTarget();
            }
        }

        void GetNewTarget() // Give the enemy a new target to move towards and rotate towards that target
        {
            currentTarget = new Vector3(Random.Range(0f, mapWidth), 0.5f, Random.Range(0f, mapWidth));
            soldierTrans.rotation = Quaternion.LookRotation(currentTarget - soldierTrans.position); // Rotate towards the target
        }
    }
}

