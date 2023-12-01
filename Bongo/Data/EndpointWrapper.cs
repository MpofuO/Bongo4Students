using Bongo.Areas.TimetableArea.Data;
using System.Net;

namespace Bongo.Data
{
    public class EndpointWrapper : IEndpointWrapper
    {
        private readonly IHttpContextAccessor context;
        private IUserEndpoint user;
        private IColorEndpoint color;
        private IMergerEndpoint merger;
        private ISessionEndpoint session;
        private ITimetableEndpoint timetable;
        private IAuthorizationEndpoint authorization;
        public EndpointWrapper(IHttpContextAccessor _context) { context = _context; }

        public ITimetableEndpoint Timetable
        {
            get { if (timetable is null) timetable = new TimetableEndpoint(context); return timetable; }
        }
        public IMergerEndpoint Merger
        {
            get { if (merger is null) merger = new MergerEndpoint(context); return merger; }
        }

        public ISessionEndpoint Session
        {
            get { if (session is null) session= new SessionEndpoint(context); return session; }
        }

        public IAuthorizationEndpoint Authorization
        {
            get { if (authorization is null) authorization = new AuthorizationEndpoint(context); return authorization; }
        }

        public IColorEndpoint Color
        {
            get { if (color is null) color = new ColorEndpoint(context); return color; }
        }

        public IUserEndpoint User
        {
            get { if (user is null) user = new UserEndpoint(context); return user; }
        }
    }
}