﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RuthlessMerchant
{
    public abstract class TradeAbstract : MonoBehaviour
    {
        public static TradeAbstract Singleton;

        #if UNITY_EDITOR
        [ReadOnly]
        #endif
        public int RealValue;

        public List<InventoryItem> ItemsToSell;

        [System.NonSerialized]
        public bool Exit = false;

        #region Serialized Fields

        [SerializeField]
        protected float weightsDeltaModifier;

        #if UNITY_EDITOR
        [ReadOnly]
        #endif
        [SerializeField]
        protected int nextPlayerOffer;

        #region GUI For debugging purposes

        [SerializeField]
        public Text TradeDialogue;

        [SerializeField]
        protected Text valueText;

        [SerializeField]
        protected Slider sliderIrritation;

        [SerializeField]
        protected Text sliderIrritationNumber;

        [SerializeField]
        protected Slider sliderSkepticism;

        [SerializeField]
        protected Text sliderSkepticismNumber;

        [SerializeField]
        protected Text traderOffer;

        [SerializeField]
        protected Text nextPlayerOfferText;

        [SerializeField]
        protected Text currentPlayerOfferText;

        #endregion

        [SerializeField]
        protected Transform connector;

        [SerializeField]
        protected Transform platePositionPlayer;

        [SerializeField]
        protected Transform platePositionTrader;

        protected Vector3 neutralConnectorRotation;

        public Transform TraderZone;

        public Transform PlayerZone;

        [SerializeField]
        protected GameObject TradeObjectsParent;

        [SerializeField]
        float connectorSensitivity = 400;

        // TODO: Delete this.
        [SerializeField]
        protected int defaultValue = 35;

        #endregion

        protected float NeutralPositionY;

        protected List<List<GameObject>> weightsPlayer;
        protected List<List<GameObject>> weightsTrader;

        protected bool initialized = false;

        protected float exitTimer = 0;

        public abstract void Initialize(int realValue);

        public abstract float GetCurrentPlayerOffer();
        public abstract float GetCurrentTraderOffer();
        public abstract void UpdateCurrentTraderOffer(float offer);
        public abstract int GetPlayerOffersCount();
        public abstract void ModifyOffer();

        protected abstract void Accept();
        public abstract void Abort();
        public abstract void Quit();

        protected virtual void Awake()
        {
            Singleton = this;
        }

        /// <summary>
        /// Gets the weight children of a specified weightParent in the scene.
        /// </summary>
        /// <param name="weightsParent">The weightsParent in which to look for weights.</param>
        /// <returns>A list with four GameObject Lists. Each List includes a weight group.</returns>
        protected List<List<GameObject>> GetPresentWeights(Transform weightsParent)
        {
            List<GameObject> weights1 = new List<GameObject>();
            List<GameObject> weights5 = new List<GameObject>();
            List<GameObject> weights10 = new List<GameObject>();
            List<GameObject> weights50 = new List<GameObject>();

            UniqueIDGenerator[] weights = weightsParent.GetComponentsInChildren<UniqueIDGenerator>(true);

            foreach (UniqueIDGenerator weight in weights)
            {
                if (weight.name == "Weight1")
                {
                    weights1.Add(weight.gameObject);
                }

                else if (weight.name == "Weight5")
                {
                    weights5.Add(weight.gameObject);
                }

                else if (weight.name == "Weight10")
                {
                    weights10.Add(weight.gameObject);
                }

                else if (weight.name == "Weight50")
                {
                    weights50.Add(weight.gameObject);
                }
            }

            List<List<GameObject>> result = new List<List<GameObject>>
            {
                weights1,
                weights5,
                weights10,
                weights50
            };

            return result;
        }

        /// <summary>
        /// Updates the weight representation in the scene.
        /// </summary>
        /// <param name="weights">The weight group to be updated (either weightsPlayer or weightsTrader).</param>
        /// <param name="offer">The offer to be represented.</param>
        protected void UpdateWeights(List<List<GameObject>> weights, int offer)
        {
            int[] targetWeights = GetTargetWeights(offer);

            UpdateWeight(weights, targetWeights, 0);
            UpdateWeight(weights, targetWeights, 1);
            UpdateWeight(weights, targetWeights, 2);
            UpdateWeight(weights, targetWeights, 3);

            float traderOffer;

            if (GetCurrentTraderOffer() == -1)
                traderOffer = RealValue;

            else
                traderOffer = GetCurrentTraderOffer();

            float playerTraderOfferDelta = ((float)nextPlayerOffer - (int)traderOffer);
            float ratioDelta = (float)nextPlayerOffer / (int)traderOffer - 1;

            if (ratioDelta != Double.NaN)
                playerTraderOfferDelta = (playerTraderOfferDelta + ratioDelta) / 2 / weightsDeltaModifier;

            else
                playerTraderOfferDelta /= weightsDeltaModifier;

            if (playerTraderOfferDelta > 0.2f)
                playerTraderOfferDelta = 0.2f;

            else if (playerTraderOfferDelta < -0.2f)
                playerTraderOfferDelta = -0.2f;

            if(GetCurrentTraderOffer() == 0)
                playerTraderOfferDelta = 0;

            Vector3 playerDelta = new Vector3(0, -PlayerZone.position.y + NeutralPositionY - playerTraderOfferDelta, 0);
            Vector3 traderDelta = new Vector3(0, -TraderZone.position.y + NeutralPositionY + playerTraderOfferDelta, 0);

            Debug.Log(playerTraderOfferDelta);

            // Trade?
            if (gameObject.GetComponent<Trade>() != null)
            {
                float newX = -90 + playerTraderOfferDelta * connectorSensitivity;

                if (newX > -70)
                    newX = -70;
                else if (newX < -110)
                    newX = -110;

                connector.localEulerAngles = new Vector3(newX, -90, 90); 
                PlayerZone.position = platePositionPlayer.position;
                TraderZone.position = platePositionTrader.position;
            }
        }

        /// <summary>
        /// Updates a single weight group in the scene.
        /// </summary>
        /// <param name="weights">The weight group to be updated.</param>
        /// <param name="targetWeights">The array of all target weight groups.</param>
        /// <param name="weightIndex">The array index that specifies the weight group</param>
        protected void UpdateWeight(List<List<GameObject>> weights, int[] targetWeights, int weightIndex)
        {
            for (int x = weights[weightIndex].Count - 1; x >= targetWeights[weightIndex]; x--)
            {
                weights[weightIndex][x].SetActive(false);
            }

            for (int x = 0; x < targetWeights[weightIndex]; x++)
            {
                weights[weightIndex][x].SetActive(true);
            }
        }

        /// <summary>
        /// Calculates how an offer should be represented in weights.
        /// </summary>
        /// <param name="offer">The amount and type of weights the offer would be represented with.</param>
        /// <returns>An array with four integers, every one of them describes how many weights in their weight group should be present.</returns>
        protected int[] GetTargetWeights(int offer)
        {
            int w50 = (int)((float)offer / 50f);
            int w10 = (int)((float)(offer - w50 * 50) / 10f);
            int w5 = (int)((float)(offer - w50 * 50 - w10 * 10) / 5f);
            int w1 = offer - w50 * 50 - w10 * 10 - w5 * 5;

            return new int[]
            {
                w1,
                w5,
                w10,
                w50
            };
        }
    }
}
