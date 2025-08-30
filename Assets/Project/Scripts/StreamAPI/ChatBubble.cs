using TMPro;
using UnityEngine;

public class ChatBubble : MonoBehaviour {
    [SerializeField] TextMeshProUGUI m_playerChatText;
    [SerializeField] CanvasGroup m_canvasGroup;

    public float m_lifeTime = 3f;
    float m_timer;
    bool m_isActive;

    void Update() {
        if ( m_isActive ) {
            m_timer += Time.deltaTime;
            if ( m_timer >= m_lifeTime ) {
                m_isActive = false;
                m_canvasGroup.alpha = 0f;
            }
        }
    }

    public void ShowMessage(string message) {
        m_playerChatText.text = message;
        m_canvasGroup.alpha = 1f;
        m_timer = 0f;
        m_isActive = true;
    }
}