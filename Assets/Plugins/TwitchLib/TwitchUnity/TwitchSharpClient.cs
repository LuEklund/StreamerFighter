using System;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Exceptions;
using TwitchLib.Client.Interfaces;
using TwitchLib.Communication.Events;
namespace TwitchLib.Unity {
    public class TwitchSharpClient : TwitchClient, ITwitchClient {
        new void HandleNotInitialized() {
            ThreadDispatcher.Enqueue( () => throw new ClientNotInitializedException( "The twitch client has not been initialized and cannot be used. Please call Initialize();" ) );
        }

        #region Events
        /// <summary>
        ///     Fires when client connects to Twitch.
        /// </summary>
        public new event EventHandler<OnConnectedArgs> OnConnected;

        /// <summary>
        ///     Fires when client reconnects to Twitch.
        /// </summary>
        public new event EventHandler<OnReconnectedEventArgs> OnReconnected;

        /// <summary>
        ///     Fires when client joins a channel.
        /// </summary>
        public new event EventHandler<OnJoinedChannelArgs> OnJoinedChannel;

        /// <summary>
        ///     Fires on logging in with incorrect details, returns ErrorLoggingInException.
        /// </summary>
        public new event EventHandler<OnIncorrectLoginArgs> OnIncorrectLogin;

        /// <summary>
        ///     Fires when connecting and channel state is changed, returns ChannelState.
        /// </summary>
        public new event EventHandler<OnChannelStateChangedArgs> OnChannelStateChanged;

        /// <summary>
        ///     Fires when a user state is received, returns UserState.
        /// </summary>
        public new event EventHandler<OnUserStateChangedArgs> OnUserStateChanged;

        /// <summary>
        ///     Fires when a new chat message arrives, returns ChatMessage.
        /// </summary>
        public new event EventHandler<OnMessageReceivedArgs> OnMessageReceived;

        /// <summary>
        ///     Fires when a new whisper arrives, returns WhisperMessage.
        /// </summary>
        public new event EventHandler<OnWhisperReceivedArgs> OnWhisperReceived;

        /// <summary>
        ///     Fires when a chat message is sent, returns username, channel and message.
        /// </summary>
        public new event EventHandler<OnMessageSentArgs> OnMessageSent;

        /// <summary>
        ///     Fires when command (uses custom chat command identifier) is received, returns channel, command, ChatMessage,
        ///     arguments as string, arguments as list.
        /// </summary>
        public new event EventHandler<OnChatCommandReceivedArgs> OnChatCommandReceived;

        /// <summary>
        ///     Fires when command (uses custom whisper command identifier) is received, returns command, Whispermessage.
        /// </summary>
        public new event EventHandler<OnWhisperCommandReceivedArgs> OnWhisperCommandReceived;

        /// <summary>
        ///     Fires when a new viewer/chatter joined the channel's chat room, returns username and channel.
        /// </summary>
        public new event EventHandler<OnUserJoinedArgs> OnUserJoined;

        /// <summary>
        ///     Fires when new subscriber is announced in chat, returns Subscriber.
        /// </summary>
        public new event EventHandler<OnNewSubscriberArgs> OnNewSubscriber;

        /// <summary>
        ///     Fires when current subscriber renews subscription, returns ReSubscriber.
        /// </summary>
        public new event EventHandler<OnReSubscriberArgs> OnReSubscriber;

        /// <summary>
        ///     Fires when a current Prime gaming subscriber converts to a paid subscription.
        /// </summary>
        public new event EventHandler<OnPrimePaidSubscriberArgs> OnPrimePaidSubscriber;

        /// <summary>
        ///     Fires when a current gifted subscriber converts to a paid subscription.
        /// </summary>
        public new event EventHandler<OnContinuedGiftedSubscriptionArgs> OnContinuedGiftedSubscription;

        /// <summary>
        ///     Fires when Twitch notifies client of existing users in chat.
        /// </summary>
        public new event EventHandler<OnExistingUsersDetectedArgs> OnExistingUsersDetected;

        /// <summary>
        ///     Fires when a PART message is received from Twitch regarding a particular viewer
        /// </summary>
        public new event EventHandler<OnUserLeftArgs> OnUserLeft;

        /// <summary>
        ///     Fires when bot has disconnected.
        /// </summary>
        public new event EventHandler<OnDisconnectedEventArgs> OnDisconnected;

        /// <summary>
        ///     Forces when bot suffers conneciton error.
        /// </summary>
        public new event EventHandler<OnConnectionErrorArgs> OnConnectionError;

