﻿using System.Collections.Generic;
using UnityEngine;

namespace RuthlessMerchant
{

    public class ItemRespawnLogic : MonoBehaviour {

        #region Private Fields

        [Header("Item/Material/Ingredient/Quest Item which should be spawned")]
        [SerializeField]
        [Tooltip("Place desired Prefab here")]
        private GameObject itemToSpawn;

        [Header("Number of Locations which should be empty")]
        [SerializeField]
        private int nrOfEmptyLocations = 10;

        [Header("Time between Respawns in Seconds")]
        [SerializeField]
        private float respawnTime = 5;

        [SerializeField]
        [Tooltip("Tag of the Spawn Locations associated with the Item")]
        private string spawnLocationsTag;

        [SerializeField]
        private float quatX, quatY, quatZ, quatW;

        private Transform spawnPosition;
        private GameObject spawnedItem;
        private static System.Random rnJesus = new System.Random();
        private float currentTimer;
        private int currentlyActiveItems;
        private List<GameObject> emptySpawners = new List<GameObject>();
        private GameObject[] spawnLocations;
        private ContainingItemInformation[] containingItemInformations;
        private int maxNrOfItems;
        #endregion

        #region Gameplay Loop
        /// <summary>
        /// sets the current Timer to th respawn Time to initiate the Spawning of the first items at the start
        /// Gets all SpawnLocations associated with the Item by predefined Tag
        /// </summary>
        void Start()
        {
            currentTimer = 0; //RespawnTime statt 0 (Gregor von Frankenberg)
            spawnLocations = GameObject.FindGameObjectsWithTag(spawnLocationsTag);
            maxNrOfItems = spawnLocations.Length - nrOfEmptyLocations;

            containingItemInformations = new ContainingItemInformation[spawnLocations.Length];
            for (int i = 0; i < spawnLocations.Length; i++)
            {
                containingItemInformations[i] = spawnLocations[i].GetComponent<ContainingItemInformation>();
            }
        }

        /// <summary>
        /// Counts up a Timer if a GameObject or more should be spawned
        /// initiates the spawning algorithm after threshhold is reached and resets the timer afterwards
        /// </summary>
        void Update()
        {
            if (CheckItemsInSpawners() > 0)
                currentTimer -= Time.deltaTime;

            if (currentTimer <= 0.0f)
            {
                InitiateSpawn();
                currentTimer = respawnTime;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks all spawn locations if they contain a spawned GamneObject
        /// resets all used global fields at the start to get current status
        /// </summary>
        /// <returns>the number of items which should be spawned</returns>
        private int CheckItemsInSpawners()
        {
            currentlyActiveItems = 0;
            emptySpawners.Clear();

            for (int i = 0; i < spawnLocations.Length; i++)
            {
                if (containingItemInformations[i].CheckContainingItem(itemToSpawn))
                    currentlyActiveItems++;
                else
                    emptySpawners.Add(spawnLocations[i]);
            }

            int nrOfItemsToSpawn = maxNrOfItems - currentlyActiveItems;

            return nrOfItemsToSpawn;
        }

        /// <summary>
        /// Initiates the Spawning of Items when the Timer reached its threshhold
        /// gets the number of items to spawn and spawns the desired number of items
        /// </summary>
        private void InitiateSpawn()
        {
            int nrOfItemsToSpawn = CheckItemsInSpawners();

            for (int i = 0; i < nrOfItemsToSpawn; i++)
            {
                emptySpawners = SpawnItem(emptySpawners);
            }
        }

        /// <summary>
        /// Instantiates a GameObject at an eligable Location
        /// </summary>
        /// <param name="eligableSpawners">a List of eligable Spawnlocations</param>
        /// <returns>a modified version of the passed List, without the used spawn location to prevent multiple uses of the same location</returns>
        private List<GameObject> SpawnItem(List<GameObject> eligableSpawners)
        {
            GameObject[] eligableSpawnersArray = eligableSpawners.ToArray();
            GameObject selectedSpawnLocation = GetSpawnLocation(eligableSpawnersArray);
            eligableSpawners.Remove(selectedSpawnLocation);

            spawnPosition = selectedSpawnLocation.transform;
            spawnedItem = Instantiate(itemToSpawn, spawnPosition.position, new Quaternion(quatX, quatY, quatZ, quatW));
            return eligableSpawners;
        }

        /// <summary>
        /// returns a random Spawn Location
        /// </summary>
        /// <param name="spawnArray">an array of all eligable spawn locations</param>
        /// <returns>returns the randomly chosen spawn location GameObject</returns>
        public GameObject GetSpawnLocation(GameObject[] spawnArray)
        {
            int spawnLocationIdentifier = rnJesus.Next(0, spawnArray.Length);
            return spawnArray[spawnLocationIdentifier];
        }

        #endregion
    }
}
