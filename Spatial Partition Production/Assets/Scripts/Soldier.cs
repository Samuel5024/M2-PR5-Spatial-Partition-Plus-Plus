using UnityEngine;
using System.Collections;

namespace SpatialPartitionPattern
{
    public class Soldier // The soldier base class for enemies and friendly
    {
        public MeshRenderer soldierMeshRenderer; // To change material
        public Transform soldierTrans; // To move the soldier
        protected float walkSpeed; // The speed the soldier is walking with Has to do with the grid, so we can avoid storing all soldiers in an arry
                                   // Instead we are going to use a linked list where all soliders in the cell are linked to each other
        public Soldier previousSoldier;
        public Soldier nextSoldier;

        public virtual void Move() // The enemy doesn't need any outside information
        {

        }

        public virtual void Move(Soldier soldier) // The friendly has to move which soldier is the closest
        {

        }
    }
}