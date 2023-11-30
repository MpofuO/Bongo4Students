using Bongo.Areas.TimetableArea.Models;
using Bongo.Data;
using System.Text.RegularExpressions;

namespace Bongo.Areas.TimetableArea.Infrastructure
{
    public static class SessionControlHelpers
    {
        public static int GetInterval(string sessionPdfValue)
        {
            Regex timePattern = new Regex(@"(\d{2}:\d{2}) (\d{2}:\d{2})");
            Match timeMatch = timePattern.Match(sessionPdfValue);
            int startTime = int.Parse(timeMatch.Groups[1].Value.Substring(0, 2));
            int endTime = int.Parse(timeMatch.Groups[2].Value.Substring(0, 2));
            return endTime - startTime;
        }
        public static Session[,] Get2DArray(List<Session> Sessions)
        {
            var array = new Session[5, 16];

            foreach (var s in Sessions)
                array[s.Period[0] - 1, s.Period[1] - 1] = s;

            return array;
        }
        public static bool HasGroups(List<Session> sessions)
        {
            Regex groupPattern = new Regex(@"Group [A-Z]{1,2}[\d]?");
            List<string> groups = new List<string>();
            foreach (var session in sessions)
            {
                Match groupMatch = groupPattern.Match(session.sessionInPDFValue);
                if (groupMatch.Success)
                {
                    groups.Add(groupMatch.Value);
                }
            }
            return groups.Distinct().Count() < groups.Count;
        }
    }

}
