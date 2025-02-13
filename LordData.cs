namespace TORBalance
{
    internal class LordData
    {
        public string Id { get; set; }

        public string Description { get; set; }
        public float HealthBonus { get; set; }

        public LordData()
        {
            Description =string.Empty;
            HealthBonus = 0f;
        }

    }
}
