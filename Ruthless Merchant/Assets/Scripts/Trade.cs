﻿using UnityEngine;
using UnityEngine.UI;

namespace RuthlessMerchant
{
    public class Trade : MonoBehaviour
    {
        public static Trade Singleton;

        public Trader Trader;

        public Item Item;

        public float currentPlayerOffer;

        float currentTraderOffer;
        public float CurrentTraderOffer
        {
            get
            {
                return currentTraderOffer;
            }
            set
            {
                currentTraderOffer = (int)value;
            }
        }

        [SerializeField]
        Text traderOffer;

        [SerializeField]
        InputField playerOfferInputField;

        [SerializeField]
        public Text bargainEventsText;

        [SerializeField]
        Slider sliderIrritation;

        [SerializeField]
        Slider sliderSkepticism;

        void Awake()
        {
            Singleton = this;
        }

        public void UpdateUI(bool exits)
        {
            sliderIrritation.value = Trader.irritationTotal;
            sliderSkepticism.value = Trader.skepticismTotal;

            if (!exits)
            {
                playerOfferInputField.text = currentPlayerOffer.ToString();
                traderOffer.text = currentTraderOffer.ToString();
            }
        }

        public void Accept()
        {
            bargainEventsText.text = "Dormammu accepts your offer.";
        }

        public void Abort()
        {
            bargainEventsText.text = "Dormammu tells you to fuck off and rides off with his galaxy-eating unicorn.";
        }
    }
}