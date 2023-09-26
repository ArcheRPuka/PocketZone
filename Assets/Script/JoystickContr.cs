using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickContr : MonoBehaviour
{
    [Header("Joistick")]
    [SerializeField]
    private float radius = 1f;

    [Header("Player")]
    [SerializeField]
    private PlayerControl player;

    private bool acceptInput = false;

    public void InputStart()
    {
        acceptInput = true;
    }

    public void InputHold()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 pos = Camera.main.ScreenToWorldPoint(touch.position);
            transform.position = pos;
            transform.localPosition = Vector2.ClampMagnitude(transform.localPosition, radius);
        }
    }

    public void InputStop()
    {
        acceptInput = false;
        transform.localPosition = Vector2.zero;
    }
    /// <summary>
    /// sending control the player 
    /// </summary>
    private void Update()
    {
        if (acceptInput)
        {
            player.Moove(transform.localPosition);
        }
        else
        {
            player.Stand();
        }
    }
}
