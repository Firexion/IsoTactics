using Turn;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    private Button _moveButton;
    private Button _attackButton;
    private Button _specialButton;
    private Button _waitButton;

    public static TurnManager turnManager;

    // Start is called before the first frame update
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
    }

    private static void MoveButtonPressed()
    {
        
    }

    private static void AttackButtonPressed()
    {
        
    }

    private static void SpecialButtonPressed()
    {
        
    }

    private static void WaitButtonPressed()
    {
        TurnManager.EndCurrentTurn();
    }
}