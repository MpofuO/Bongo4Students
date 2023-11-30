using Bongo.Data;

namespace Bongo.Areas.TimetableArea.Data
{
    public class TimetableAreaBaseEndpoint : BaseEndpoint, ITimetableAreaBaseEndpoint
    {
        public TimetableAreaBaseEndpoint(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            URI += "timetablearea/";
        }
    }
}
