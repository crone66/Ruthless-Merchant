﻿//---------------------------------------------------------------
// Author: Marcel Croonenbroeck
//
//---------------------------------------------------------------

using System.Collections;
using UnityEngine;

namespace RuthlessMerchant
{
    public abstract class Fighter : NPC
    {
        [Header("NPC Fighter settings")]
        [SerializeField, Tooltip("If the distance to a character is smaller than the hunting distance, the NPC follows the character")]
        [Range(0.0f, 100.0f)]
        protected float huntDistance = 5;

        [SerializeField, Tooltip("If the distance to a character is smaller then the attacking distance, the npc attacks the character")]
        [Range(0.0f, 100.0f)]
        protected float attackDistance = 1.5f;

        public float HuntDistance
        {
            get
            {
                return huntDistance;
            }
        }

        public float AttackDistance
        {
            get
            {
                return attackDistance;
            }
        }

        public override void Start()
        {
            base.Start();
            HealthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
            HealthSystem.OnDeath += HealthSystem_OnDeath;
        }

        private void HealthSystem_OnDeath(object sender, DamageAbleObject.HealthArgs e)
        {
            ParticleSystem fx = BloodManager.GetFreeFX();
            if (fx != null)
            {
                fx.transform.position = transform.position + (Vector3.up * 0.5f);
                fx.transform.rotation = transform.rotation;
                fx.Play();
            }
        }

        public override void Update()
        {
            SetCurrentAction(new ActionIdle(ActionNPC.ActionPriority.None), null);
            base.Update();
        }

        private void HealthSystem_OnHealthChanged(object sender, DamageAbleObject.HealthArgs e)
        {
            if (e.ChangedValue < 0 && e.Sender != null && HealthSystem.Health > 0)
            {
                if (CurrentReactTarget == null || currentReactTarget.IsDying || !IsThreat(CurrentReactTarget.gameObject) || Vector3.Distance(CurrentReactTarget.transform.position, transform.position) > Vector3.Distance(e.Sender.transform.position, transform.position))
                {
                    currentReactTarget = e.Sender;
                    reactionState = TargetState.None;
                    reactionState = reactionState.SetFlag(TargetState.InView);
                    reactionState = reactionState.SetFlag(TargetState.IsThreat);
                    SetCurrentAction(new ActionHunt(ActionNPC.ActionPriority.Medium), e.Sender.gameObject, true, true);
                }
            }
        }

        public override void React(Character character, bool isThreat)
        {
            if (isThreat)
            {
                float distance = Vector3.Distance(transform.position, character.transform.position);
                if (distance <= attackDistance)
                {
                    if (CurrentAction == null || !(CurrentAction is ActionAttack))
                        SetCurrentAction(new ActionAttack(), character.gameObject);
                }
                else if(distance < huntDistance)
                {
                    if (CurrentAction == null || !(CurrentAction is ActionHunt))
                        SetCurrentAction(new ActionHunt(), character.gameObject, true, true);
                }
            }
        }

        public override void React(Item item)
        {
            //TODO: Pickup item
            Reacting = true;
        }
    }
}