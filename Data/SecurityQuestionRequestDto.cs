using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecurityQuestionAuthAPI.Data
{
    public class SecurityQuestionRequestDto
    {
        public string QuestionText { get; set;}
        public string Answer { get; set; }

        public string UserEmail{get; set;}
    }
}