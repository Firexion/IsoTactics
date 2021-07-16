using Turn;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace
{
    public class InputController : MonoBehaviour,  PlayerActions.ITileActions, PlayerActions.IUIActions
    {
        public static PlayerActions playerActions;
        public void Start()
        {
            playerActions = new PlayerActions();
            playerActions.UI.SetCallbacks(this);
            playerActions.Tile.SetCallbacks(this);
            playerActions.Enable();
        }
        
        // UI

        public void OnNavigate(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            var input = context.ReadValue<Vector2>();

            if (input.y < 0) // Go Down to the next button
            {
                UIController.FocusNextButton(true);
            }
            else if (input.y > 0) // Go Up to the previous button
            {
                UIController.FocusNextButton(false);
            }
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            UIController.Submit();
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

        // Tile
        void PlayerActions.ITileActions.OnClick(InputAction.CallbackContext context)
        {
            TurnManager.TileClicked(context);
        }
        
        public void OnSelect(InputAction.CallbackContext context)
        {
            TurnManager.TileSelected(context);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            TurnManager.TileMoved(context);
        }

        public static void MenuOpened()
        {
            playerActions.Tile.Disable();
            playerActions.UI.Enable();
        }

        public static void MenuClosed()
        {
            playerActions.UI.Disable();
            playerActions.Tile.Enable();
        }

        public static void ResetMenu()
        {
            playerActions.UI.Disable();
            playerActions.Tile.Disable();
            UIController.ResetMenu();
        }
    }
}