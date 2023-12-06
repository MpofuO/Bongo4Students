using Bongo.Areas.TimetableArea.Data;
using Bongo.Controllers;
using System.Net;

namespace Bongo.Data
{
    public class EndpointWrapper : IEndpointWrapper
    {
        private readonly IHttpContextAccessor context;
        private static IUserEndpoint user;
        private static IColorEndpoint color;
        private static IMergerEndpoint merger;
        private static ISessionEndpoint session;
        private static ITimetableEndpoint timetable;
        private static IAuthorizationEndpoint authorization;
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

        public void Clear()
        {
            user = null;
            color = null;
            merger = null;
            timetable = null;
            session = null;
            authorization = null;
        }
    }
}