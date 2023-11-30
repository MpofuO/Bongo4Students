using Bongo.Areas.TimetableArea.Models;
using Bongo.Data;
using Microsoft.AspNetCore.Mvc;

namespace Bongo.Areas.TimetableArea.Data
{
    public interface ITimetableEndpoint : IBaseEndpoint
    {
        ///<summary>
        ///Gets the current user's timetable.
        ///</summary>
        ///<returns>
        ///<list type="string">
        ///<item>StatusCode 200 with a Timetable object if the user's timetable exists.</item>
        ///<item>StatusCode 404 with a Timetable object if the user's timetable does exists.</item>
        ///</list>
        /// </returns>
        public Task<HttpResponseMessage> GetUserTimetable();

        ///<summary>
        ///Clears the text of the current user's timetable or deletes the user's timetable.
        ///</summary>
        ///<param name="id">The identifier of the clearing process. Set to 0 if the timetable has to be deleted and 1 if only the text is cleared.</param> 
        ///<returns>
        ///<list type="string">
        ///<item>StatusCode 204 if the clearing process was successful.</item>
        ///<item>StatusCode 400 if the clearing process was unsuccessful.</item>
        ///<item>StatusCode 404 if the user does not have a timetable.</item>
        ///</list>
        ///</returns>
        public Task<HttpResponseMessage> ClearUserTable(int id);

        ///<summary>
        ///Updates or creates the current user's timetable. 
        ///</summary>
        ///<param name="text">The text where the timetable's text will be extracted.</param> 
        ///<returns>
        ///<list type="string">
        ///<item>StatusCode 200 if the timetable was successfully created or updated.</item>
        ///<item>StatusCode 400 if the given text is invalid.</item>
        ///</list>
        ///</returns>
        public Task<HttpResponseMessage> UploadOrCreate(string text);
    }
}
