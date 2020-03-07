// GENERATED AUTOMATICALLY FROM 'Assets/PS4 Controller.inputactions'

using System;
using UnityEngine;
using UnityEngine.Experimental.Input;


[Serializable]
public class PS4Controller : InputActionAssetReference
{
    public PS4Controller()
    {
    }
    public PS4Controller(InputActionAsset asset)
        : base(asset)
    {
    }
    private bool m_Initialized;
    private void Initialize()
    {
        // Gamplay
        m_Gamplay = asset.GetActionMap("Gamplay");
        m_Gamplay_Rotatecameraright = m_Gamplay.GetAction("Rotate camera right");
        m_Gamplay_Rotatecameraleft = m_Gamplay.GetAction("Rotate camera left");
        m_Gamplay_ConfirmOrder = m_Gamplay.GetAction("Confirm Order");
        m_Initialized = true;
    }
    private void Uninitialize()
    {
        m_Gamplay = null;
        m_Gamplay_Rotatecameraright = null;
        m_Gamplay_Rotatecameraleft = null;
        m_Gamplay_ConfirmOrder = null;
        m_Initialized = false;
    }
    public void SetAsset(InputActionAsset newAsset)
    {
        if (newAsset == asset) return;
        if (m_Initialized) Uninitialize();
        asset = newAsset;
    }
    public override void MakePrivateCopyOfActions()
    {
        SetAsset(ScriptableObject.Instantiate(asset));
    }
    // Gamplay
    private InputActionMap m_Gamplay;
    private InputAction m_Gamplay_Rotatecameraright;
    private InputAction m_Gamplay_Rotatecameraleft;
    private InputAction m_Gamplay_ConfirmOrder;
    public struct GamplayActions
    {
        private PS4Controller m_Wrapper;
        public GamplayActions(PS4Controller wrapper) { m_Wrapper = wrapper; }
        public InputAction @Rotatecameraright { get { return m_Wrapper.m_Gamplay_Rotatecameraright; } }
        public InputAction @Rotatecameraleft { get { return m_Wrapper.m_Gamplay_Rotatecameraleft; } }
        public InputAction @ConfirmOrder { get { return m_Wrapper.m_Gamplay_ConfirmOrder; } }
        public InputActionMap Get() { return m_Wrapper.m_Gamplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(GamplayActions set) { return set.Get(); }
    }
    public GamplayActions @Gamplay
    {
        get
        {
            if (!m_Initialized) Initialize();
            return new GamplayActions(this);
        }
    }
}
