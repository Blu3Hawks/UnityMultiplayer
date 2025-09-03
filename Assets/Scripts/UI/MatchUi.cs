using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game_Events;
using TMPro;
using UnityEngine;

namespace UI {
    public class MatchUi : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text roundText;
        [SerializeField] private TMP_Text countdownText;
        [SerializeField] private TMP_Text centerBannerText;
        [SerializeField] private CanvasGroup centerBannerGroup;

        [Header("Scoreboard")]
        [SerializeField] private Transform scoreboardContent;
        [SerializeField] private ScoreboardRowUI scoreboardRowPrefab;

        private readonly Dictionary<int,string> _names  = new(); // actor -> name
        private readonly Dictionary<int,int> _scores = new(); // actor -> score
        private readonly Dictionary<int,ScoreboardRowUI> _rows = new(); // actor -> row

        private Coroutine _countdownCo;
        private Tween _currentTween;

        private void OnEnable() {
            GameEvents.OnMatchStarted += OnMatchStarted;
            GameEvents.OnRoundCountdownStarted += OnRoundCountdown;
            GameEvents.OnRoundStarted += OnRoundStarted;
            GameEvents.OnPlayerJoined += OnPlayerJoined;
            GameEvents.OnPlayerDied += OnPlayerDied;
            GameEvents.OnRoundEnded += OnRoundEnded;
            GameEvents.OnMatchEnded += OnMatchEnded;
            //GameEvents.OnPlayersSynced += OnPlayersSynced;
            GameEvents.OnScoreSet += OnScoreSet;
        }

        private void OnDisable() {
            GameEvents.OnMatchStarted -= OnMatchStarted;
            GameEvents.OnRoundCountdownStarted -= OnRoundCountdown;
            GameEvents.OnRoundStarted -= OnRoundStarted;
            GameEvents.OnPlayerJoined -= OnPlayerJoined;
            GameEvents.OnPlayerDied -= OnPlayerDied;
            GameEvents.OnRoundEnded -= OnRoundEnded;
            GameEvents.OnMatchEnded -= OnMatchEnded;
            //GameEvents.OnPlayersSynced -= OnPlayersSynced;
            GameEvents.OnScoreSet -= OnScoreSet;
        }

        // Event handlers: data
        private void OnPlayersSynced(GameEvents.PlayersSynced e) {
            _names.Clear(); 
            _scores.Clear();
            foreach (var p in e.Players) {
                _names[p.ActorNumber] = p.Name; 
                _scores[p.ActorNumber] = 0;
            }
            RebuildScoreboard();
        }

        private void OnScoreSet(GameEvents.ScoreSet e) {
            _scores[e.ActorNumber] = e.Score;
            AddOrUpdateRow(e.ActorNumber);
            ResortAndHighlight();
        }

        // Event handlers: flow/UI
        private void OnMatchStarted(GameEvents.MatchStart e) {
            SetBanner("", 0f);
            roundText.text = "Round 1";
            countdownText.text = "";
            RebuildScoreboard();
        }

        private void OnRoundCountdown(GameEvents.RoundCountdownStart e) {
            roundText.text = $"Round {e.RoundIndex + 1}";
            StartCountdown(e.Duration);
        }

        private void OnRoundStarted(GameEvents.RoundStart e) {
            StopCountdown();
            countdownText.text = "";
            PulseBanner("GO!", 1.2f);
        }

        private void OnPlayerJoined(int actorNumber, string playerName) {
            _names[actorNumber] = playerName;
            _scores[actorNumber] = 0;
            RebuildScoreboard();
        }

        private void OnPlayerDied(GameEvents.PlayerDied e) {
            PulseBanner($"{NameOf(e.ActorNumber)} was eliminated", 1f);
        }

        private void OnRoundEnded(GameEvents.RoundEnd e) {
            PulseBanner($"Round Winner: {NameOf(e.WinnerActorNumber)}", 1.8f);
        }

        private void OnMatchEnded(GameEvents.MatchEnd e) {
            PulseBanner($"MATCH WINNER: {NameOf(e.WinnerActorNumber)}", 2.5f);
        }

        // UI helpers
        private void RebuildScoreboard() {
            foreach (Transform c in scoreboardContent) Destroy(c.gameObject);
            _rows.Clear();
            foreach (var actor in _names.Keys.OrderByDescending(a => _scores.GetValueOrDefault(a, 0))) {
                AddOrUpdateRow(actor);
            }
            ResortAndHighlight();
        }

        private void AddOrUpdateRow(int actor) {
            if (!_rows.TryGetValue(actor, out var row)) {
                row = Instantiate(scoreboardRowPrefab, scoreboardContent);
                _rows[actor] = row;
            }
            int sc = _scores.GetValueOrDefault(actor, 0);
            row.Set(NameOf(actor), sc);
        }

        private void ResortAndHighlight() {
            var ordered = _rows.OrderByDescending(r => r.Value.Score).ToList();
            
            for (int i = 0; i < ordered.Count; i++) 
                ordered[i].Value.transform.SetSiblingIndex(i);
            
            if (ordered.Count == 0) return;
            
            var leader = ordered.First().Value;
            foreach (var r in _rows.Values) 
                r.SetLeader(r == leader);
        }

        private string NameOf(int actor) => _names.TryGetValue(actor, out var playerName) ? playerName : $"{actor}";

        // Countdown/Banner/Kill feed
        private void StartCountdown(float seconds) {
            StopCountdown(); 
            _countdownCo = StartCoroutine(CoCountdown(seconds));
        }

        private void StopCountdown() {
            if (_countdownCo != null) {
                StopCoroutine(_countdownCo); 
                _countdownCo = null;
            }
        }

        private IEnumerator CoCountdown(float seconds) {
            float t = seconds;
            _currentTween = countdownText.rectTransform.DOPunchScale(
                new Vector3(1f, 1f, 0f),
                0.3f,
                6,
                0.8f
            );
            while (t > 0f) {
                countdownText.rectTransform.localScale = Vector3.one;
                
                countdownText.text = Mathf.CeilToInt(t).ToString(); 
                yield return null; 
                t -= Time.deltaTime;
            }
            countdownText.text = "GO!";
        }

        private void PulseBanner(string msg, float visibleSeconds) {
            centerBannerText.text = msg;
            StopAllCoroutines();
            StartCoroutine(CoBanner(visibleSeconds));
        }

        private void SetBanner(string msg, float alpha) {
            centerBannerText.text = msg; 
            centerBannerGroup.alpha = alpha; 
            centerBannerGroup.gameObject.SetActive(alpha > 0f);
        }

        private IEnumerator CoBanner(float seconds) {
            centerBannerGroup.gameObject.SetActive(true);
            yield return Fade(centerBannerGroup, 0f, 1f, 0.12f);
            yield return new WaitForSecondsRealtime(seconds);
            yield return Fade(centerBannerGroup, 1f, 0f, 0.25f);
            centerBannerGroup.gameObject.SetActive(false);
        }

        private static IEnumerator Fade(CanvasGroup g, float a, float b, float t) {
            float e = 0f;
            while (e < t) {
                e += Time.unscaledDeltaTime; 
                g.alpha = Mathf.Lerp(a,b,e/t); 
                yield return null;
            } 
            g.alpha = b;
        }
    }
}
