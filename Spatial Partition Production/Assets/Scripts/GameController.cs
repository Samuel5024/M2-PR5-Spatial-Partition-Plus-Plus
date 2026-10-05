using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace SpatialPartitionPattern
{
    public class GameController : MonoBehaviour
    {
        public GameObject friendlyObj;
        public GameObject enemyObj;
        public Material enemyMaterial;
        public int totalEnemies;
        public int totalFriendlies;
        public Material closestEnemyMaterial; // Change materials to detect which enemy is the closest
        public Transform enemyParent; // To get a cleaner workspace, parent all soldiers to these empty gameobjects
        public Transform friendlyParent;
        List<Soldier> enemySoldiers = new List<Soldier>(); // Store all soldiers in these lists
        List<Soldier> friendlySoldiers = new List<Soldier>();
        List<Soldier> closestEnemies = new List<Soldier>(); // Save the closest enemies to easier change back its material

        private float mapWidth = 50f; // Grid data
        private int cellSize = 10;
        private int numberOfSoldiers = 100; // Number of soldiers on each team
        private bool isRestarting = false;

        [SerializeField] TextMeshProUGUI timerText; // Timer elements
        public TextMeshProUGUI enemiesRemainingText;
        public TextMeshProUGUI friendliesRemainingText;
        private float elapsedTime;

        [SerializeField] private Toggle partitionToggle; // Partition Toggle

        Grid grid; // The Spatial Partition Grid


        void Start()
        {
            grid = new Grid((int)mapWidth, cellSize); // Create a new grid

            for(int i = 0; i < numberOfSoldiers; i++) // Add random enemies and friendly and store them in a list
            {
                Vector3 randomPos = new Vector3(Random.Range(0f, mapWidth), 0.5f, Random.Range(0f, mapWidth)); // Give the enemy a random position
                GameObject newEnemy = Instantiate(enemyObj, randomPos, Quaternion.identity) as GameObject; // Create a new enemy
                enemySoldiers.Add(new Enemy(newEnemy, mapWidth, grid)); // Add the enemy to a list
                newEnemy.transform.parent = enemyParent; // Parent it

                randomPos = new Vector3(Random.Range(0f, mapWidth), 0.5f, Random.Range(0f, mapWidth)); // Give the friendly a random position
                GameObject newFriendly = Instantiate(friendlyObj, randomPos, Quaternion.identity) as GameObject; // Create a new friendly
                friendlySoldiers.Add(new Friendly(newFriendly, mapWidth)); // Add the friendly to a list
                newFriendly.transform.parent = friendlyParent;// Parent it
            }
        }

        void Update()
        {
            enemySoldiers.RemoveAll(enemy => enemy == null || enemy.soldierTrans == null); // Remove destroyed soldiers first
            friendlySoldiers.RemoveAll(friendly => friendly == null || friendly.soldierTrans == null);
            
            totalEnemies = enemySoldiers.Count;
            totalFriendlies = friendlySoldiers.Count;
            
            for(int i = 0; i < enemySoldiers.Count; i++) // Move the enemies
            {
                if (enemySoldiers[i] != null && enemySoldiers[i].soldierTrans != null)
                {
                    enemySoldiers[i].Move();
                }
            }

            for(int i = 0; i < closestEnemies.Count; i++) // Reset material of the closest enemies
            {
                if (closestEnemies[i] != null && closestEnemies[i].soldierMeshRenderer != null) // Check if the enemy still exists before resetting material
                {
                    closestEnemies[i].soldierMeshRenderer.material = enemyMaterial;
                }
            }
            closestEnemies.Clear(); 

            for(int i = 0; i < friendlySoldiers.Count; i++) // Move the friendlies
            {
                if (friendlySoldiers[i] == null || friendlySoldiers[i].soldierTrans == null)
                {
                    continue;
                }

                Soldier closestEnemy = null;

                if(partitionToggle.isOn)
                {
                    closestEnemy = grid.FindClosestEnemy(friendlySoldiers[i]); 
                }
                else
                {
                    closestEnemy = FindClosestEnemySlow(friendlySoldiers[i]);
                }

                if(closestEnemy != null && closestEnemy.soldierTrans != null) // Only move and change materials if a valid enemy was found
                {
                    if (closestEnemy.soldierMeshRenderer != null)
                    {
                        closestEnemy.soldierMeshRenderer.material = closestEnemyMaterial;
                    }
                    closestEnemies.Add(closestEnemy);
                    friendlySoldiers[i].Move(closestEnemy); 
                }
            }

            elapsedTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); 

            enemiesRemainingText.text = string.Format($"{totalEnemies}");
            friendliesRemainingText.text = string.Format($"{totalFriendlies}");

            if (totalEnemies == 0 || totalFriendlies == 0)
            {
                isRestarting = true;
                StartCoroutine(Restart());
            }
        }

        Soldier FindClosestEnemySlow(Soldier soldier) // Find the closest enemy - slow version
        {
            Soldier closestEnemy = null;
            float bestDistSqr = Mathf.Infinity;

            for (int i = 0; i < enemySoldiers.Count; i++) // Loop through all enemies
            {
                if(enemySoldiers[i] == null || enemySoldiers[i].soldierTrans == null) // Check for any destroyed enemies or transforms
                {
                    continue;
                }
                
                float distSqr = (soldier.soldierTrans.position - enemySoldiers[i].soldierTrans.position).sqrMagnitude; // The distance sqr between the soldier and this enemy

                if (distSqr < bestDistSqr) // If the distance is better than the previous best distance, then we have found an enemy that's closer
                {
                    bestDistSqr = distSqr;
                    closestEnemy = enemySoldiers[i];
                }
            }
            return closestEnemy;
        }

        public IEnumerator Restart()
        {
            yield return new WaitForSeconds(2.5f);
            SceneManager.LoadScene("SpatialPartition");
        }
    }
}