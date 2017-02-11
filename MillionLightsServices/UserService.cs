using System;
using System.Configuration;
using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json;
using Millionlights.Models;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Millionlights.Services
{
    public class UserService : IUserService
    {
        MillionlightsContext mldb = new MillionlightsContext();
        /// <summary>
        /// Public method to authenticate user by user name and password.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int Authenticate(string userName, string password)
        {
            try
            {
                var user = mldb.Users.SingleOrDefault(p => p.EmailId == userName && p.IsActive == true);

                if (user != null && user.UserId > 0)
                {
                    if (passwordDecrypt(user.Password) == password)
                    {
                        return user.UserId;
                    }
                }
            }
            catch ( Exception ex)
            {
                // log message
                throw ex;
            }
            return 0;
        }

        public AuthTokens GenerateAuthToken(int userId)
        {
            string token = Guid.NewGuid().ToString();
            DateTime issuedOn = DateTime.Now;
            DateTime expiredOn = DateTime.Now.AddSeconds(Convert.ToDouble(ConfigurationManager.AppSettings["AuthTokenExpiry"]));

            AuthTokens authToken = new AuthTokens
            {
                UserId = userId,
                Token = token,
                IssuedOn = issuedOn,
                ExpiresOn = expiredOn
            };

            mldb.AuthTokens.Add(authToken);
            mldb.SaveChanges();

            return authToken;
        }

        public bool ValidateToken(string token)
        {
            var tok = mldb.AuthTokens.Where(t => t.Token == token && t.ExpiresOn > DateTime.Now).FirstOrDefault();
            if (tok != null && !(DateTime.Now > tok.ExpiresOn))
            {
                tok.ExpiresOn = tok.ExpiresOn.AddSeconds(
                                              Convert.ToDouble(ConfigurationManager.AppSettings["AuthTokenExpiry"]));
                mldb.Entry(tok).State = System.Data.Entity.EntityState.Modified;
                mldb.SaveChanges();
                return true;
            }
            return false;
        }

        public List<dynamic> GetUsersCourses(int userId)
        {
            //Get User Courses
            var userCourses = mldb.UsersCourses.Where( u=> u.UserId == userId && u.IsActive == true).Select
                              (row => new
                              {
                                  UserId = row.UserId,
                                  CourseID = row.CourseID,
                                  CreatedOn = row.CreatedOn
                              }).Distinct();

            List<dynamic> miniCourses = new List<dynamic>();
            foreach (var userCourse in userCourses)
            {
                Course course = mldb.Courses.Where(c => c.Id == userCourse.CourseID).FirstOrDefault();

                dynamic cor = new ExpandoObject();
                cor.Id = course.Id;
                cor.CourseName = course.CourseName;
                cor.LMSCourseId = course.LMSCourseId;
                cor = JsonConvert.SerializeObject(cor);
                miniCourses.Add(cor);
            }

            return miniCourses;
        }

        public bool Delete(string token)
        {
            AuthTokens tok = mldb.AuthTokens.Where(x => x.Token == token).FirstOrDefault();
            mldb.AuthTokens.Remove(tok);
            mldb.Entry(tok).State = System.Data.Entity.EntityState.Deleted;
            mldb.SaveChanges();

            return !mldb.AuthTokens.Where(x => x.Token == token).Any();
        }

        public bool DeleteAll(int userId)
        {
            var toks = mldb.AuthTokens.Where(x => x.UserId == userId);
            mldb.AuthTokens.RemoveRange(toks);
            foreach (var tok in toks)
            {
                mldb.Entry(tok).State = System.Data.Entity.EntityState.Deleted;
            }
            mldb.SaveChanges();

            return !mldb.AuthTokens.Where(x => x.UserId == userId).Any();
        }
        public string passwordDecrypt(string cryptTxt)
        {
            if (cryptTxt != null && cryptTxt != string.Empty)
            {
                string key = "MAKV2SPBNI99212";
                cryptTxt = cryptTxt.Replace(" ", "+");
                byte[] bytesBuff = Convert.FromBase64String(cryptTxt);
                using (Aes aes = Aes.Create())
                {
                    Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    aes.Key = crypto.GetBytes(32);
                    aes.IV = crypto.GetBytes(16);
                    using (MemoryStream mStream = new MemoryStream())
                    {
                        using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cStream.Write(bytesBuff, 0, bytesBuff.Length);
                            cStream.Close();
                        }
                        cryptTxt = Encoding.Unicode.GetString(mStream.ToArray());
                    }
                }
            }
            return cryptTxt;
        }
    }
}
