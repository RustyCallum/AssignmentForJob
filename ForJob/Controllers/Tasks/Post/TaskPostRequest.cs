namespace ForJob.Controllers.Tasks.Post
{
    public class TaskPostRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
    }
}
