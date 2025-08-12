using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audio {
    public sealed class AudioManager : MonoBehaviour
    {
        [Header("Clips & Audio Sources")]
        [SerializeField] private List<SfxEntry> sfx = new();
        [Tooltip("Should be ordered as the MusicId Enum")]
        [SerializeField] private List<AudioClip> musicClips = new();
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfx2DSource;

        private Dictionary<SfxId, SfxEntry> _map;

        private void Awake() {
            _map = sfx.ToDictionary(sfxEntry => sfxEntry.id);
        }

        public void PlaySfx(SfxId id) {
            if (!_map.TryGetValue(id, out var sfxEntry) || sfxEntry.clip == null) return;
            
            sfx2DSource.PlayOneShot(sfxEntry.clip);
        }

        public void SetMusic(MusicId id, float fade = 0.5f) {
            var clip = musicClips.ElementAtOrDefault((int)id);
            if (!clip) return;
            
            StopAllCoroutines();
            
            StartCoroutine(FadeTo(clip, fade));
        }

        private IEnumerator FadeTo(AudioClip next, float t) {
            var startVolume = musicSource.volume;

            for (float a = 0; a < t; a += Time.unscaledDeltaTime) {
                musicSource.volume = Mathf.Lerp(startVolume, 0f, a / t);
                yield return null;
            }
            
            musicSource.clip = next; 
            musicSource.Play();

            for (float a = 0; a < t; a += Time.unscaledDeltaTime) {
                musicSource.volume = Mathf.Lerp(0f, startVolume, a / t);
                yield return null;
            }
        }
    }

    public enum SfxId { RoundCountdown, RoundStart, Death, PointScored, Victory, UIClick }
    public enum MusicId { Menu, Match, Win }
}