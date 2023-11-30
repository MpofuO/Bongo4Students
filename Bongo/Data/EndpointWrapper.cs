using Bongo.Areas.TimetableArea.Data;
using System.Net;

namespace Bongo.Data
{
    public class EndpointWrapper : IEndpointWrapper
    {
        private readonly IHttpContextAccessor context;
        private IColorEndpoint color;
        private IMergerEndpoint merger;
        private ISessionEndpoint session;
        private ITimetableEndpoint timetable;
        private IAuthorizationEndpoint authorization;
        public EndpointWrapper(IHttpContextAccessor _context) { context = _context; }

        public ITimetableEndpoint Timetable
        {
            get
            {
                if (timetable == null)
                    timetable = new TimetableEndpoint(context);
                return timetable;
            }
        }

        public IMergerEndpoint Merger
        {
            get
            {
                if (merger == null)
                    merger = new MergerEndpoint(context);
                return merger;
            }
        }

        public ISessionEndpoint Session
        {
            get
            {
                if (session == null)
                    session = new SessionEndpoint(context);
                return session;
            }
        }

        public IAuthorizationEndpoint Authorization
        {
            get
            {
                if (authorization == null)
                    authorization = new AuthorizationEndpoint(context);
                return authorization;
            }
        }

        public IColorEndpoint Color
        {
            get
            {
                if (color == null)
                    color = new ColorEndpoint(context);
                return color;
            }
        }
    }
}