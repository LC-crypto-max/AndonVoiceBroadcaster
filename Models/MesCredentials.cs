namespace AndonVoiceBroadcaster.Models
{
    public sealed class MesCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public bool IsComplete()
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }
    }
}
