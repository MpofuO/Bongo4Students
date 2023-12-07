using Bongo.MockAPI.Bongo.Areas.TimetableArea.Infrastructure;
using Bongo.MockAPI.Bongo.Areas.TimetableArea.Models;
using Bongo.MockAPI.Bongo.Data;
using Bongo.MockAPI.Bongo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Bongo.MockAPI.Bongo.Areas.TimetableArea.Controllers;

[Authorize]
public class MockAPITimetableController : ControllerBase
{
    private IRepositoryWrapper _repository;
    private string Username;

    public MockAPITimetableController(IRepositoryWrapper repo, UserManager<BongoUser> userManager, string username)
    {
        _repository = repo;
        Username = username;
    }

    #region GetMethods

    ///<summary>
    ///Gets the current user's timetable.
    ///</summary>
    ///<returns>
    ///<list type="string">
    ///<item>StatusCode 200 with a Timetable object if the user's timetable exists.</item>
    ///<item>StatusCode 404 with a Timetable object if the user's timetable does exists.</item>
    ///</list>
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetUserTimetable()
    {
        var table = _repository.Timetable.GetUserTimetable(Username);
        if (table is not null)
            return Ok(table);

        return NotFound("User has no timetable.");
    }

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
    [HttpGet]
    public async Task<IActionResult> ClearUserTable(int id)
    {
        Timetable table = _repository.Timetable.GetUserTimetable(Username);
        var moduleColor = _repository.ModuleColor.GetByCondition(m => m.Username == Username);
        if (table != null)
        {
            try
            {
                if (id == 0)
                    _repository.Timetable.Delete(table);
                else
                {
                    table.TimetableText = "";
                    _repository.Timetable.Update(table);
                }

                if (moduleColor != null)
                {
                    foreach (var item in moduleColor)
                    {
                        _repository.ModuleColor.Delete(item);
                    }
                }
                _repository.SaveChanges();

                return StatusCode(204, "Timetable successfully cleared.");
            }
            catch
            {
                return BadRequest("Something went wrong");
            }
        }

        return NotFound("Timetable does not exist.");
    }
    #endregion GetMethods

    #region PostMethods

    ///<summary>
    ///Updates or creates the current user's timetable. 
    ///</summary>
    ///<param name="text">The text where the timetable's text will be extracted.</param> 
    ///<returns>
    ///<list type="string">
    ///<item>StatusCode 200 if the timetable was successfully created or updated.</item>
    ///<item>StatusCode 400 if the given text is invalid.</item>
    ///</list>
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> UpdateOrCreate(string text)
    {
        //Remove unwanted text
        Regex patternTop = new Regex(@"(\d{4}) CLASS TIMETABLE\n(\d{10})");
        Match match = patternTop.Match(text);
        if (match.Success)
        {
            Regex pattern = new Regex(@"205 Nelson Mandela Drive  \|  Park West, Bloemfontein 9301 \| South Africa\nP\.O\. Box 339  \|  Bloemfontein 9300  \|  South Africa \| www\.ufs\.ac\.za|\nVenue Start End Day From To|Venue Start End Day From To\n");//|\(Group [A-Z]{1,2}\)|
            text = pattern.Replace(text, String.Empty);
            CreateOrUpdate(text);
        }
        else if (string.IsNullOrEmpty(text))
            CreateOrUpdate(text);
        else
            return BadRequest("Something went wrong while uploading timetable. " +
                "\n Please make sure your have uploaded your personal timetable");


        return Ok("Timetable created/updated successfully");
    }
    private void CreateOrUpdate(string text)
    {
        Timetable timetable = _repository.Timetable.GetUserTimetable(Username) ?? new Timetable { TimetableText = text, Username = Username };

        if (timetable.TimetableID == 0)
            _repository.Timetable.Create(timetable);
        else
            _repository.Timetable.Update(timetable);

        if (!string.IsNullOrEmpty(text))
            SessionControlHelpers.AddNewUserModuleColor(ref _repository, Username, timetable.TimetableText);

        _repository.SaveChanges();
    }
    #endregion PostMethods
}