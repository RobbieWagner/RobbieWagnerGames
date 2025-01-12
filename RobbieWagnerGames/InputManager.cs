using RobbieWagnerGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RobbieWagnerGames
{
    public enum ActionMapName
    {
        PLAYER,
        UI
    }

    public class InputManager : MonoBehaviour
    {
        public GameControls gameControls;
        public Dictionary<string, InputActionMap> actionMaps;

        public static InputManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;

            actionMaps = new Dictionary<string, InputActionMap>();

            gameControls = new GameControls();
            gameControls.Enable();

            foreach (InputActionMap actionMap in gameControls.asset.actionMaps)
                actionMaps[actionMap.name] = actionMap;
        }

        public void EnableActionMap(ActionMapName actionMap) => EnableActionMap(actionMap.ToString());

        public void DisableActionMap(ActionMapName actionMap) => DisableActionMap(actionMap.ToString());

        public void EnableActionMap(string actionMapName)
        {
            if (actionMaps.TryGetValue(actionMapName, out var actionMap))
                actionMap.Enable();
        }

        public void DisableActionMap(string actionMapName)
        {
            if (actionMaps.TryGetValue(actionMapName, out var actionMap))
                actionMap.Disable();
        }
    }
}