using System;
using System.Linq;
using Plugins;
using TwitchSDK;
using TwitchSDK.Interop;
using UnityEngine;

namespace TwitchStuff {
    [Obsolete("Use TwitchRevamp instead")]
    public class TwitchPollManager : MonoBehaviour {
        GameTask<Poll> ActivePoll = null;
        private bool PollHasEnded = false;

        private void Start() { }

        //Called by some external class
        public void StartPoll(string pollTitle, string[] pollChoices) {
            // We already polled something, so we don't do anything.
            if ( ActivePoll != null )
                return;
            var authStatus = Twitch.API?.GetAuthState().MaybeResult;
            // Not logged in
            if ( authStatus?.Status != AuthStatus.LoggedIn )
                return;
            // If we don't have the "Manage Polls" permission, we also give up.
            if ( !authStatus.Scopes.Any( a => a == TwitchOAuthScope.Channel.ManagePolls.Scope ) )
                return;
            ActivePoll = Twitch.API.NewPoll(
                new PollDefinition {
                    Title = pollTitle,
                    Choices = pollChoices,
                    Duration = 20, // in seconds
                }
            );
        }

        private void Update() {
            if ( PollHasEnded )
                return;
            var poll = ActivePoll?.MaybeResult;
            if ( poll == null )
                return;
            // This means we have an active poll, and are waiting for it to finish.
            if ( poll.Info.Status == PollStatus.Active )
                return;
            // Now we know the poll is finished in some way.
            PollHasEnded = true;
            // This poll was cancelled by someone, so we don't take the results.
            if ( poll.Info.Status != PollStatus.Completed )
                return;
            // Now we can tally results and do something
            var votesForChoice1 = poll.Info.Choices.Where( a => a.Title == "Yes" ).Single();
            var votesForChoice2 = poll.Info.Choices.Where( a => a.Title == "No" ).Single();

            if ( votesForChoice1.Votes > votesForChoice2.Votes )

                Debug.Log( "✅ User1" );
            // Do something
            if ( votesForChoice2.Votes > votesForChoice1.Votes )
                // Do something else
                Debug.Log( "✅ User2" );
            else // in case they somehow tie
                Debug.Log( "✅ User3" );
            // Do something else again
        }
    }
}