using Bongo.Data;

namespace Bongo.Areas.TimetableArea.Data
{
    public interface IMergerEndpoint : IBaseEndpoint
    {
        ///<summary>
        ///Initialises the merger for the current user.
        ///</summary>
        ///<param name="isForFirstSemester"></param> 
        ///<returns>
        ///<list type="string">
        ///<item>StatusCode 202 with a MergerIndexViewModel object if the user's timetable has no issues and the initialization was successful.</item>
        ///<item>StatusCode 204 if the user's timetable does not have any sessions.</item>
        ///<item>StatusCode 400 if the user's timetable has clashes or groups that need to be managed.</item>
        ///<item>Status code 404 if the user does not have a timetable to merge.</item>
        ///</list>
        /// </returns>
        public Task<HttpResponseMessage> InitialiseMerger(bool isForFirstSemester);


        ///<summary>
        ///Merges a user's timetable with the existing merged users' timetables.
        ///</summary>
        ///<param name="username">The username of the user whose timetable is being merged.</param> 
        ///<returns>
        ///<list type="string">
        ///<item>StatusCode 202 with a MergerIndexViewModel object if if the user's timetable was successfully merged with.</item>
        ///<item>StatusCode 200 with a MergerIndexViewModel object if if the user's timetable was already merged with.</item>
        ///<item>StatusCode 204 if the user's timetable does not have any sessions.</item>
        ///<item>StatusCode 400 if the user's timetable has clashes or groups that need to be managed.</item>
        ///<item>Status code 404 if the user does not have a timetable to merge.</item>
        ///</list>
        ///</returns>
        public Task<HttpResponseMessage> AddUserTimetable(string username);


        ///<summary>
        ///Removes a user's timetable from the merged timetable.
        ///</summary>
        ///<param name="username">The username of the user whose timetable must be removed from the merged timetable</param> 
        ///<returns>
        ///<list type="string">
        ///<item>StatusCode 202 if the specified user's timetable was successfully removed from the merged timetable.</item>
        ///<item>StatusCode 404 if the specified user's timetable was never merged with.</item>
        ///</list>
        ///</returns>
        public Task<HttpResponseMessage> RemoveUserTimetable(string username);
    }
}
