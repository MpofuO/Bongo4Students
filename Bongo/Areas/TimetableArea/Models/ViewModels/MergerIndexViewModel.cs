﻿namespace Bongo.Areas.TimetableArea.Models.ViewModels
{
    public abstract class AbstractMergerIndexViewModel
    {
        public Dictionary<string, string> Users { get; set; }
        public List<string> MergedUsers { get; set; }

    }
    public class MergerIndexViewModel : AbstractMergerIndexViewModel
    {
        public Session[,] Sessions { get; set; }
        public int latestPeriod
        {
            get
            {
                int latest = 0;
                foreach (Session session in Sessions)
                {
                    if (session != null)
                    {
                        latest = session.Period[1] > latest ? session.Period[1] : latest;
                    }
                }
                return latest;
            }
        }
    }
    public class APIMergerIndexViewModel : AbstractMergerIndexViewModel
    {
        public List<Session> Sessions { get; set; }
    }


}
