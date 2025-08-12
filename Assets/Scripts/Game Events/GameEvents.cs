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

        // Events
        public static event UnityAction<MatchStart> OnMatchStarted;
        public static event UnityAction<RoundCountdownStart> OnRoundCountdownStarted;
        public static event UnityAction<RoundStart> OnRoundStarted;
        public static event UnityAction<PlayerDied> OnPlayerDied;
        public static event UnityAction<RoundEnd> OnRoundEnded;
        public static event UnityAction<MatchEnd> OnMatchEnded;

        // Raisers
        public static void Raise(MatchStart e) => OnMatchStarted?.Invoke(e);
        public static void Raise(RoundCountdownStart e) => OnRoundCountdownStarted?.Invoke(e);
        public static void Raise(RoundStart e) => OnRoundStarted?.Invoke(e);
        public static void Raise(PlayerDied e) => OnPlayerDied?.Invoke(e);
        public static void Raise(RoundEnd e) => OnRoundEnded?.Invoke(e);
        public static void Raise(MatchEnd e) => OnMatchEnded?.Invoke(e);
    }
}
