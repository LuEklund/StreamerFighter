#if UNITY_EDITOR

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
namespace TwitchRevamp.API {
    [InitializeOnLoad]
    [CustomEditor(typeof(TwitchAPISettings))]
    public class TwitchAPISettingsEditor : Editor
    {
        void SetDirtyIfNeeded<T>(ref T field, T value)
        {
            if (!System.Object.Equals(field, value))
            {
                field = value;
                EditorUtility.SetDirty(target);
            }
        }


        public override void OnInspectorGUI()
        {
            var inst = TwitchAPISettings.Instance;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("TwitchAPI Client ID:");
            SetDirtyIfNeeded(ref inst.m_clientId, EditorGUILayout.TextField(inst.m_clientId));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Status: " + this.m_credentialStatus);
            if (GUILayout.Button("Go to dev.twitch.tv", EditorStyles.linkLabel))
            {
                System.Diagnostics.Process.Start("https://dev.twitch.tv");
            }

            UpdateCredentialStatus(inst.m_clientId);

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Use EventSubProxy:", "Do not enable this in releases. This option instructs the plugin to connect to a local EventSubProxy instead of directly to TwitchAPI."));
            SetDirtyIfNeeded(ref inst.m_useEventSubProxy, EditorGUILayout.Toggle(inst.m_useEventSubProxy));
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Installed Plugin Core Library:");
            EditorGUILayout.LabelField(TwitchSDK.TwitchSDKApi.Version);
            EditorGUILayout.EndHorizontal();
        }

        [MenuItem("TwitchAPI/Edit Settings")]
        public static void Edit()
        {
            if (TwitchAPISettings.NullableInstance == null)
            {
                var instance = ScriptableObject.CreateInstance<TwitchAPISettings>();
                string path = Path.Combine(Application.dataPath, "Plugins", "Resources");
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }

                string str = Path.Combine(Path.Combine("Assets", "Plugins", "Resources"), $"{nameof(TwitchAPISettings)}.asset");
                AssetDatabase.CreateAsset(instance, str);
            }
            Selection.activeObject = TwitchAPISettings.Instance;
        }


        HttpClient m_http = new();
        string m_credentialStatus = "";
        CancellationTokenSource m_currentCts = null;
        string m_lastCheckedClientId = "";

        public TwitchAPISettingsEditor()
        {
            m_http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("TwitchAPI-Route-66", "0.1"));
            m_http.Timeout = TimeSpan.FromSeconds(5);
        }

        public async void UpdateCredentialStatus(string clientId)
        {
            try
            {
                if (clientId == m_lastCheckedClientId)
                {
                    return;
                }

                m_lastCheckedClientId = clientId;

                m_currentCts?.Cancel();
                m_currentCts = new CancellationTokenSource();
                this.m_credentialStatus = "Checking ClientId ...";
                try
                {
                    this.m_credentialStatus = await GetCredentialStatus(clientId);
                }
                catch (TaskCanceledException)
                {
                }
                this.Repaint();
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error updating credential status.");
                Debug.LogException(e);
            }
        }

        public async Task<string> GetCredentialStatus(string clientId)
        {
            if (clientId.Length == 0 || clientId == TwitchAPISettings.INITIAL_CLIENT_ID) {
                return "Please enter a valid ClientId!";
            }

            clientId = Uri.EscapeDataString(clientId);

            try
            {
                var res = await m_http.PostAsync(
                    $"https://id.twitch.tv/oauth2/device?client_id={clientId}",
                    new StringContent(""),
                    m_currentCts.Token);
                var text = await res.Content.ReadAsStringAsync();

                if (res.IsSuccessStatusCode) {
                    return "ClientId is valid!";
                }

                if (res.StatusCode == System.Net.HttpStatusCode.BadRequest) {
                    return "Please enter a valid ClientId!";
                }
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            return "Unable to check if the ClientId is valid.";
        }
    }
}

#endif