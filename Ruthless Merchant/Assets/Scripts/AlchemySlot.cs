﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuthlessMerchant
{
    public class AlchemySlot : InteractiveWorldObject {

        #region Fields ##################################################################

        Ingredient _ingredient;

        #endregion


        #region Properties ##############################################################

        public Ingredient Ingredient
        {
            get { return _ingredient; }
        }

        #endregion


        #region Private Functions #######################################################

        #endregion


        #region Public Functions ########################################################

        public override void Interact(GameObject caller)
        {
            if(!_ingredient)
            {
                caller.GetComponent<Player>().EnterAlchemySlot(this);
            }
            else
            {
                RemoveItem(caller.GetComponent<Player>().Inventory);
            }
        }

        public override void Start()
        {

        }

        public override void Update()
        {

        }

        /// <summary>
        /// Adds an Item to the Alchemyslot
        /// </summary>
        /// <param name="ingredient">ingredient to add</param>
        public void AddItem(Ingredient ingredient)
        {
            _ingredient = ingredient;

            //Sound - AlchemyItems
            FMODUnity.RuntimeManager.PlayOneShot("event:/Characters/Player/Alchemy Items", GameObject.FindGameObjectWithTag("Player").transform.position);
        }

        /// <summary>
        /// Removes the Item from the Slot and adds it to the inventory
        /// </summary>
        /// <param name="inventory">Inventory to add the item to</param>
        public void RemoveItem(Inventory inventory)
        {
            Item item = _ingredient.DeepCopy();
            inventory.Add(item, 1, true);
            _ingredient = null;

            //Sound - AlchemyItems
            FMODUnity.RuntimeManager.PlayOneShot("event:/Characters/Player/Alchemy Items", GameObject.FindGameObjectWithTag("Player").transform.position);
        }

        public void ClearItem()
        {
            _ingredient = null;
        }

        #endregion
    }
}
