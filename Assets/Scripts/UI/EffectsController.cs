using System;
using System.Collections.Generic;
using Game_Events;
using UnityEngine;

namespace UI
{
    public class EffectsController : MonoBehaviour
    {
        [SerializeField] private List<ParticleSystem> winEffects;

        private void Start()
        {
            GameEvents.OnMatchEnded += PlayEffects;
        }

        private void OnDestroy()
        {
            GameEvents.OnMatchEnded -= PlayEffects;
        }

        private void PlayEffects(GameEvents.MatchEnd end)
        {
            foreach (ParticleSystem effect in winEffects)
            {
                effect.Play();
            }
        }
    }
}