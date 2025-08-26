using System.Linq;
using Plugins;
using TwitchSDK;
using TwitchSDK.Interop;
using UnityEngine;
using Logger = TCS.Utils.Logger;
namespace TwitchRevamp {
    public class TwitchPredictionManager : MonoBehaviour {
        GameTask<Prediction> m_active;
        bool m_ended;
        Prediction m_local;

        void Update() {
            if ( m_ended ) return;

            var p = m_active?.MaybeResult;
            if ( p == null ) return;

            m_local = p;

            if ( p.Info.Status == PredictionStatus.Active ) return;

            m_ended = true;
            if ( p.Info.Status != PredictionStatus.Locked ) return;

            var winner = p.Info.Outcomes.First( o => o.Id == p.Info.WinningOutcomeId );
            Logger.Log( $"Prediction complete. Winner: {winner.Title}" );
        }

        public void CreatePrediction(string title, string[] outcomes, int durationSeconds = 20) {
            if ( m_active != null ) return;

            var st = Twitch.API.GetAuthState()?.MaybeResult;
            if ( st?.Status != AuthStatus.LoggedIn ) return;
            if ( st.Scopes.All( s => s != TwitchOAuthScope.Channel.ManagePredictions.Scope ) ) return;

            m_active = Twitch.API.NewPrediction(
                new PredictionDefinition {
                    Title = title,
                    Outcomes = outcomes,
                    Duration = durationSeconds,
                }
            );
            m_ended = false;
        }

        // Call this to pick a winner (index into Outcomes)
        public void ResolvePrediction(int winningIndex) {
            if ( m_local == null ) return;
            var outcome = m_local.Info.Outcomes[winningIndex];
            m_local.Resolve( outcome );
            Logger.Log( "Prediction resolved." );
        }
    }
}