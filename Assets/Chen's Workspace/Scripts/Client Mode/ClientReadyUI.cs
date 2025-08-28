using Fusion;
using TMPro;
using UnityEngine;

public class ClientReadyUI : MonoBehaviour
{
    [Header("Labels (optional)")]
    [SerializeField] private TextMeshProUGUI readyLabel;
    [SerializeField] private TextMeshProUGUI tallyLabel;

    private RoomState _state;
    private bool _isReady;

    private void OnEnable()
    {
        RoomState.CurrentChanged += OnRoomStateChanged;

        if (RoomState.Current != null)
            OnRoomStateChanged(RoomState.Current);

        RefreshAll();
    }

    private void OnDisable()
    {
        RoomState.CurrentChanged -= OnRoomStateChanged;
    }

    private void Update()
    {
        if (_state && tallyLabel)
            tallyLabel.SetText($"{_state.ReadyCount} / {_state.TotalClients}");
    }

    private void OnRoomStateChanged(RoomState state)
    {
        _state = state;
        _isReady = false;
        RefreshAll();
    }

    public void ToggleReady()
    {
        if (!_state) return;

        _isReady = !_isReady;
        _state.RpcSetReady(_isReady);
        RefreshAll();
    }

    private void RefreshAll()
    {
        if (readyLabel) readyLabel.SetText(_isReady ? "Ready" : "Not Ready");
        if (_state && tallyLabel)
            tallyLabel.SetText($"{_state.ReadyCount} / {_state.TotalClients}");
    }
}
