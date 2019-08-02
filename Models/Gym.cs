namespace PoGoChatbot.Models
{
    public class Gym
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsEXEligible { get; set; }
        public LatLng Location { get; set; }
    }
}