        /// <summary>
        ///     Fires when a channel's chat is cleared.
        /// </summary>
        public new event EventHandler<OnChatClearedArgs> OnChatCleared;

        /// <summary>
        ///     Fires when a viewer gets timedout by any moderator.
        /// </summary>
        public new event EventHandler<OnUserTimedoutArgs> OnUserTimedout;

        /// <summary>
        ///     Fires when client successfully leaves a channel.
        /// </summary>
        public new event EventHandler<OnLeftChannelArgs> OnLeftChannel;

        /// <summary>
        ///     Fires when a viewer gets banned by any moderator.
        /// </summary>
        public new event EventHandler<OnUserBannedArgs> OnUserBanned;

        /// <summary>
        ///     Fires when a list of moderators is received.
        /// </summary>
        public new event EventHandler<OnModeratorsReceivedArgs> OnModeratorsReceived;

        /// <summary>
        ///     Fires when confirmation of a chat color change request was received.
        /// </summary>
        public new event EventHandler<OnChatColorChangedArgs> OnChatColorChanged;

        /// <summary>
        ///     Fires when data is either received or sent.
        /// </summary>
        public new event EventHandler<OnSendReceiveDataArgs> OnSendReceiveData;

        /// <summary>
        ///     Fires when a raid notification is detected in chat
        /// </summary>
        public new event EventHandler<OnRaidNotificationArgs> OnRaidNotification;

        /// <summary>
        ///     Fires when a subscription is gifted and announced in chat
        /// </summary>
        public new event EventHandler<OnGiftedSubscriptionArgs> OnGiftedSubscription;

        // /// <summary>Fires when TwitchClient attempts to host a channel it is in.</summary>
        // public new event EventHandler<NoticeEventArgs> OnSelfRaidError;
        //
        // /// <summary>Fires when TwitchClient receives generic no permission error from Twitch.</summary>
        // public new event EventHandler<NoticeEventArgs> OnNoPermissionError;
        //
        // /// <summary>Fires when newly raided channel is mature audience only.</summary>
        // public new event EventHandler<NoticeEventArgs> OnRaidedChannelIsMatureAudience;

        /// <summary>Fires when the client was unable to join a channel.</summary>
        public new event EventHandler<OnFailureToReceiveJoinConfirmationArgs> OnFailureToReceiveJoinConfirmation;

        /// <summary>Fires when data is received from Twitch that is not able to be parsed.</summary>
        public new event EventHandler<OnUnaccountedForArgs> OnUnaccountedFor;

        /// <summary>Fires when named event occurs.</summary>
        public new event EventHandler<OnMessageClearedArgs> OnMessageCleared;
        /// <summary>Fires when named event occurs.</summary>
        public new event EventHandler<OnCommunitySubscriptionArgs> OnCommunitySubscription;
        /// <summary>Fires when named event occurs.</summary>
        public new event EventHandler<OnErrorEventArgs> OnError;
        /// <summary>Fires when named event occurs.</summary>
        public new event EventHandler<OnVIPsReceivedArgs> OnVIPsReceived;
        /// <summary>Fires when named event occurs.</summary>
        public new event EventHandler<OnAnnouncementArgs> OnAnnouncement;
        /// <summary>Fires when named event occurs.</summary>
        /*public new event EventHandler<OnMessageThrottledArgs> OnMessageThrottled;
        /// <summary>Fires when named event occurs.</summary>
        public new event EventHandler<NoticeEventArgs> OnRequiresVerifiedPhoneNumber;
        /// <summary>Fires when named event occurs.</summary>
        public new event EventHandler<NoticeEventArgs> OnFollowersOnly;
        /// <summary>Fires when named event occurs.</summary>
        public new event EventHandler<NoticeEventArgs> OnSubsOnly;
        /// <summary>Fires when named event occurs.</summary>
        public new event EventHandler<NoticeEventArgs> OnEmoteOnly;
        /// <summary>Fires when named event occurs.</summary>
        public new event EventHandler<NoticeEventArgs> OnSuspended;
        /// <summary>Fires when named event occurs.</summary>
        public new event EventHandler<NoticeEventArgs> OnBanned;
        /// <summary>Fires when named event occurs.</summary>
        public new event EventHandler<NoticeEventArgs> OnSlowMode;
        /// <summary>Fires when named event occurs.</summary>
        public new event EventHandler<NoticeEventArgs> OnR9kMode;
        /// <summary>Fires when named event occurs.</summary>
        public new event EventHandler<OnUserIntroArgs> OnUserIntro;
        /// <summary>Fires when named event occurs.</summary>
        public new event EventHandler<OnAnonGiftPaidUpgradeArgs> OnAnonGiftPaidUpgrade;
        /// <summary>Fires when named event occurs.</summary>
        public new event EventHandler<OnUnraidNotificationArgs> OnUnraidNotification;
        /// <summary>Fires when named event occurs.</summary>
        public new event EventHandler<OnRitualArgs> OnRitual;
        /// <summary>Fires when named event occurs.</summary>
        public new event EventHandler<OnBitsBadgeTierArgs> OnBitsBadgeTier;
        /// <summary>Fires when named event occurs.</summary>
        public new event EventHandler<OnCommunityPayForwardArgs> OnCommunityPayForward;
        /// <summary>Fires when named event occurs.</summary>
        public new event EventHandler<OnStandardPayForwardArgs> OnStandardPayForward;*/
        #endregion

