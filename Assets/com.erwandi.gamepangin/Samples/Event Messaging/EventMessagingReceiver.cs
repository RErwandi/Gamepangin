using Erwandi.Gamepangin.Patterns;
using UnityEngine;
using UnityEngine.UI;

public class EventMessagingReceiver : MonoBehaviour
{
    public Text receiverText;

    private void OnEnable()
    {
        EventMessaging<GameEvent>.AddListener(OnBroadcastReceived);
    }
    
    private void OnDisable()
    {
        EventMessaging<GameEvent>.RemoveListener(OnBroadcastReceived);
    }
    
    private void OnBroadcastReceived(GameEvent gameEvent)
    {
        receiverText.text = gameEvent.EventName + " Received";
    }
}
