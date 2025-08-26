using System.Linq;
using TwitchRevamp.API;
using TwitchSDK;
using TwitchSDK.Interop;
using UnityEngine;
using Logger = TCS.Utils.Logger;
namespace TwitchRevamp {
    public class TwitchPollManager : MonoBehaviour {
        GameTask<Poll> m_activePoll;
        bool m_ended;

        void Update() {
            if ( m_ended ) {
                return;
            }

            var poll = m_activePoll?.MaybeResult;
            if ( poll == null ) {
                return;
            }

            if ( poll.Info.Status == PollStatus.Active ) {
                return;
            }

            m_ended = true;
            if ( poll.Info.Status != PollStatus.Completed ) {
                return;
            }

            var winner = poll.Info.Choices.OrderByDescending( c => c.Votes ).First();
            Logger.Log( $"Poll finished. Winner: {winner.Title} ({winner.Votes})" );
        }

        public void StartPoll(string title, string[] choices, int durationSeconds = 20) {
            if ( m_activePoll != null ) {
                return;
            }

            var st = TwitchAPI.API.GetAuthState()?.MaybeResult;
            if ( st?.Status != AuthStatus.LoggedIn ) {
                return;
            }

            if ( st.Scopes.All( s => s != TwitchOAuthScope.Channel.ManagePolls.Scope ) ) {
                return;
            }

            m_activePoll = TwitchAPI.API.NewPoll(
                new PollDefinition {
                    Title = title,
                    Choices = choices,
                    Duration = durationSeconds,
                }
            );
            m_ended = false;
        }
    }
}