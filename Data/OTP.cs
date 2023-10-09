using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecurityQuestionAuthAPI.Data
{
    public class OTP
    {
        public OTP()
        {
            CreatedDate = DateTime.UtcNow;


        }

        public int Id { get; set; }
        public int Code { get; set; }
        public int UserId { get; set; }
        public User user { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public string UserEmail { get; set; }


    }
}