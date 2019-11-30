// GENERATED AUTOMATICALLY FROM 'Assets/Controls.inputactions'

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Input;


[Serializable]
public class Controls : InputActionAssetReference
{
    public Controls()
    {
    }
    public Controls(InputActionAsset asset)
        : base(asset)
    {
    }
    private bool m_Initialized;
    private void Initialize()
    {
        // gameplay
        m_gameplay = asset.GetActionMap("gameplay");
        m_gameplay_ConfirmOrder = m_gameplay.GetAction("ConfirmOrder");
        if (m_gameplayConfirmOrderActionStarted != null)
            m_gameplay_ConfirmOrder.started += m_gameplayConfirmOrderActionStarted.Invoke;
        if (m_gameplayConfirmOrderActionPerformed != null)
            m_gameplay_ConfirmOrder.performed += m_gameplayConfirmOrderActionPerformed.Invoke;
        if (m_gameplayConfirmOrderActionCancelled != null)
            m_gameplay_ConfirmOrder.cancelled += m_gameplayConfirmOrderActionCancelled.Invoke;
        m_gameplay_movement = m_gameplay.GetAction("movement");
        if (m_gameplayMovementActionStarted != null)
            m_gameplay_movement.started += m_gameplayMovementActionStarted.Invoke;
        if (m_gameplayMovementActionPerformed != null)
            m_gameplay_movement.performed += m_gameplayMovementActionPerformed.Invoke;
        if (m_gameplayMovementActionCancelled != null)
            m_gameplay_movement.cancelled += m_gameplayMovementActionCancelled.Invoke;
        m_Initialized = true;
    }
    private void Uninitialize()
    {
        if (m_GameplayActionsCallbackInterface != null)
        {
            gameplay.SetCallbacks(null);
        }
        m_gameplay = null;
        m_gameplay_ConfirmOrder = null;
        if (m_gameplayConfirmOrderActionStarted != null)
            m_gameplay_ConfirmOrder.started -= m_gameplayConfirmOrderActionStarted.Invoke;
        if (m_gameplayConfirmOrderActionPerformed != null)
            m_gameplay_ConfirmOrder.performed -= m_gameplayConfirmOrderActionPerformed.Invoke;
        if (m_gameplayConfirmOrderActionCancelled != null)
            m_gameplay_ConfirmOrder.cancelled -= m_gameplayConfirmOrderActionCancelled.Invoke;
        m_gameplay_movement = null;
        if (m_gameplayMovementActionStarted != null)
            m_gameplay_movement.started -= m_gameplayMovementActionStarted.Invoke;
        if (m_gameplayMovementActionPerformed != null)
            m_gameplay_movement.performed -= m_gameplayMovementActionPerformed.Invoke;
        if (m_gameplayMovementActionCancelled != null)
            m_gameplay_movement.cancelled -= m_gameplayMovementActionCancelled.Invoke;
        m_Initialized = false;
    }
    public void SetAsset(InputActionAsset newAsset)
    {
        if (newAsset == asset) return;
        var gameplayCallbacks = m_GameplayActionsCallbackInterface;
        if (m_Initialized) Uninitialize();
        asset = newAsset;
        gameplay.SetCallbacks(gameplayCallbacks);
    }
    public override void MakePrivateCopyOfActions()
    {
        SetAsset(ScriptableObject.Instantiate(asset));
    }
    // gameplay
    private InputActionMap m_gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private InputAction m_gameplay_ConfirmOrder;
    [SerializeField] private ActionEvent m_gameplayConfirmOrderActionStarted;
    [SerializeField] private ActionEvent m_gameplayConfirmOrderActionPerformed;
    [SerializeField] private ActionEvent m_gameplayConfirmOrderActionCancelled;
    private InputAction m_gameplay_movement;
    [SerializeField] private ActionEvent m_gameplayMovementActionStarted;
    [SerializeField] private ActionEvent m_gameplayMovementActionPerformed;
    [SerializeField] private ActionEvent m_gameplayMovementActionCancelled;
    public struct GameplayActions
    {
        private Controls m_Wrapper;
        public GameplayActions(Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @ConfirmOrder { get { return m_Wrapper.m_gameplay_ConfirmOrder; } }
        public ActionEvent ConfirmOrderStarted { get { return m_Wrapper.m_gameplayConfirmOrderActionStarted; } }
        public ActionEvent ConfirmOrderPerformed { get { return m_Wrapper.m_gameplayConfirmOrderActionPerformed; } }
        public ActionEvent ConfirmOrderCancelled { get { return m_Wrapper.m_gameplayConfirmOrderActionCancelled; } }
        public InputAction @movement { get { return m_Wrapper.m_gameplay_movement; } }
        public ActionEvent movementStarted { get { return m_Wrapper.m_gameplayMovementActionStarted; } }
        public ActionEvent movementPerformed { get { return m_Wrapper.m_gameplayMovementActionPerformed; } }
        public ActionEvent movementCancelled { get { return m_Wrapper.m_gameplayMovementActionCancelled; } }
        public InputActionMap Get() { return m_Wrapper.m_gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                ConfirmOrder.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnConfirmOrder;
                ConfirmOrder.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnConfirmOrder;
                ConfirmOrder.cancelled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnConfirmOrder;
                movement.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMovement;
                movement.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMovement;
                movement.cancelled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMovement;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                ConfirmOrder.started += instance.OnConfirmOrder;
                ConfirmOrder.performed += instance.OnConfirmOrder;
                ConfirmOrder.cancelled += instance.OnConfirmOrder;
                movement.started += instance.OnMovement;
                movement.performed += instance.OnMovement;
                movement.cancelled += instance.OnMovement;
            }
        }
    }
    public GameplayActions @gameplay
    {
        get
        {
            if (!m_Initialized) Initialize();
            return new GameplayActions(this);
        }
    }
    [Serializable]
    public class ActionEvent : UnityEvent<InputAction.CallbackContext>
    {
    }
}
public interface IGameplayActions
{
    void OnConfirmOrder(InputAction.CallbackContext context);
    void OnMovement(InputAction.CallbackContext context);
}
