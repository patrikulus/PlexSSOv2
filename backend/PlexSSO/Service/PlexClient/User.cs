using PlexSSO.Common.Model.Types;

namespace PlexSSO.Service.PlexClient
{
    public class User
    {
        public Username Username { get; }
        public Email Email { get; }
        public Thumbnail Thumbnail { get; }

        public User(string username, string email, string thumbnail)
        {
            Username = string.IsNullOrWhiteSpace(username)
                ? new Username(email)
                : new Username(username);
            Email = string.IsNullOrWhiteSpace(email)
                ? null
                : new Email(email);
            Thumbnail = string.IsNullOrWhiteSpace(thumbnail)
                ? null
                : new Thumbnail(thumbnail);
        }
    }
}
