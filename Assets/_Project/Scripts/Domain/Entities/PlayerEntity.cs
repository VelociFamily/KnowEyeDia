namespace KnowEyeDia.Domain.Entities
{
    public class PlayerEntity
    {
        public float CurrentHealth { get; set; } = 100f;
        public float MaxHealth { get; set; } = 100f;
        public float Hunger { get; set; } = 100f;
        public float BodyTemperature { get; set; } = 37f; // Celsius
    }
}
