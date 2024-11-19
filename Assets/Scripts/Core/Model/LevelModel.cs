namespace Core.Model
{
    public class LevelModel
    {
        public int Id { get; set; }
        public int SceneIndex { get; set; }
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsUnlocked { get; set; }

        public int TotalFruits { get; set; }
        public float BestTime { get; set; }

        public int FruitsCollected { get; set; }

        public LevelModel(int id, int sceneIndex, string name, bool isCompleted, bool isUnlocked, int totalFruits,
            float bestTime, int fruitsCollected)
        {
            Id = id;
            SceneIndex = sceneIndex;
            Name = name;
            IsCompleted = isCompleted;
            IsUnlocked = isUnlocked;
            TotalFruits = totalFruits;
            BestTime = bestTime;
            FruitsCollected = fruitsCollected;
        }
    }
}