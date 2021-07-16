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
        if (!context.started) return;
        if (DisabledButtons.Contains(_current)) return;
        switch (_current)
        {
            case ButtonEnum.Move:
            {
                MoveButtonPressed();
                break;
            }
            case ButtonEnum.Attack:
            {
                AttackButtonPressed();
                break;
            }
            case ButtonEnum.Special:
            {
                SpecialButtonPressed();
                break;
            }
            case ButtonEnum.Wait:
            {
                WaitButtonPressed();
                break;
            }
            case ButtonEnum.None:
                break;
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
        // TODO activeTurnTaker.turnTaker.moveController.FindAttackTiles();
    }

    private void SpecialButtonPressed()
    {
        CloseMenu();
    }

    private void WaitButtonPressed()
    {
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
        Debug.Log(_current);
        var button = Buttons[_current];
        button.AddToClassList("button-focus");
        button.RemoveFromClassList("button");
    }


    public void ShowMenu()
    {
        if (!activeTurnTaker.IsPlayer()) return;
        _menu ??= GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("menu");
        
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
        DisabledButtons.Add(ButtonEnum.Move);
        _moveButton.AddToClassList("button-disabled");
        _moveButton.RemoveFromClassList("button");
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