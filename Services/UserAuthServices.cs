using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Nelibur.ObjectMapper;
using SecurityQuestionAuthAPI.Data;
using System.Net.Mail;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using Hangfire;


namespace SecurityQuestionAuthAPI.Services
{
    public class UserAuthServices
    {
        SecurityAuthContext _securityAuthContext;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public UserAuthServices(SecurityAuthContext securityAuthContext, IBackgroundJobClient backgroundJobClient)
        {
            _securityAuthContext = securityAuthContext;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var response = await _securityAuthContext.Users.ToListAsync();
            TinyMapper.Bind<List<User>, List<UserResponseDto>>();
            var result = TinyMapper.Map<List<UserResponseDto>>(response);
            return result;

        }

        public async Task<UserResponseDto> GetUserByIdAsync(int Id)
        {
            var response = await _securityAuthContext.Users.SingleOrDefaultAsync(x => x.Id == Id);
            TinyMapper.Bind<User, UserResponseDto>();
            var result = TinyMapper.Map<UserResponseDto>(response);
            return result;

        }

        public async Task<User> CreateUserAsync([FromBody] string username, string password, string email)
        {
            byte[] passwordkey;
            byte[] passwordhash;

            using (var HMAC = new HMACSHA512())
            {
                passwordkey = HMAC.Key;
                passwordhash = HMAC.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            User user = new User();
            user.UserName = username;
            user.Password = passwordhash;
            user.PasswordKey = passwordkey;
            user.Email = email;

            await _securityAuthContext.AddAsync(user);
            await _securityAuthContext.SaveChangesAsync();
            var messageBody = $"welcome to our platform {username}";
            var messageSubject = $"welcome {username}";
            _backgroundJobClient.Schedule(() => SmtpSenderProfile(email, messageBody, messageSubject), TimeSpan.FromSeconds(20));
            return user;
        }

        public async Task<SecurityQuestion> CreateSecurityQuestionAndAnswerAsync(string securityQuestion, string answer, string userEmail)
        {
            byte[] securityAnswerKey;
            byte[] securityAnswerHash;
            var USER = await _securityAuthContext.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

            using (var hmac = new HMACSHA1())
            {
                securityAnswerKey = hmac.Key;
                securityAnswerHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(answer));

            }

            SecurityQuestion security = new SecurityQuestion();
            security.QuestionText = securityQuestion;
            security.Answer = securityAnswerHash;
            security.AnswerKey = securityAnswerKey;
            security.UserId = USER.Id;
            security.UserEmail = userEmail;

            await _securityAuthContext.AddAsync(security);
            await _securityAuthContext.SaveChangesAsync();
            return security;
        }



        public bool securityAnswerCheckAsync(string answer, byte[] securityAnwser, byte[] securityAnserKey)
        {
            using (var hmac = new HMACSHA1(securityAnserKey))
            {
                var newAnswerHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(answer));
                for (int i = 0; i < securityAnwser.Length; i++)
                {
                    if (newAnswerHash[i] != securityAnwser[i])
                    {
                        return false;
                    }


                }

                return true;
            }

        }

        public bool GetSecurityQuestionByEmailAsync(string userEmail)
        {
            var response = _securityAuthContext.securityQuestions.FirstOrDefault(x => x.UserEmail == userEmail);
            if (response == null)
            {
                return false;
            }
            return true;
        }

