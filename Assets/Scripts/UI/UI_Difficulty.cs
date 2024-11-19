using UnityEngine;

namespace UI
{
    public class UI_Difficulty : MonoBehaviour
    {
        private DifficultyManager _difficultyManager;

        private void Start()
        {
            _difficultyManager = DifficultyManager.instance;
        }

        public void SetEasyDifficulty()
        {
            _difficultyManager.SetDifficulty(DifficultyType.Easy);
        }

        public void SetNormalDifficulty()
        {
            _difficultyManager.SetDifficulty(DifficultyType.Normal);
        }

        public void SetHardDifficulty()
        {
            _difficultyManager.SetDifficulty(DifficultyType.Hard);
        }
    }
}