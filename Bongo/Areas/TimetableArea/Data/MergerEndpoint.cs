namespace Bongo.Areas.TimetableArea.Data
{
    public class MergerEndpoint : TimetableAreaBaseEndpoint, IMergerEndpoint
    {
        public MergerEndpoint(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            URI += "Merger";
        }

        public async Task<HttpResponseMessage> AddUserTimetable(string username)
        {
            return await Client.GetAsync(new Uri($"{URI}/AddUserTimetable/{username}"));
        }

        public async Task<HttpResponseMessage> InitialiseMerger(bool isForFirstSemester)
        {
            return await Client.GetAsync(new Uri($"{URI}/InitialiseMerger/{isForFirstSemester}"));
        }

        public async Task<HttpResponseMessage> RemoveUserTimetable(string username)
        {
            return await Client.GetAsync(new Uri($"{URI}/RemoveUserTimetable/{username}"));
        }
    }
}
