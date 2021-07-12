using System.Collections.Generic;
using Turn;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour, PlayerActions.IUIActions
{
    private static Button _moveButton;
    private static Button _attackButton;
    private static Button _specialButton;
    private static Button _waitButton;

    private readonly List<Button> _buttons = new List<Button>();
    private static readonly List<string> _disabledButtons = new List<string>();
    private int _selectedIndex = -1;
    private PlayerActions _playerActions;

    private static VisualElement _menu;

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

        _buttons.Add(_moveButton);
        _buttons.Add(_attackButton);
        _buttons.Add(_specialButton);
        _buttons.Add(_waitButton);

        _playerActions = new PlayerActions();
        _playerActions.UI.SetCallbacks(this);
        _playerActions.Enable();

        _menu = root.Q<VisualElement>("menu");
    }

    private static void MoveButtonPressed()
    {
        if (_disabledButtons.Contains("move-button")) return;
        _menu.visible = false;
        TurnManager.FindMoveTiles();
    }

    private static void AttackButtonPressed()
    {
        _menu.visible = false;
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

    public void OnNavigate(InputAction.CallbackContext context)
    {
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
    }

    public void OnTab(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (context.ReadValueAsButton())
        {
            FocusNextButton();
        }
        else
        {
            FocusPreviousButton();
        }
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        var mousePosition = Mouse.current.position.ReadValue();
        Debug.Log("Clicked at " + mousePosition.x + ", " + mousePosition.y);
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

    private void FocusNextButton()
    {
        if (_selectedIndex == -1)
        {
            var button = _buttons[0];
            if (_disabledButtons.Contains(button.name))
            {
                FocusAgain(true);
                return;
            }

            button.AddToClassList("button-focus");
            button.RemoveFromClassList("button");
            _selectedIndex = 0;
        }
        else
        {
            var button = _buttons[_selectedIndex];
            button.AddToClassList("button");
            button.RemoveFromClassList("button-focus");

            _selectedIndex = _selectedIndex == _buttons.Count - 1 ? 0 : _selectedIndex + 1;
            button = _buttons[_selectedIndex];
            if (_disabledButtons.Contains(button.name))
            {
                FocusAgain(true);
                return;
            }

            button.AddToClassList("button-focus");
            button.RemoveFromClassList("button");
        }
    }

    private void FocusAgain(bool forward)
    {
        if (forward)
        {
            _selectedIndex = _selectedIndex == 0 ? _selectedIndex : _selectedIndex + 1;
            FocusNextButton();
        }
        else
        {
            _selectedIndex = _selectedIndex == _buttons.Count - 1 ? _selectedIndex : _selectedIndex - 1;
            FocusPreviousButton();
        }
    }

    private void FocusPreviousButton()
    {
        if (_selectedIndex == -1)
        {
            _selectedIndex = _buttons.Count - 1;
            var button = _buttons[_selectedIndex];
            if (_disabledButtons.Contains(button.name))
            {
                FocusAgain(false);
                return;
            }
            button.AddToClassList("button-focus");
            button.RemoveFromClassList("button");
        }
        else
        {
            var button = _buttons[_selectedIndex];
            button.AddToClassList("button");
            button.RemoveFromClassList("button-focus");

            _selectedIndex = _selectedIndex == 0 ? _buttons.Count - 1 : _selectedIndex - 1;
            button = _buttons[_selectedIndex];
            if (_disabledButtons.Contains(button.name))
            {
                FocusAgain(false);
                return;
            }
            button.AddToClassList("button-focus");
            button.RemoveFromClassList("button");
        }
    }

    public static void ShowMenu()
    {
        _menu.visible = true;
    }

    public static void DisableMove()
    {
        _disabledButtons.Add("move-button");
        _moveButton.AddToClassList("button-disabled");
        _moveButton.RemoveFromClassList("button");
    }
}