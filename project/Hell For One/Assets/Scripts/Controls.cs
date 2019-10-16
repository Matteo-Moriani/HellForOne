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
        // Gameplay
        m_Gameplay = asset.GetActionMap("Gameplay");
        m_Gameplay_ConfirmOrder = m_Gameplay.GetAction("ConfirmOrder");
        if (m_GameplayConfirmOrderActionStarted != null)
            m_Gameplay_ConfirmOrder.started += m_GameplayConfirmOrderActionStarted.Invoke;
        if (m_GameplayConfirmOrderActionPerformed != null)
            m_Gameplay_ConfirmOrder.performed += m_GameplayConfirmOrderActionPerformed.Invoke;
        if (m_GameplayConfirmOrderActionCancelled != null)
            m_Gameplay_ConfirmOrder.cancelled += m_GameplayConfirmOrderActionCancelled.Invoke;
        m_Initialized = true;
    }
    private void Uninitialize()
    {
        if (m_GameplayActionsCallbackInterface != null)
        {
            Gameplay.SetCallbacks(null);
        }
        m_Gameplay = null;
        m_Gameplay_ConfirmOrder = null;
        if (m_GameplayConfirmOrderActionStarted != null)
            m_Gameplay_ConfirmOrder.started -= m_GameplayConfirmOrderActionStarted.Invoke;
        if (m_GameplayConfirmOrderActionPerformed != null)
            m_Gameplay_ConfirmOrder.performed -= m_GameplayConfirmOrderActionPerformed.Invoke;
        if (m_GameplayConfirmOrderActionCancelled != null)
            m_Gameplay_ConfirmOrder.cancelled -= m_GameplayConfirmOrderActionCancelled.Invoke;
        m_Initialized = false;
    }
    public void SetAsset(InputActionAsset newAsset)
    {
        if (newAsset == asset) return;
        var GameplayCallbacks = m_GameplayActionsCallbackInterface;
        if (m_Initialized) Uninitialize();
        asset = newAsset;
        Gameplay.SetCallbacks(GameplayCallbacks);
    }
    public override void MakePrivateCopyOfActions()
    {
        SetAsset(ScriptableObject.Instantiate(asset));
    }
    // Gameplay
    private InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private InputAction m_Gameplay_ConfirmOrder;
    [SerializeField] private ActionEvent m_GameplayConfirmOrderActionStarted;
    [SerializeField] private ActionEvent m_GameplayConfirmOrderActionPerformed;
    [SerializeField] private ActionEvent m_GameplayConfirmOrderActionCancelled;
    public struct GameplayActions
    {
        private Controls m_Wrapper;
        public GameplayActions(Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @ConfirmOrder { get { return m_Wrapper.m_Gameplay_ConfirmOrder; } }
        public ActionEvent ConfirmOrderStarted { get { return m_Wrapper.m_GameplayConfirmOrderActionStarted; } }
        public ActionEvent ConfirmOrderPerformed { get { return m_Wrapper.m_GameplayConfirmOrderActionPerformed; } }
        public ActionEvent ConfirmOrderCancelled { get { return m_Wrapper.m_GameplayConfirmOrderActionCancelled; } }
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
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
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                ConfirmOrder.started += instance.OnConfirmOrder;
                ConfirmOrder.performed += instance.OnConfirmOrder;
                ConfirmOrder.cancelled += instance.OnConfirmOrder;
            }
        }
    }
    public GameplayActions @Gameplay
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
}
