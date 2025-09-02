using UnityEngine.Events;

namespace Game_Events {
    public static class GameEvents {
        // Helpers
        public readonly struct MatchStart {
            public readonly int FirstTo;

            public MatchStart(int firstTo) {
                FirstTo = firstTo;
            }
        }

        public readonly struct RoundCountdownStart {
            public readonly int RoundIndex; 
            public readonly float Duration;

            public RoundCountdownStart(int roundIndex, float duration) {
                RoundIndex = roundIndex; 
                Duration = duration;
            }
        }

        public readonly struct RoundStart {
            public readonly int RoundIndex;

            public RoundStart(int roundIndex) {
                RoundIndex = roundIndex;
            }
        }

        public readonly struct PlayerDied {
            public readonly int ActorNumber; 

            public PlayerDied(int actorNumber) {
                ActorNumber = actorNumber; 
            }
        }

        public readonly struct RoundEnd {
            public readonly int RoundIndex; 
            public readonly int WinnerActorNumber;

            public RoundEnd(int roundIndex, int winnerActorNumber) {
                RoundIndex = roundIndex; 
                WinnerActorNumber = winnerActorNumber;
            }
        }

        public readonly struct MatchEnd {
            public readonly int WinnerActorNumber;

            public MatchEnd(int winnerActorNumber) {
                WinnerActorNumber = winnerActorNumber;
            }
        }
        
        public readonly struct PlayerInfo {
            public readonly int ActorNumber; 
            public readonly string Name;

            public PlayerInfo(int actorNumber, string name) {
                ActorNumber = actorNumber; 
                Name = name;
            }
        }

        // Snapshot of everyone currently in the match (call on round start or when lobby locks)
        public readonly struct PlayersSynced {
            public readonly PlayerInfo[] Players;

            public PlayersSynced(PlayerInfo[] players) {
                Players = players;
            }
        }

        // Set/overwrite a player's score (use for initial sync and any change)
        public readonly struct ScoreSet {
            public readonly int ActorNumber; 
            public readonly int Score;

            public ScoreSet(int actorNumber, int score) {
                ActorNumber = actorNumber; 
                Score = score;
            }
        }

        // Events
        public static event UnityAction<MatchStart> OnMatchStarted;
        public static event UnityAction<RoundCountdownStart> OnRoundCountdownStarted;
        public static event UnityAction<RoundStart> OnRoundStarted;
        public static event UnityAction<PlayerDied> OnPlayerDied;
        public static event UnityAction<int, string> OnPlayerJoined;
        public static event UnityAction<RoundEnd> OnRoundEnded;
        public static event UnityAction<MatchEnd> OnMatchEnded;
        public static event UnityAction<PlayersSynced> OnPlayersSynced;
        public static event UnityAction<ScoreSet> OnScoreSet;

        // Raisers
        public static void Raise(MatchStart e) => OnMatchStarted?.Invoke(e);
        public static void Raise(RoundCountdownStart e) => OnRoundCountdownStarted?.Invoke(e);
        public static void Raise(RoundStart e) => OnRoundStarted?.Invoke(e);
        public static void Raise(PlayerDied e) => OnPlayerDied?.Invoke(e);

        public static void Raise(int actorNumber, string playerName) {
            OnPlayerJoined?.Invoke(actorNumber, playerName);
        }
        public static void Raise(RoundEnd e) => OnRoundEnded?.Invoke(e);
        public static void Raise(MatchEnd e) => OnMatchEnded?.Invoke(e);
        public static void Raise(PlayersSynced e) => OnPlayersSynced?.Invoke(e);
        public static void Raise(ScoreSet e)      => OnScoreSet?.Invoke(e);
        
    }
}
