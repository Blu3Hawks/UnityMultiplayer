using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace UI {
    public class ScoreboardRowUI : MonoBehaviour {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private Image crownIcon;
        public int Score { get; private set; }

        public void Set(string playerName, int score) {
            nameText.text = playerName; 
            SetScore(score);
        }

        private void SetScore(int score) {
            Score = score; 
            scoreText.text = score.ToString();
        }

        public void SetLeader(bool isLeader) {
            if (!crownIcon) return;
            
            crownIcon.enabled = isLeader;
        }
    }
}