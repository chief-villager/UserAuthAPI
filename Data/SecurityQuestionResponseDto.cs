using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecurityQuestionAuthAPI.Data
{
    public class SecurityQuestionResponseDto
    {
        public string QuestionText { get; set;}

        public int UserId { get; set; }

        public string UserEmail{get; set;}
    }
}