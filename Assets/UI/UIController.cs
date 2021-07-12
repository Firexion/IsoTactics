using System;
using System.Collections.Generic;
using DefaultNamespace;
using Turn;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

public class UIController : MonoBehaviour
{
    private static Button _moveButton;
    private static Button _attackButton;
    private static Button _specialButton;
    private static Button _waitButton;

    private static readonly Dictionary<ButtonEnum, Button> Buttons = new Dictionary<ButtonEnum, Button>();
    private static readonly List<ButtonEnum> DisabledButtons = new List<ButtonEnum>();

    private static ButtonEnum _current = ButtonEnum.None;

    private static VisualElement _menu;

    private enum ButtonEnum
    {
        None = 0,
        Move = 1,
        Attack = 2,
        Special = 3,
        Wait = 4
    }

    private void Start()
    {
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

    private static void MoveButtonPressed()
    {
        if (DisabledButtons.Contains(ButtonEnum.Move)) return;
        _menu.visible = false;
        InputController.MenuClosed();
        TurnManager.FindSelectableTiles();
    }

    private static void AttackButtonPressed()
    {
        _menu.visible = false;
        InputController.MenuClosed();
        TurnManager.FindAttackTiles();
    }

    private static void SpecialButtonPressed()
    {
    }

    private static void WaitButtonPressed()
    {
        _menu.visible = false;
        TurnManager.EndCurrentTurn();
    }

    public static void FocusNextButton(bool forward)
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
            FocusNextButton(forward);
            return;
        }

        AddFocus();
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


    public static void ShowMenu()
    {
        InputController.MenuOpened();
        _menu.visible = true;
        FocusNextButton(true);
    }

    public static void DisableMove()
    {
        DisabledButtons.Add(ButtonEnum.Move);
        _moveButton.AddToClassList("button-disabled");
        _moveButton.RemoveFromClassList("button");
    }

    public static void Submit()
    {
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

    public static void ResetMenu()
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