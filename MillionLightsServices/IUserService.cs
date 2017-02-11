using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Millionlights.Models;

namespace Millionlights.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Authenticates the user and returns user id
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        int Authenticate(string userName, string password);

        /// <summary>
        /// Generates an User Token
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        AuthTokens GenerateAuthToken(int userId);

        /// <summary>
        /// Function to validate token againt expiry and existance in database.
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        bool ValidateToken(string token);

        /// <summary>
        /// Returns the Courses Enrolled by user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<dynamic> GetUsersCourses(int userId);


        /// <summary>
        /// Method to kill the provided token
        /// </summary>
        /// <param name="tokenId"></param>
        bool Delete(string token);

        /// <summary>
        /// Delete tokens for the specific deleted user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool DeleteAll(int userId);
    }
}
