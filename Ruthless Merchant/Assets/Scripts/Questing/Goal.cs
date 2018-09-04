﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {


    [SerializeField]
    public string QuestTitle;
    [SerializeField]
    public string Description; //{ get; set; }
    //[SerializeField]
    public int RequiredAmount; //{ get; set; }
    [SerializeField]
    public int Reward;              // reward for the hero
    [SerializeField]
    public float ReputationGain;    // reward for the player

    //[SerializeField]
    //protected List<Transform> Waypoints;

    public bool Completed; //{ get; set; }
    public bool InProgress;
    public int CurrentAmount; //{ get; set; }


    public virtual void Initialize()
    {

    }
    public void Evaluate()
    {
        if (CurrentAmount >= RequiredAmount)
        {
            Complete();
        }
    }

    public void Complete()
    {
        Completed = true;
    }
}
