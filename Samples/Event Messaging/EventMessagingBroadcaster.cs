using Erwandi.Gamepangin.Patterns;
using UnityEngine;
using UnityEngine.UI;

public class EventMessagingBroadcaster : MonoBehaviour
{
    public Button broadcastButton;

    private void OnEnable()
    {
        broadcastButton.onClick.AddListener(BroadcastEvent);
    }

    private void OnDisable()
    {
        broadcastButton.onClick.RemoveListener(BroadcastEvent);
    }

    private void BroadcastEvent()
    {
        var testEvent = new GameEvent {EventName = "Broadcast"};
        EventMessaging<GameEvent>.Trigger(testEvent);
    }
}
