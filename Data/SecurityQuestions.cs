using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecurityQuestionAuthAPI.Data
{
    public class SecurityQuestion
    {

        public int Id { get; set; }
        public string QuestionText { get; set;}
        public byte[] Answer { get; set; }

         public byte[] AnswerKey { get; set; }

        public int UserId { get; set; }

        public string UserEmail{get; set;}

        public virtual User User{get; set;}
    }
}