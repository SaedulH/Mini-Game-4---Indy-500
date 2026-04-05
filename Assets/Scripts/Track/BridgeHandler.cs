using UnityEngine;
using Utilities;

public class BridgeHandler : MonoBehaviour
{
    [field: Header("Triggers")]
    [field: SerializeField] public BridgeTrigger UnderPassInTrigger { get; set; }
    [field: SerializeField] public BridgeTrigger UnderPassOutTrigger { get; set; }
    [field: SerializeField] public BridgeTrigger OverPassInTrigger { get; set; }
    [field: SerializeField] public BridgeTrigger OverPassOutTrigger { get; set; }

    [field: Header("Colliders")]
    [field: SerializeField] public BoxCollider2D UnderPassInCollider { get; set; }
    [field: SerializeField] public BoxCollider2D UnderPassOutCollider { get; set; }
    [field: SerializeField] public BoxCollider2D OverPassInCollider { get; set; }
    [field: SerializeField] public BoxCollider2D OverPassOutCollider { get; set; }

    private CapsuleCollider2D _playerOneCollider;
    private CapsuleCollider2D _playerTwoCollider;
    private bool _isPlayerOneUnderPass = false;
    private bool _isPlayerOneOverPass = false;
    private bool _isPlayerTwoUnderPass = false;
    private bool _isPlayerTwoOverPass = false;

    private void OnEnable()
    {
        SubscribeTriggerEvent(UnderPassInTrigger);
        SubscribeTriggerEvent(UnderPassOutTrigger);
        SubscribeTriggerEvent(OverPassInTrigger);
        SubscribeTriggerEvent(OverPassOutTrigger);
    }

    private void OnDisable()
    {
        UnsubscribeTriggerEvent(UnderPassInTrigger);
        UnsubscribeTriggerEvent(UnderPassOutTrigger);
        UnsubscribeTriggerEvent(OverPassInTrigger);
        UnsubscribeTriggerEvent(OverPassOutTrigger);
    }

    private void SubscribeTriggerEvent(BridgeTrigger bridgeTrigger)
    {
        if (bridgeTrigger != null)
        {
            bridgeTrigger.TriggerEvent += OnBridgeTriggerEvent;
        }
    }

    private void UnsubscribeTriggerEvent(BridgeTrigger bridgeTrigger)
    {
        if (bridgeTrigger != null)
        {
            bridgeTrigger.TriggerEvent -= OnBridgeTriggerEvent;
        }
    }

    private void OnBridgeTriggerEvent(GameObject player, BridgeTriggerType type, BridgeTriggerDirection direction)
    {
        Debug.Log($"Bridge Trigger Event {type}:{direction} for player {player.name}");

        bool isPlayerOne = player.name == "PlayerOne";
        bool isUnderPass = type == BridgeTriggerType.Underpass;

        if (player.TryGetComponent(out CapsuleCollider2D collider))
        {
            if (isPlayerOne)
            {
                _isPlayerOneUnderPass = isUnderPass;
                _isPlayerOneOverPass = !isUnderPass;
                if (_playerOneCollider == null)
                {
                    _playerOneCollider = collider;
                }
            }
            else
            {
                _isPlayerTwoUnderPass = isUnderPass;
                _isPlayerTwoOverPass = !isUnderPass;
                if (_playerTwoCollider == null)
                {
                    _playerTwoCollider = collider;
                }
            }
            HandleVehicleCollision();
            HandleBridgeCollision(isUnderPass, collider);
        }

        if (player.TryGetComponent(out LayerHandler layerHandler))
        {
            layerHandler.SetBridgeSortingLayer(isUnderPass);
        }
    }

    private void HandleBridgeCollision(bool isUnderPass, CapsuleCollider2D collider)
    {
        Physics2D.IgnoreCollision(collider, UnderPassInCollider, isUnderPass);
        Physics2D.IgnoreCollision(collider, UnderPassOutCollider, isUnderPass);
        Physics2D.IgnoreCollision(collider, OverPassInCollider, !isUnderPass);
        Physics2D.IgnoreCollision(collider, OverPassOutCollider, !isUnderPass);
    }

    private void HandleVehicleCollision()
    {
        if (_playerOneCollider != null && _playerTwoCollider != null)
        {
            if ((_isPlayerOneOverPass && _isPlayerTwoUnderPass) || (_isPlayerOneUnderPass && _isPlayerTwoOverPass))
            {
                Physics2D.IgnoreCollision(_playerOneCollider, _playerTwoCollider, true);
            }
            else
            {
                Physics2D.IgnoreCollision(_playerOneCollider, _playerTwoCollider, false);
            }
        }
    }
}
