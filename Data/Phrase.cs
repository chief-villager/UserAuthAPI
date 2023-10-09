using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecurityQuestionAuthAPI.Data
{
    public class Phrase
    {
        public int Id{get; set;}
        public byte[] PhraseText{get; set;}

        public int UserId{get; set;}

        public string UserEmail {get; set;}

        public virtual User user{get; set;}

        public byte[] PhraseKey{get; set;}

        public byte[] IV {get; set;}
    }
}