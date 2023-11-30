using Bongo.Areas.TimetableArea.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Bongo.Areas.TimetableArea.Data
{
    public class SessionEndpoint : TimetableAreaBaseEndpoint, ISessionEndpoint
    {
        public SessionEndpoint(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            URI += "Session";
        }

        public async Task<HttpResponseMessage> AddSession(AddSessionViewModel model)
        {
            using (HttpContent content = JsonContent.Create(model))
            {
                return await Client.PostAsync(new Uri($"{URI}/AddSession"), content);
            }
        }

        public async Task<HttpResponseMessage> ConfirmGroup(AddSessionViewModel model)
        {
            using (HttpContent content = JsonContent.Create(model))
            {
                return await Client.PostAsync(new Uri($"{URI}/ConfirmGroup"), content);
            }
        }

        public async Task<HttpResponseMessage> DeleteModule(string moduleCode)
        {
            return await Client.GetAsync(new Uri($"{URI}/DeleteModule/{moduleCode}"));
        }

        public async Task<HttpResponseMessage> DeleteSession(string session)
        {
            return await Client.DeleteAsync(new Uri($"{URI}/DeleteSession/{session}"));
        }

        public async Task<HttpResponseMessage> GetClashes()
        {
            return await Client.GetAsync(new Uri($"{URI}/GetClashes"));
        }

        public async Task<HttpResponseMessage> GetGroups()
        {
            return await Client.GetAsync(new Uri($"{URI}/GetGroups"));
        }

        public async Task<HttpResponseMessage> GetSessionDetails(string sessionInPDFValue)
        {
            return await Client.GetAsync(new Uri($"{URI}/GetSessionDetails/{sessionInPDFValue}"));
        }

        public async Task<HttpResponseMessage> GetTimetableSessions(bool isForFirstSemester)
        {
            return await Client.GetAsync(new Uri($"{URI}/GetTimetableSessions/{isForFirstSemester}"));
        }

        public async Task<HttpResponseMessage> HandleClashes(string[] Sessions)
        {
            using (HttpContent content = JsonContent.Create(Sessions))
            {
                return await Client.PostAsync(new Uri($"{URI}/HandleClashes"), content);
            }
        }

        public async Task<HttpResponseMessage> HandleGroups(GroupsViewModel model)
        {
            using (HttpContent content = JsonContent.Create(model))
            {
                return await Client.PostAsync(new Uri($"{URI}/HandleGroups"), content);
            }
        }

        public async Task<HttpResponseMessage> SetColorsRandomly()
        {
            return await Client.GetAsync(new Uri($"{URI}/SetColorsRandomly"));
        }

        public async Task<HttpResponseMessage> UpdateModuleColor(SessionModuleColorsUpdate model)
        {
            using (HttpContent content = JsonContent.Create(model))
            {
                return await Client.PostAsync(new Uri($"{URI}/UpdateModuleColor"), content);
            }
        }
    }
}
