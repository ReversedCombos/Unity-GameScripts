using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMovement : MonoBehaviour
{
    [Header("Camera Speeds")]
    [SerializeField] private float Camera_m_Speed;
    [SerializeField] private float Camera_m_Max;
    [Header("Player Speeds")]
    [SerializeField] private float Player_m_Speed;
    [SerializeField] private float Player_m_Max;
    [Header("Smoothing")]
    [SerializeField] private float m_Smooth;

    private Vector3 initialPosition;
    private PlayerActions PlayerActions;
    void Awake()
    {
        initialPosition = transform.localPosition;
        PlayerActions = new PlayerActions();

        PlayerActions.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        float CameraMovementX = Mathf.Clamp(PlayerActions.Looking.Move.ReadValue<Vector2>().x, -Camera_m_Max, Camera_m_Max);
        float CameraMovementY = Mathf.Clamp(PlayerActions.Looking.Move.ReadValue<Vector2>().y, -Camera_m_Max, Camera_m_Max);

        float PlayerMovementX = Mathf.Clamp(PlayerActions.Movement.Move.ReadValue<Vector2>().x, -Player_m_Speed, Player_m_Speed);
        float PlayerMovementY = Mathf.Clamp(PlayerActions.Movement.Move.ReadValue<Vector2>().y, -Player_m_Speed, Player_m_Speed);

        Vector3 CameraFinalPosition = new Vector3(CameraMovementX, CameraMovementY, 0);

        Vector3 PlayerFinalPosition = new Vector3(PlayerMovementX, 0, PlayerMovementY);

        transform.localPosition = Vector3.Lerp(transform.localPosition, CameraFinalPosition + PlayerFinalPosition  + initialPosition, Time.deltaTime * m_Smooth);
    }
}
