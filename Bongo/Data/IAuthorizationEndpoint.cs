using Bongo.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Bongo.Data
{
    public interface IAuthorizationEndpoint:IBaseEndpoint
    {
        /// <summary>
        /// Logs the user in
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// <list type="string">
        /// <item>StatusCode 200 with the user object if the user is successfully authenticated.</item>
        /// <item> StatusCode 400 otherwise</item>
        /// </list>
        /// </returns>
        public Task<HttpResponseMessage> Login(LoginViewModel model);

        /// <summary>
        /// Logs the user out
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// <list type="string">
        /// <item>StatusCode 204.</item>
        /// </list>
        /// </returns>
        public Task<HttpResponseMessage> Logout();

        /// <summary>
        /// Registers a user
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// <list type="string">
        /// <item>StatusCode 200 if the user is successfully registered.</item>
        /// <item>StatusCode 409 if there was a email conflict.</item>
        /// <item>StatusCode 406 with an IEnumerable object of IdentityError if registration failed at user creation.</item>
        /// <item>StatusCode 400 if the model was not valid.</item>
        /// <item>StatusCode 500 if there was a server error.</item>
        /// </list>
        /// </returns>
        public Task<HttpResponseMessage> Register(RegisterModel registerModel);

        /// <summary>
        /// Verifies if a user with the given email exists
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// <list type="string">
        /// <item>StatusCode 200 if the user exists, 404 otherwise.</item>
        /// </list>
        /// </returns>
        public Task<HttpResponseMessage> VerifyEmail(string email);

        /// <summary>
        /// Initiates a password reset for a user once the user's answer to security question is validated.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// <list type="string">
        /// <item>StatusCode 200 with the an array containing the reset password token and the userId respectively if the token was successfully created.</item>
        /// <item>StatusCode 400 if the model was not valid.</item>
        /// <item>StatusCode 404 if user with the given email does not exist.</item>
        /// <item>StatusCode 406 if the given answer to the security question is incorrect.</item>
        /// </list>
        /// </returns>
        public Task<HttpResponseMessage> ForgotPassword(AnswerSecurityQuestionViewModel model);

        /// <summary>
        /// Initiates a change password event by creating a password reset token
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// <list type="string">
        /// <item>StatusCode 200 with the an array containing the reset password token and the userId respectively if the token was successfully created.</item>
        /// <item>StatusCode 400 if the request is potentially an attempt wrongfully reset a user's password.</item>
        /// <item>StatusCode 404 if the given userId matches no user.</item>
        /// </list>
        /// </returns>
        public Task<HttpResponseMessage> ChangePassword(string userId);

        /// <summary>
        /// Resets a user's password.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// <list type="string">
        /// <item>StatusCode 200 if the password was successfully reset.</item>
        /// <item>StatusCode 406 with an IEnumerable object of IdentityError if resetting the password failed.</item>
        /// <item>StatusCode 400 if model was not valid.</item>
        /// <item>StatusCode 404 if the given userId matches no user.</item>
        /// </list>
        /// </returns>
        public Task<HttpResponseMessage> ResetPassword(ResetPassword model);

        /// <summary>
        /// Updates a user's security question and it's answer.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// <list type="string">
        /// <item>StatusCode 200 if the security question and answer were successfully set.</item>
        /// <item>StatusCode 400 if model was not valid.</item>
        /// <item>StatusCode 404 if the given userId matches no user.</item>
        /// </list>
        /// </returns>
        public Task<HttpResponseMessage> UpdateSecurityQuestion(SecurityQuestionViewModel model);
    }
}
