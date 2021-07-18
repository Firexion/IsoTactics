using System;
using System.Collections.Generic;
using DefaultNamespace;
using Events;
using Turn;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Utils;

public class UIController : MonoBehaviour, PlayerActions.IUIActions
{
    private static Button _moveButton;
    private static Button _attackButton;
    private static Button _specialButton;
    private static Button _waitButton;

    private static readonly Dictionary<ButtonEnum, Button> Buttons = new Dictionary<ButtonEnum, Button>();
    private static readonly List<ButtonEnum> DisabledButtons = new List<ButtonEnum>();

    private static ButtonEnum _current = ButtonEnum.None;

    private static VisualElement _menu;

    public TurnTakerVariable activeTurnTaker;
    public PlayerActionsVariable playerActions;

    public GameEvent menuClosedEvent;
    public GameEvent menuOpenedEvent;

    private float _turnStartTime;

    private enum ButtonEnum
    {
        None = 0,
        Move = 1,
        Attack = 2,
        Special = 3,
        Wait = 4
    }

    public void OnEnable()
    {
        playerActions.Value.UI.SetCallbacks(this);
        playerActions.Value.UI.Enable();
        
        
        var root = GetComponent<UIDocument>().rootVisualElement;
        _moveButton = root.Q<Button>("move-button");
        _attackButton = root.Q<Button>("attack-button");
        _specialButton = root.Q<Button>("special-button");
        _waitButton = root.Q<Button>("wait-button");

        _moveButton.clicked += MoveButtonPressed;
        _attackButton.clicked += AttackButtonPressed;
        _specialButton.clicked += SpecialButtonPressed;
        _waitButton.clicked += WaitButtonPressed;

        Buttons.Add(ButtonEnum.Move, _moveButton);
        Buttons.Add(ButtonEnum.Attack, _attackButton);
        Buttons.Add(ButtonEnum.Special, _specialButton);
        Buttons.Add(ButtonEnum.Wait, _waitButton);

        _menu = root.Q<VisualElement>("menu");
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        var input = context.ReadValue<Vector2>();
        if (input.y == 0) return;
        FocusNextButton(input.y < 0);
        // Go Down to the next button if y is negative
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        var clicked = context.ReadValueAsButton();
        if (!clicked && !context.started) return;
        if (DisabledButtons.Contains(_current)) return;
        if (Time.time - _turnStartTime < 0.001f) return; // Jank fix for a jank problem
        switch (_current)
        {
            case ButtonEnum.Move:
            {
                MoveButtonPressed();
                return;
            }
            case ButtonEnum.Attack:
            {
                AttackButtonPressed();
                return;
            }
            case ButtonEnum.Special:
            {
                SpecialButtonPressed();
                return;
            }
            case ButtonEnum.Wait:
            {
                WaitButtonPressed();
                return;
            }
            case ButtonEnum.None:
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
    }

    public void OnTab(InputAction.CallbackContext context)
    {
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
    }

    void PlayerActions.IUIActions.OnClick(InputAction.CallbackContext context)
    {
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
    }

    private void MoveButtonPressed()
    {
        if (DisabledButtons.Contains(ButtonEnum.Move)) return;
        CloseMenu();
        activeTurnTaker.Value.MoveController.FindSelectableTiles();
    }

    private void AttackButtonPressed()
    {
        CloseMenu();
        activeTurnTaker.Value.Attack.FindSelectableTiles();
    }

    private void SpecialButtonPressed()
    {
       // CloseMenu();
    }

    private void WaitButtonPressed()
    {
        ResetMenu();
        CloseMenu();
        activeTurnTaker.Value.EndTurn();
        
    }

    private static void FocusNextButton(bool forward)
    {
        while (true)
        {
            if (_current == ButtonEnum.None)
            {
                _current = forward ? _current.Next() : _current.Previous();
            }
            else
            {
                RemoveFocus();
                _current = forward ? _current.Next() : _current.Previous();
                if (_current == ButtonEnum.None)
                {
                    _current = forward ? _current.Next() : _current.Previous();
                }
            }

            if (DisabledButtons.Contains(_current))
            {
                continue;
            }

            AddFocus();
            break;
        }
    }

    private static void RemoveFocus()
    {
        var button = Buttons[_current];
        button.AddToClassList("button");
        button.RemoveFromClassList("button-focus");
    }

    private static void AddFocus()
    {
        var button = Buttons[_current];
        button.AddToClassList("button-focus");
        button.RemoveFromClassList("button");
    }


    public void ShowMenu()
    {
        if (!activeTurnTaker.IsPlayer()) return;
        _turnStartTime = Time.time;
        _menu.visible = true;
        FocusNextButton(true);
        playerActions.Value.UI.Enable();
        menuOpenedEvent.Raise();
    }

    private void CloseMenu()
    {
        _menu.visible = false;
        playerActions.Value.UI.Disable();
        menuClosedEvent.Raise();
    }

    public void DisableMove()
    {
        DisableButton(_moveButton, ButtonEnum.Move);
    }

    private static void DisableButton(VisualElement button, ButtonEnum type)
    {
        DisabledButtons.Add(type);
        button.AddToClassList("button-disabled");
        button.RemoveFromClassList("button");
    }
    
    public void DisableAttackAndSpecial()
    {
        DisableButton(_attackButton, ButtonEnum.Attack);
        DisableButton(_specialButton, ButtonEnum.Special);
    }

    public void ResetMenu()
    {
        foreach (var button in Buttons)
        {
            button.Value.RemoveFromClassList("button-disabled");
            button.Value.RemoveFromClassList("button-focus");
            button.Value.AddToClassList("button");
        }

        DisabledButtons.Clear();
        _current = ButtonEnum.None;
    }
    
}