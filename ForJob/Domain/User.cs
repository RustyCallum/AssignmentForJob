﻿namespace ForJob.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public byte[] Hash { get; set; }
        public byte[] Salt { get; set; }
    }
}
