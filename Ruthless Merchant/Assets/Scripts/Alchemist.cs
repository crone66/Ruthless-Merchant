﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RuthlessMerchant
{
    public class Alchemist : InteractiveWorldObject
    {
        #region Fields ##################################################################

        [SerializeField, TextArea, Tooltip("The Description, that will be added to a potion")]
        string potionDescription;

        [SerializeField]
        Sprite[] potionSprites;

        [SerializeField]
        ParticleSystem effect;

        private AlchemySlot[] alchemySlots;

        [SerializeField] private UnityEvent onSuccesfullAlchemy;

        [SerializeField] private UnityEvent onSuccesfullAlchemyAchievment;

        #endregion


        #region Properties ##############################################################

        #endregion


        #region Private Functions #######################################################

        private void Awake()
        {
            alchemySlots = GetComponentsInChildren<AlchemySlot>();
        }

        /// <summary>
        /// Sets the rarity of the potion, depending on the total power
        /// </summary>
        private ItemRarity Rarity(int atk, int def, int speed, int hp)
        {
            int totalPower = Mathf.Abs(atk) + Mathf.Abs(def) + Mathf.Abs(speed) + Mathf.Abs(hp);

            if (totalPower >= 3)
            {
                return ItemRarity.Selten;
            }
            if (totalPower >= 2)
            {
                return ItemRarity.Ungewöhnlich;
            }
            return ItemRarity.Üblich;
        }

        /// <summary>
        /// Creates the Value for the Potion
        /// </summary>
        /// <param name="atk">ATK-Parameter of the potion</param>
        /// <param name="def">DEF-Parameter of the potion</param>
        /// <param name="speed">Speed-Parameter of the potion</param>
        /// <param name="hp">Health Regeneration-Parameter of the potion</param>
        /// <returns>final value as int</returns>
        int PotionValue(int atk, int def, int speed, int hp)
        {
            int value = 3;

            for(int i = Mathf.Abs(atk); i > 0; i--)
            {
                value += i;
            }
            for (int i = Mathf.Abs(def); i > 0; i--)
            {
                value += i;
            }
            for (int i = Mathf.Abs(speed); i > 0; i--)
            {
                value += i;
            }
            for (int i = Mathf.Abs(hp); i > 0; i--)
            {
                value += i;
            }

            return value;
        }

        /// <summary>
        /// Creates a Name for the Potion
        /// </summary>
        /// <param name="atk">ATK-Parameter of the potion</param>
        /// <param name="def">DEF-Parameter of the potion</param>
        /// <param name="speed">Speed-Parameter of the potion</param>
        /// <param name="hp">Health Regeneration-Parameter of the potion</param>
        /// <returns>final name as string</returns>
        string PotionName(int atk, int def, int speed, int hp)
        {
            string name = "Potion";

            if (atk != 0)
            {
                name += GetPowerAsString(atk);
                if (atk > 0)
                {
                    name += " STR";
                }
                else
                {
                    name += " WEAK";
                }
            }
            if (def != 0)
            {
                if (atk != 0)
                {
                    name += " &";
                }
                name += GetPowerAsString(def);
                if (def > 0)
                {
                    name += " DEF";
                }
                else
                {
                    name += " FRAG";
                }
            }
            if (speed != 0)
            {
                if (atk != 0 || def != 0)
                {
                    name += " &";
                }
                name += GetPowerAsString(speed);
                if (speed > 0)
                {
                    name += " SPED";
                }
                else
                {
                    name += " SLOW";
                }
            }
            if (hp != 0)
            {
                if (atk != 0 || def != 0 || speed != 0)
                {
                    name += " &";
                }
                name += GetPowerAsString(hp);
                if (hp > 0)
                {
                    name += " HEAL";
                }
                else
                {
                    name += " HURT";
                }
            }
            return name;
        }

        /// <summary>
        /// Creates a part-name that describes the strength of the potion
        /// </summary>
        /// <param name="power">strength value</param>
        /// <returns>returns the strength as string</returns>
        string GetPowerAsString(int power)
        {
            if (Mathf.Abs(power) > 2)
            {
                return " 3x";
            }

            else if (Mathf.Abs(power) > 1)
            {
                return " 2x";
            }
            return "";
        }

        /// <summary>
        /// Chooses a random sprite for the potion
        /// </summary>
        /// <returns>returns a sprite for the potion</returns>
        private Sprite RandomSprite()
        {
            if(potionSprites.Length > 0)
            {
                return potionSprites[Random.Range(0, potionSprites.Length)];
            }
            return null;
        }

        #endregion


        #region Public Functions ########################################################

        /// <summary>
        /// used when interacting with the Alchemystation. adds a potion to the inventory if at least one ingredient was found in the alchemyslots
        /// </summary>
        /// <param name="caller"></param>
        public override void Interact(GameObject caller)
        {
            float atk = 0;
            int def = 0;
            float speed = 0;
            float hp = 0;
            int reg = 0;

            int atkCount = 0;
            int defCount = 0;
            int speedCount = 0;
            int hpCount = 0;

            bool hasItem = false;
            for(int i = 0; i < alchemySlots.Length; i++)
            {
                if(alchemySlots[i].Ingredient)
                {
                    atk += alchemySlots[i].Ingredient.AttackSpeedBuff;
                    def += alchemySlots[i].Ingredient.DefenseBuff;
                    hp += alchemySlots[i].Ingredient.HealthBuff;
                    speed += alchemySlots[i].Ingredient.MovementBuff;
                    reg += alchemySlots[i].Ingredient.RegenerationBuff;

                    if (alchemySlots[i].Ingredient.IngredientType == IngredientType.Schwertgras)
                        atkCount++;
                    else if(alchemySlots[i].Ingredient.IngredientType == IngredientType.Stichelpilz)
                        atkCount--;
                    else if (alchemySlots[i].Ingredient.IngredientType == IngredientType.Segensblüte)
                        hpCount++;
                    else if (alchemySlots[i].Ingredient.IngredientType == IngredientType.Grabesmoos)
                        hpCount--;
                    else if (alchemySlots[i].Ingredient.IngredientType == IngredientType.Windfarn)
                        speedCount++;
                    else if (alchemySlots[i].Ingredient.IngredientType == IngredientType.KnarzigeKnolle)
                        speedCount--;
                    else if (alchemySlots[i].Ingredient.IngredientType == IngredientType.SteinernderRuestling)
                        defCount++;
                    else if (alchemySlots[i].Ingredient.IngredientType == IngredientType.GemeinerDornling)
                        defCount--;

                    hasItem = true;
                    alchemySlots[i].ClearItem();
                    alchemySlots[i].DoExplosive();
                }
            }
            if(!hasItem)
            {
                return;
            }

            string itemName = PotionName(atkCount, defCount, speedCount, hpCount);

            //Sets the potion-parameters
            Potion potion = new GameObject(itemName).AddComponent<Potion>();
            potion.CreatePotion(atk, def, speed, hp, reg);
            potion.ItemInfo.ItemName = itemName;
            potion.ItemInfo.ItemLore = potionDescription;
            potion.ItemInfo.ItemType = ItemType.ConsumAble;
            potion.ItemInfo.ItemRarity = Rarity(atkCount, defCount, speedCount, hpCount);
            potion.ItemInfo.ItemSprite = RandomSprite();
            potion.ItemInfo.MaxStackCount = 1;
            potion.ItemInfo.ItemValue = PotionValue(atkCount, defCount, speedCount, hpCount);

            //Adds potion to infentory
            Player p = caller.GetComponent<Player>();
            p.Inventory.Add(potion.DeepCopy(), 1, true);
            onSuccesfullAlchemy.Invoke();
            onSuccesfullAlchemyAchievment.Invoke();
            Destroy(potion.gameObject);
            effect.Play();
        }

        public override void Start()
        {

        }

        public override void Update()
        {

        }

        #endregion
    }
}