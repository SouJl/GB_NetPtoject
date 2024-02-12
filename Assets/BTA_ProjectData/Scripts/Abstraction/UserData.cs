﻿using System;

namespace Abstraction
{
    public class UserData
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserEmail { get; set; }
        public int CurrentLevel { get; set; }
        public float CurrLevelProgress { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
