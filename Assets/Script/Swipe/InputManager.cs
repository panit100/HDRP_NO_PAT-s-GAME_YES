using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class InputManager : Singleton<InputManager>
{
    public delegate void StartTouch(Vector2 position,float time);
    public delegate void EndTouch(Vector2 position,float time);
    // public delegate void AlreadyTouch(Vector2 position);
    public event StartTouch OnStartTouch;
    // public event StartTouch OnAlreadyTouch;
    public event StartTouch OnEndTouch;
    
    PlayerControl playerControl;
    private Camera mainCamera;

    private void Awake() {
        playerControl = new PlayerControl();
        mainCamera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerControl.PlayerControls.PrimaryContact.started += ctx => StartTouchPrimary(ctx);
        // playerControl.PlayerControls.PrimaryContact.performed += ctx => AlreadyTouchPrimary(ctx);
        playerControl.PlayerControls.PrimaryContact.canceled += ctx => EndTouchPrimary(ctx);
    }

    private void OnEnable() {
        playerControl.Enable();
    }

    private void OnDisable() {
        playerControl.Disable();
    }
    
    void StartTouchPrimary(InputAction.CallbackContext context){
    // void StartTouchPrimary(){
        if(OnStartTouch != null){
            Vector2 input2 = playerControl.PlayerControls.PrimaryPosition.ReadValue<Vector2>();
            Vector3 input3 = ScreenToWorld(mainCamera,input2);
            OnStartTouch(input3,(float)context.startTime);
            // OnStartTouch(input3);
        }
    }

    // void AlreadyTouchPrimary(InputAction.CallbackContext context){
    // // void EndTouchPrimary(){
    //     if(OnAlreadyTouch != null){
    //         Vector2 input2 = playerControl.PlayerControls.PrimaryPosition.ReadValue<Vector2>();
    //         Vector3 input3 = ScreenToWorld(mainCamera,input2);
    //         OnAlreadyTouch(input3);
    //         // OnEndTouch(input3);
    //     }
    // }
    void EndTouchPrimary(InputAction.CallbackContext context){
    // void EndTouchPrimary(){
        if(OnEndTouch != null){
            Vector2 input2 = playerControl.PlayerControls.PrimaryPosition.ReadValue<Vector2>();
            Vector3 input3 = ScreenToWorld(mainCamera,input2);
            OnEndTouch(input3,(float)context.startTime);
            // OnEndTouch(input3);
        }
    }

    public Vector3 ScreenToWorld(Camera camera,Vector3 position){
        position.z = camera.nearClipPlane;
        return camera.ScreenToWorldPoint(position);
    }

    public Vector2 PrimaryPosition(){
        Vector2 input2 = playerControl.PlayerControls.PrimaryPosition.ReadValue<Vector2>();
        Vector3 input3 = ScreenToWorld(mainCamera,input2);
        return input3;
    }
}
