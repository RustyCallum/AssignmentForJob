namespace ForJob.Controllers.Users.Create
{
    public class UserCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
