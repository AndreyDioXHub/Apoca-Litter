using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static game.configuration.GameConfig;

namespace game.utils {
    public class ActivatorForMissionGame : MonoBehaviour {
        protected GameModes _gameMode = GameModes.Missions;
        // Start is called before the first frame update
        void Awake() {
            gameObject.SetActive(CurrentConfiguration.GameMode == _gameMode);
        }

    }
}