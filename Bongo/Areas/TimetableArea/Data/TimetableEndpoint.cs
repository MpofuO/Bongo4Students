using System.Text;

namespace Bongo.Areas.TimetableArea.Data
{
    public class TimetableEndpoint : TimetableAreaBaseEndpoint, ITimetableEndpoint
    {
        public TimetableEndpoint(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            URI += "Timetable";
        }

        public async Task<HttpResponseMessage> ClearUserTable(int id)
        {
            return await Client.GetAsync(new Uri($"{URI}/ClearUserTable/{id}"));
        }

        public async Task<HttpResponseMessage> GetUserTimetable()
        {
            return await Client.GetAsync(new Uri($"{URI}/GetUserTimetable"));
        }

        public async Task<HttpResponseMessage> UploadOrCreate(string text)
        {
            using (HttpContent content = new StringContent(text, Encoding.UTF8, "application/json"))
            {
                return await Client.PostAsync(new Uri($"{URI}/UploadOrCreate"), content);
            }
        }
    }
}
