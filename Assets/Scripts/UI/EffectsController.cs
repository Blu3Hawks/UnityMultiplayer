using System;
using System.Collections.Generic;
using Game_Events;
using Player;
using Projectiles;
using UnityEngine;

namespace UI
{
    public class EffectsController : MonoBehaviour
    {
        [SerializeField] private List<ParticleSystem> winEffects;

        [SerializeField] private ParticleSystem hitEffect;
        private void Start()
        {
            GameEvents.OnMatchEnded += PlayWinEffects;
            Projectile.OnProjectileHit += PlayerHitEffect;
        }

        private void OnDestroy()
        {
            GameEvents.OnMatchEnded -= PlayWinEffects;
            Projectile.OnProjectileHit -= PlayerHitEffect;
            
        }

        private void PlayWinEffects(GameEvents.MatchEnd end)
        {
            foreach (ParticleSystem effect in winEffects)
            {
                effect.Play();
            }
        }

        private void PlayerHitEffect(Vector3 playerPosition)
        {
            Instantiate(hitEffect, playerPosition, Quaternion.identity);
        }
    }
}