        public async Task<User> ResetPasswordByQuestionAsync(string email, string question, string answer, string newpassword)
        {
            var existingUser = await _securityAuthContext.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (!GetSecurityQuestionByEmailAsync(email))
            {
                return null;
            }
            var response = await _securityAuthContext.securityQuestions.FirstOrDefaultAsync(x => x.QuestionText == question);
            if (response == null)
            {
                return null;
            }

            if (!securityAnswerCheckAsync(answer, response.Answer, response.AnswerKey))
            {
                return null;

            }

            byte[] newPasswordHash, newPasswordKey;
            using (var hmac = new HMACSHA512())
            {
                newPasswordKey = hmac.Key;
                newPasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(newpassword));
            }
            existingUser.Password = newPasswordHash;
            existingUser.PasswordKey = newPasswordKey;
            await _securityAuthContext.SaveChangesAsync();
            return existingUser;

        }

        public async Task<IEnumerable<SecurityQuestionResponseDto>> GetallSecurityQuestionsAsyncByemail(string email)
        {
            var response = await _securityAuthContext.securityQuestions.Where(x => x.UserEmail == email).ToListAsync();
            if (response == null)
            {
                return null;
            }
            TinyMapper.Bind<List<SecurityQuestion>, List<SecurityQuestionResponseDto>>();
            var result = TinyMapper.Map<List<SecurityQuestionResponseDto>>(response);
            return result;
        }


        public string GenerateCatchPhrase()
        {
            List<string> myList = new List<string> { "dog", "rat", "cat", "bat", "cow", "mouse", "pig", "fish" };

            Random random = new Random();

            // Shuffle the list using the Fisher-Yates algorithm
            for (int i = myList.Count - 1; i > 0; i--)
            {
                int randomIndex = random.Next(0, i + 1);
                string temp = myList[i];
                myList[i] = myList[randomIndex];
                myList[randomIndex] = temp;
            }

            // Take the first 4 elements from the shuffled list
            var phrase = myList.GetRange(0, 4);
            var phraseString = string.Join(" ", phrase);
            return phraseString;
        }

        public byte[] EncryptPhrase(string phrase, byte[] phrasekey, byte[] iv)
        {

            using (Aes aesAlg = Aes.Create())
            {


                aesAlg.Key = phrasekey;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using MemoryStream msEncrypt = new MemoryStream();

                using CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    //Write all data to the stream.
                    swEncrypt.Write(phrase);
                }
                return msEncrypt.ToArray();

            }


        }


        public string DecryptPhrase(byte[] cipherText, byte[] Key, byte[] IV)
        {

            using Aes aesAlg = Aes.Create();

            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new MemoryStream(cipherText);

            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

            using StreamReader srDecrypt = new StreamReader(csDecrypt);
            string plaintext = srDecrypt.ReadToEnd();
            return plaintext;

        }


        public async Task<Phrase> CreatePhraseAsync(string useremail)
        {
            var User = await _securityAuthContext.Users.FirstOrDefaultAsync(x => x.Email == useremail);
            if (User == null)
            {
                return null;
            }
            var phrases = GenerateCatchPhrase();


            byte[] encryptionKey = new byte[32];
            byte[] iv = new byte[16];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(encryptionKey);
                rng.GetBytes(iv);
            }

            var encyptedPhrase = EncryptPhrase(phrases, encryptionKey, iv);


            Phrase phrase = new Phrase();
            phrase.PhraseText = encyptedPhrase;
            phrase.UserEmail = User.Email;
            phrase.UserId = User.Id;
            phrase.PhraseKey = encryptionKey;
            phrase.IV = iv;

            if (encyptedPhrase == null)
            {
                return null;
            }
            await _securityAuthContext.AddAsync(phrase);
            await _securityAuthContext.SaveChangesAsync();
            return phrase;

        }
        public async Task<string> GetRecoveryPhrase(string email)
        {
            var recovery = await CreatePhraseAsync(email);

            var phrase = DecryptPhrase(recovery.PhraseText, recovery.PhraseKey, recovery.IV);
            var rphrase = string.Join(", ", phrase);
            return rphrase;
        }

        public async Task<User> ResetPasswordUsingPhrase(string newpassword, string phrase, string email)
        {

            var existingUser = await _securityAuthContext.Users.FirstOrDefaultAsync(x => x.Email == email);
            var existingPhrase = await _securityAuthContext.phrases.FirstOrDefaultAsync(x => x.UserEmail == email);
            var phraseToLower = phrase.Replace(" ", "").ToLower();
            if (phraseToLower == null)
            {
                return null;
            }
            var decrypedPhrase = DecryptPhrase(existingPhrase.PhraseText, existingPhrase.PhraseKey, existingPhrase.IV);
            var decrypedPhraseToLower = decrypedPhrase.Replace(" ", "").ToLower();
            if (decrypedPhraseToLower != phraseToLower)
            {
                return null;
            }

            using (HMAC hmac = new HMACSHA512())
            {
                existingUser.PasswordKey = hmac.Key;
                existingUser.Password = hmac.ComputeHash(System.Text.Encoding.Unicode.GetBytes(newpassword));

            }
            await _securityAuthContext.SaveChangesAsync();
            return existingUser;


        }

        public int GenerateOtp()
        {
            Random rand = new Random();
            return rand.Next(100000, 999999);
        }

        public bool VerifyOtp(DateTime otpCreationTime)
        {
            TimeSpan elapsed = DateTime.UtcNow - otpCreationTime;
            if (elapsed.TotalMinutes > 5)
            {
                // OTP has expired
                return false;
            }
            return true;
        }

        public async Task<int> CreateAndSaveOTP(string email)
        {
            var OTP = GenerateOtp();
            var existingUser = await _securityAuthContext.Users.FirstOrDefaultAsync(x => x.Email == email);
            OTP oTP = new OTP();
            oTP.Code = OTP;
            oTP.user = existingUser;
            oTP.UserEmail = email;
            oTP.UserId = existingUser.Id;

            await _securityAuthContext.AddAsync(oTP);
            await _securityAuthContext.SaveChangesAsync();
            return OTP;

        }

        public async Task<bool> DeleteOTP(string email)
        {
            var response = await _securityAuthContext.Otp.Where(x => x.UserEmail == email).ToListAsync();
            if (response == null)
            {
                return false;
            }
            response.ForEach(item => _securityAuthContext.Remove(item));
            await _securityAuthContext.SaveChangesAsync();
            return true;
        }

        public async Task<int> SendOtpToUserEmailAsync(string email)
        {

            var OTP = await CreateAndSaveOTP(email);
            if (OTP == null)
            {
                throw new Exception("Otp not Generated");
            }
            string subject = "Your OTP Code";
            string body = $"Your OTP code is: {OTP}";
            string SendingMail = "ssolis4789@gmail.com";
            string Password = "wnvfifrtwjgyblcy";
            _backgroundJobClient.Enqueue(() => SmtpSenderProfile(email, body, subject));
            _backgroundJobClient.Schedule(() => DeleteOTP(email), TimeSpan.FromMinutes(15));

            return OTP;

        }

        public async Task SmtpSenderProfile(string email, string messageBody, string messageSubject)
        {
            string subject = messageSubject;
            string body = messageBody;
            string SendingMail = "";
            string Password = "";


            using (SmtpClient smtpClient = new SmtpClient())
            {
                smtpClient.Credentials = new NetworkCredential(SendingMail, Password);
                smtpClient.Host = "smtp.gmail.com";
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(SendingMail);
                    mailMessage.To.Add(email);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;




                    await smtpClient.SendMailAsync(mailMessage);

                }
            }

        }

        public async Task<User> ResetPasswordWithOTP(string NewPasword, int Otp)
        {

            var OTP = await _securityAuthContext.Otp.FirstOrDefaultAsync(x => x.Code == Otp);
            var verifyOTPTime = VerifyOtp(OTP.CreatedDate);
            if (!verifyOTPTime || OTP == null)
            {
                return null;
            }
            var existingUser = await _securityAuthContext.Users.FirstOrDefaultAsync(x => x.Email == OTP.UserEmail);
            if (OTP.Code != Otp)
            {
                return null;
            }
            using (var hmac = new HMACSHA512())
            {
                existingUser.Password = hmac.Key;
                existingUser.PasswordKey = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(NewPasword));
            }
            await _securityAuthContext.SaveChangesAsync();
            return existingUser;

        }

        public async Task<bool> ActiveOtp(string email)
        {
            var currentTimeUtc = DateTime.UtcNow;
            var response = _securityAuthContext.Otp.Where(x => x.UserEmail == email && x.IsActive
            && currentTimeUtc - x.CreatedDate > TimeSpan.FromMinutes(5)).ToList();
            if (response.Any())
            {
                response.ForEach(item => item.IsActive = false);
            }
            await _securityAuthContext.SaveChangesAsync();
            return response.Any();



        }


    }


}



