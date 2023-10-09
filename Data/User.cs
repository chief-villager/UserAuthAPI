using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecurityQuestionAuthAPI.Data
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = "";
        public string Email { get; set; }

        public byte[] Password { get; set; }

        public byte[] PasswordKey { get; set; }

        public virtual ICollection<SecurityQuestion> securityQuestions { get; set; }

        public virtual ICollection<Phrase> phrase { get; set; }

        public virtual ICollection<OTP> otp { get; set; }



    }
}