using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace SpatialPartitionPattern
{
    public class GameController : MonoBehaviour
    {
        public GameObject friendlyObj;
        public GameObject enemyObj;

        // Change materials to detect which enemy is the closest
        public Material enemyMaterial;
        public Material closestEnemyMaterial;

        // To get a cleaner workspace, parent all soldiers to these empty gameobjects
        public Transform enemyParent;
        public Transform friendlyParent;

        // Store all soldiers in these lists
        List<Soldier> enemySoldiers = new List<Soldier>();
        List<Soldier> friendlySoldiers = new List<Soldier>();

        // Save the closest enemies to easier change back its material
        List<Soldier> closestEnemies = new List<Soldier>();

        // Grid data
        float mapWidth = 50f;
        int cellSize = 10;

        // Number of soldiers on each team
        int numberOfSoldiers = 100;

        // The Spatial Partition Grid
        Grid grid;

        // Timer elements
        [SerializeField] TextMeshProUGUI timerText;
        float elapsedTime;

        [SerializeField] private Toggle partitionToggle;
        private bool lastPartitionState;

        void Start()
        {
            // Create a new grid
            grid = new Grid((int)mapWidth, cellSize);

            // Add random enemies and friendly and store them in a list
            for(int i = 0; i < numberOfSoldiers; i++)
            {
                // Give the enemy a random position
                Vector3 randomPos = new Vector3(Random.Range(0f, mapWidth), 0.5f, Random.Range(0f, mapWidth));

                // Create a new enemy
                GameObject newEnemy = Instantiate(enemyObj, randomPos, Quaternion.identity) as GameObject;

                // Add the enemy to a list
                enemySoldiers.Add(new Enemy(newEnemy, mapWidth, grid));

                // Parent it
                newEnemy.transform.parent = enemyParent;


                // Give the friendly a random position
                randomPos = new Vector3(Random.Range(0f, mapWidth), 0.5f, Random.Range(0f, mapWidth));

                // Create a new friendly
                GameObject newFriendly = Instantiate(friendlyObj, randomPos, Quaternion.identity) as GameObject;

                // Add the friendly to a list
                friendlySoldiers.Add(new Friendly(newFriendly, mapWidth));

                // Parent it
                newFriendly.transform.parent = friendlyParent;
            }
            lastPartitionState = partitionToggle.isOn; // set initial toggle state
        }

        void Update()
        {
            if(partitionToggle.isOn && !lastPartitionState) // did we just turn spatial partitoning back on
            {
                grid.Clear();
                
                foreach(Soldier enemy in enemySoldiers)
                {
                    grid.Add(enemy);
                }
            }
            lastPartitionState = partitionToggle.isOn;

            // Move the enemies
            for(int i = 0; i < enemySoldiers.Count; i++)
            {
                enemySoldiers[i].Move();
            }

            // Reset Material of the closest enemies
            foreach(Soldier enemy in closestEnemies)
            {
                enemy.soldierMeshRenderer.material = enemyMaterial;
            }

            // Reset the list with the closest enemies
            closestEnemies.Clear();

            // For each friendly, find the closest enemy and change its color and chase it
            for(int i = 0; i < friendlySoldiers.Count; i++)
            {
                Soldier closestEnemy = null;

                if(partitionToggle.isOn)
                {
                    closestEnemy = grid.FindClosestEnemy(friendlySoldiers[i], true);
                }
                else
                {
                    closestEnemy = FindClosestEnemySlow(friendlySoldiers[i]);
                }
                if (closestEnemy == null)
                {
                    continue;
                }

                closestEnemy.soldierMeshRenderer.material = closestEnemyMaterial;
                closestEnemies.Add(closestEnemy);
                // move in the direction of the enemy
                friendlySoldiers[i].Move(closestEnemy);
            }

            elapsedTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            // Format time to read as 01:01 instead of a long decimal
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        // Find the closest enemy - slow version
        Soldier FindClosestEnemySlow(Soldier soldier)
        {
            Soldier closestEnemy = null;
            float bestDistSqr = Mathf.Infinity;

            // Loop through all enemies
            for (int i = 0; i < enemySoldiers.Count; i++)
            {
                // The distance sqr between the soldier and this enemy
                float distSqr = (soldier.soldierTrans.position - enemySoldiers[i].soldierTrans.position).sqrMagnitude;

                // If the distance is better than the previous best distance, then we have found an enemy that's closer
                if (distSqr < bestDistSqr)
                {
                    bestDistSqr = distSqr;
                    closestEnemy = enemySoldiers[i];
                }
            }

            return closestEnemy;
        }
    }
}