        public TwitchSharpClient() : base() {
            ThreadDispatcher.EnsureCreated();

            base.OnConnected += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnConnected?.Invoke( sender, e ) );
            };
            base.OnReconnected += (sender, e) => {

                ThreadDispatcher.Enqueue( () => OnReconnected?.Invoke( sender, e ) );
            };
            base.OnJoinedChannel += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnJoinedChannel?.Invoke( sender, e ) );
            };
            base.OnIncorrectLogin += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnIncorrectLogin?.Invoke( sender, e ) );
            };
            base.OnChannelStateChanged += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnChannelStateChanged?.Invoke( sender, e ) );
            };
            base.OnUserStateChanged += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnUserStateChanged?.Invoke( sender, e ) );
            };
            base.OnMessageReceived += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnMessageReceived?.Invoke( sender, e ) );
            };
            base.OnWhisperReceived += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnWhisperReceived?.Invoke( sender, e ) );
            };
            base.OnMessageSent += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnMessageSent?.Invoke( sender, e ) );
            };
            base.OnChatCommandReceived += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnChatCommandReceived?.Invoke( sender, e ) );
            };
            base.OnWhisperCommandReceived += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnWhisperCommandReceived?.Invoke( sender, e ) );
            };
            base.OnUserJoined += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnUserJoined?.Invoke( sender, e ) );
            };
            base.OnNewSubscriber += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnNewSubscriber?.Invoke( sender, e ) );
            };
            base.OnReSubscriber += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnReSubscriber?.Invoke( sender, e ) );
            };
            base.OnPrimePaidSubscriber += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnPrimePaidSubscriber?.Invoke( sender, e ) );
            };
            base.OnContinuedGiftedSubscription += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnContinuedGiftedSubscription?.Invoke( sender, e ) );
            };
            base.OnExistingUsersDetected += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnExistingUsersDetected?.Invoke( sender, e ) );
            };
            base.OnUserLeft += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnUserLeft?.Invoke( sender, e ) );
            };
            base.OnDisconnected += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnDisconnected?.Invoke( sender, e ) );
            };
            base.OnConnectionError += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnConnectionError?.Invoke( sender, e ) );
            };
            base.OnChatCleared += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnChatCleared?.Invoke( sender, e ) );
            };
            base.OnUserTimedout += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnUserTimedout?.Invoke( sender, e ) );
            };
            base.OnLeftChannel += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnLeftChannel?.Invoke( sender, e ) );
            };
            base.OnUserBanned += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnUserBanned?.Invoke( sender, e ) );
            };
            base.OnModeratorsReceived += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnModeratorsReceived?.Invoke( sender, e ) );
            };
            base.OnChatColorChanged += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnChatColorChanged?.Invoke( sender, e ) );
            };
            base.OnSendReceiveData += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnSendReceiveData?.Invoke( sender, e ) );
            };
            base.OnRaidNotification += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnRaidNotification?.Invoke( sender, e ) );
            };
            base.OnGiftedSubscription += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnGiftedSubscription?.Invoke( sender, e ) );
            };
            // OnRaidedChannelIsMatureAudience += (object sender, NoticeEventArgs arg) => {
            //     ThreadDispatcher.Enqueue( () => OnRaidedChannelIsMatureAudience?.Invoke( sender, arg ) );
            // };
            base.OnFailureToReceiveJoinConfirmation += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnFailureToReceiveJoinConfirmation?.Invoke( sender, e ) );
            };
            base.OnUnaccountedFor += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnUnaccountedFor?.Invoke( sender, e ) );
            };
            // OnSelfRaidError += (object sender, NoticeEventArgs e) => {
            //     ThreadDispatcher.Enqueue( () => OnSelfRaidError?.Invoke( sender, e ) );
            // };
            // OnNoPermissionError += (object sender, NoticeEventArgs e) => {
            //     ThreadDispatcher.Enqueue( () => OnNoPermissionError?.Invoke( sender, e ) );
            // };
            base.OnMessageCleared += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnMessageCleared?.Invoke( sender, e ) );
            };
            base.OnCommunitySubscription += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnCommunitySubscription?.Invoke( sender, e ) );
            };
            base.OnError += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnError?.Invoke( sender, e ) );
            };
            base.OnVIPsReceived += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnVIPsReceived?.Invoke( sender, e ) );
            };
            base.OnAnnouncement += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnAnnouncement?.Invoke( sender, e ) );
            };


            /*OnMessageThrottled += (object sender, OnMessageThrottledArgs e) => {
                ThreadDispatcher.Enqueue( () => OnMessageThrottled?.Invoke( sender, e ) );
            };
            OnRequiresVerifiedPhoneNumber += (object sender, NoticeEventArgs e) => {
                ThreadDispatcher.Enqueue( () => OnRequiresVerifiedPhoneNumber?.Invoke( sender, e ) );
            };
            OnFollowersOnly += (object sender, NoticeEventArgs e) => {
                ThreadDispatcher.Enqueue( () => OnFollowersOnly?.Invoke( sender, e ) );
            };
            OnSubsOnly += (object sender, NoticeEventArgs e) => {
                ThreadDispatcher.Enqueue( () => OnSubsOnly?.Invoke( sender, e ) );
            };
            OnEmoteOnly += (object sender, NoticeEventArgs e) => {
                ThreadDispatcher.Enqueue( () => OnEmoteOnly?.Invoke( sender, e ) );
            };
            OnSuspended += (object sender, NoticeEventArgs e) => {
                ThreadDispatcher.Enqueue( () => OnSuspended?.Invoke( sender, e ) );
            };
            OnBanned += (object sender, NoticeEventArgs e) => {
                ThreadDispatcher.Enqueue( () => OnBanned?.Invoke( sender, e ) );
            };
            OnSlowMode += (object sender, NoticeEventArgs e) => {
                ThreadDispatcher.Enqueue( () => OnSlowMode?.Invoke( sender, e ) );
            };
            OnR9kMode += (object sender, NoticeEventArgs e) => {
                ThreadDispatcher.Enqueue( () => OnR9kMode?.Invoke( sender, e ) );
            };
            OnUserIntro += (sender, e) => {
                ThreadDispatcher.Enqueue( () => OnUserIntro?.Invoke( sender, e ) );
            };*/


            /*base.OnAnonGiftPaidUpgrade += (object sender, OnAnonGiftPaidUpgradeArgs e) => {
                ThreadDispatcher.Enqueue( () => OnAnonGiftPaidUpgrade?.Invoke( sender, e ) );
                return Task.CompletedTask;
            };
            base.OnUnraidNotification += (object sender, OnUnraidNotificationArgs e) => {
                ThreadDispatcher.Enqueue( () => OnUnraidNotification?.Invoke( sender, e ) );
                return Task.CompletedTask;
            };
            base.OnRitual += (object sender, OnRitualArgs e) => {
                ThreadDispatcher.Enqueue( () => OnRitual?.Invoke( sender, e ) );
                return Task.CompletedTask;
            };
            base.OnBitsBadgeTier += (object sender, OnBitsBadgeTierArgs e) => {
                ThreadDispatcher.Enqueue( () => OnBitsBadgeTier?.Invoke( sender, e ) );
                return Task.CompletedTask;
            };
            base.OnCommunityPayForward += (object sender, OnCommunityPayForwardArgs e) => {
                ThreadDispatcher.Enqueue( () => OnCommunityPayForward?.Invoke( sender, e ) );
                return Task.CompletedTask;
            };
            base.OnStandardPayForward += (object sender, OnStandardPayForwardArgs e) => {
                ThreadDispatcher.Enqueue( () => OnStandardPayForward?.Invoke( sender, e ) );
                return Task.CompletedTask;
            };*/
        }
    }
}