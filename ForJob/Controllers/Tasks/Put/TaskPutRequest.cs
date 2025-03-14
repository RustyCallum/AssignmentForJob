﻿namespace ForJob.Controllers.Tasks.Put
{
    public class TaskPutRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}
