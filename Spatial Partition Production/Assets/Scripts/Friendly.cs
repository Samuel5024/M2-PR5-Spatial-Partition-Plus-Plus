using UnityEngine;
using System.Collections;

namespace SpatialPartitionPattern
{
    public class Friendly : Soldier // The friendly sphere which is chasing the enemy cubes
    {
        public Friendly(GameObject soldierObj, float mapWidth) // init friendly
        {
            this.soldierTrans = soldierObj.transform;
            this.walkSpeed = 2f;
        }

        public override void Move(Soldier closestEnemy)  // Move towards the closest enemy - will always move within its grid
        {
            soldierTrans.rotation = Quaternion.LookRotation(closestEnemy.soldierTrans.position - soldierTrans.position); // Rotate twoards the closest enemy
            soldierTrans.Translate(Vector3.forward * Time.deltaTime * walkSpeed); // Move towards the closest enemy
        }
    }
}