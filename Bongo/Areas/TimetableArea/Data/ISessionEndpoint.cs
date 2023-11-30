using Bongo.Areas.TimetableArea.Models.ViewModels;
using Bongo.Data;
using Microsoft.AspNetCore.Mvc;

namespace Bongo.Areas.TimetableArea.Data
{
    public interface ISessionEndpoint : IBaseEndpoint
    {
        #region Get Methods
        ///<summary>
        ///Gets the current user's sessions for the specified semester.
        ///</summary>
        ///<param name="isForFirstSemester">Specifies if the required sessions are for the first semester.</param> 
        ///<returns>
        ///<list type="string">
        ///<item>StatusCode 200 with a list of Session objects the represents the user's timetable sessions</item>
        ///<item>StatusCode 400 if the user has clashes or groups that need to be handled.</item>
        ///<item>StatusCode 404 if the user does not have a timetable.</item>
        ///</list>
        ///</returns>
        public Task<HttpResponseMessage> GetTimetableSessions(bool isForFirstSemester);


        ///<summary>
        ///Gets the clasing sessions for the current user.
        ///</summary>
        ///<returns>
        ///<list type="string">
        ///<item>StatusCode 200 with a list of list of Sessions objects representing the clashes</item>
        ///<item>StatusCode 400 if the user does not have a timetable</item>
        ///</list>
        ///</returns>
        public Task<HttpResponseMessage> GetClashes();


        ///<summary>
        ///Gets the lectures that have groups for the current user.
        ///</summary>
        ///<returns>
        ///<list type="string">
        ///<item>StatusCode 200 with a list of Lecture objects representing the clashes</item>
        ///<item>StatusCode 400 if the user does not have a timetable</item>
        ///</list>
        ///</returns>
        public Task<HttpResponseMessage> GetGroups();


        ///<summary>
        ///Gets the details of a given session.
        ///</summary>
        ///<param name="sessionInPDFValue">The value of the session in the timetable's text</param> 
        ///<returns>
        ///<list type="string">
        ///<item>StatusCode 200 with a SessionModuleColorViewModel object containing the session details.</item>
        ///<item>StatusCode 400 if the value of the session is null.</item>
        ///<item>StatusCode 404 if the value of the session is does not match any session in the timetable.</item>
        ///</list>
        ///</returns>
        public Task<HttpResponseMessage> GetSessionDetails(string sessionInPDFValue);

        ///<summary>
        ///Randomly sets the colors for the current user's modules.
        ///</summary>
        ///<returns>
        ///StatusCode 204.
        ///</returns>
        public Task<HttpResponseMessage> SetColorsRandomly();
        #endregion

        #region Post Methods
        ///<summary>
        ///Add a new session to the timetable.
        ///</summary>
        ///<param name="model"></param> 
        ///<returns>
        ///<list type="string">
        ///<item>StatusCode 200 if the session has successfully been added.</item>
        ///<item>StatusCode 409 if the added session is conflicting with an already existing session on the timetable.</item>
        ///<item>StatusCode 400 if the model is invalid.</item>
        ///</list>
        ///</returns>
        public Task<HttpResponseMessage> AddSession(AddSessionViewModel model);


        ///<summary>
        ///Confirms that an added session can be added in place of already existing one if they are now of the same session.
        ///</summary>
        ///<param name="model">The session being added.</param> 
        ///<returns>
        ///<list type="string">
        ///<item>StatusCode 200 if the session has successfully been added.</item>
        ///<item>StatusCode 400 if the model is invalid.</item>
        ///</list>
        ///</returns>
        public Task<HttpResponseMessage> ConfirmGroup(AddSessionViewModel model);


        ///<summary>
        ///Handles the user's clashing sessions according to their selection.
        ///</summary>
        ///<param name="Sessions"></param> 
        ///<returns>
        ///<list type="string">
        ///<item>Status code 200 if clashes have successfully been handled</item>
        ///<item>Status code 400 if user has selected sessions that may be clashing with each other or with already existing sessions on the timetable.</item>
        ///</list>
        ///</returns>
        public Task<HttpResponseMessage> HandleClashes(string[] Sessions);


        ///<summary>
        ///Handles the user's grouped sessions according to their selection.
        ///</summary>
        ///<param name="model"></param> 
        ///<returns>
        ///<list type="string">
        ///<item>Status code 200 if groups have successfully been handled</item>
        ///<item>Status code 400 if user has selected sessions that may be clashing with each other or with already existing sessions on the timetable.</item>
        ///</list>
        ///</returns>
        public Task<HttpResponseMessage> HandleGroups(GroupsViewModel model);


        ///<summary>
        ///Updates module colors for current user.
        ///</summary>
        ///<param name="model"></param> 
        ///<returns>
        ///<list type="string">
        ///<item>StatusCode 204 once the module colors have been successfully updated.</item>
        ///<item>StatusCode 500 if something went wrong.</item>
        /// </list>
        ///</returns>
        public Task<HttpResponseMessage> UpdateModuleColor(SessionModuleColorsUpdate model);
        #endregion

        #region Delete Methods
        ///<summary>
        ///Deletes a module from the timetable.
        ///</summary>
        ///<param name="moduleCode">The module code of the module to be deleted.</param> 
        ///<returns>
        ///StatusCode 200 after the module has been deleted.
        ///</returns>
        public Task<HttpResponseMessage> DeleteModule(string moduleCode);


        ///<summary>
        ///Deletes the given session from the timetable.
        ///</summary>
        ///<param name="session">The value of the session as it is on the timetable's text.</param> 
        ///<returns>
        ///StatusCode 200 after the session is deleted.
        ///</returns>
        public Task<HttpResponseMessage> DeleteSession(string session);
        #endregion 
    }
}
