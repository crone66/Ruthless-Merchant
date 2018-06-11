﻿using UnityEngine;

namespace RuthlessMerchant
{
    public class Item : InteractiveWorldObject
    {
        private int ownerId;
        public ItemType type;
        private ItemValue[] itemValue;
        private int weight;
        private string itemName = "defaultname";
        private int maxStackCount = 1;

        public int ItemWeight
        {
            get { return weight; }
        }

        public string Name
        {
            get { return itemName; }
        }

        public ItemType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }

        public ItemValue[] ItemValue
        {
            get
            {
                return itemValue;
            }
            set
            {
                itemValue = value;
            }
        }

        public override void Start()
        {

        }

        public override void Update()
        {
        }

        public override void Interact(GameObject caller)
        {
            throw new System.NotImplementedException();
        }

        public void Pickup()
        {
            throw new System.NotImplementedException();
        }

        public int MaxStackCount
        {
            get
            {
                return maxStackCount;
            }
        }

        // Creates deep copy of current item
        internal Item DeepCopy()
        {
            Item otherItem = (Item) this.MemberwiseClone();
            return otherItem;
        }
    }
}