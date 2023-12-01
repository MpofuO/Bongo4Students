using Bongo.Areas.TimetableArea.Data;

namespace Bongo.Data
{
    public interface IEndpointWrapper
    {
        IUserEndpoint User { get; }
        IColorEndpoint Color { get; }
        IMergerEndpoint Merger { get; }
        ISessionEndpoint Session { get; }
        ITimetableEndpoint Timetable { get; }
        IAuthorizationEndpoint Authorization { get; }
    }
}