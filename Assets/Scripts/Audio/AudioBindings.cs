using Game_Events;
using UnityEngine;

namespace Audio {
    public class AudioBindings : MonoBehaviour {
        [Header("References")]
        [SerializeField] private AudioManager audioManager;
        
        private void OnEnable() {
            GameEvents.OnMatchStarted += OnMatchStarted;
            GameEvents.OnRoundCountdownStarted += OnRoundCountdown;
            GameEvents.OnRoundStarted += OnRoundStarted;
            GameEvents.OnPlayerDied += OnPlayerDied;
            GameEvents.OnRoundEnded += OnRoundEnded;
            GameEvents.OnMatchEnded += OnMatchEnded;
        }

        private void OnDisable() {
            GameEvents.OnMatchStarted -= OnMatchStarted;
            GameEvents.OnRoundCountdownStarted -= OnRoundCountdown;
            GameEvents.OnRoundStarted -= OnRoundStarted;
            GameEvents.OnPlayerDied -= OnPlayerDied;
            GameEvents.OnRoundEnded -= OnRoundEnded;
            GameEvents.OnMatchEnded -= OnMatchEnded;
        }

        private void OnMatchStarted(GameEvents.MatchStart e) {
            audioManager.SetMusic(MusicId.Match);
        }

        private void OnRoundCountdown(GameEvents.RoundCountdownStart e) {
            audioManager.PlaySfx(SfxId.RoundCountdown);
        }

        private void OnRoundStarted(GameEvents.RoundStart e) {
            audioManager.PlaySfx(SfxId.RoundStart);
        }

        private void OnPlayerDied(GameEvents.PlayerDied e) {
            audioManager.PlaySfx(SfxId.Death);
        }

        private void OnRoundEnded(GameEvents.RoundEnd e) {
            audioManager.PlaySfx(SfxId.PointScored);
        }

        private void OnMatchEnded(GameEvents.MatchEnd e) {
            audioManager.PlaySfx(SfxId.Victory);
            audioManager.SetMusic(MusicId.Win);
        }
    }
}