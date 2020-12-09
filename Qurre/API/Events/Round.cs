using Respawning;
using System;
using System.Collections.Generic;

namespace Qurre.API.Events
{
    public class CheckEvent : EventArgs
    {
        public CheckEvent(RoundSummary.LeadingTeam leadingTeam, bool isRoundEnd, bool isAllowed = true)
        {
            LeadingTeam = leadingTeam;
            IsRoundEnd = isRoundEnd;
            IsAllowed = isAllowed;
        }
        public RoundSummary.LeadingTeam LeadingTeam { get; set; }
        public bool IsRoundEnd { get; set; }
        public bool IsAllowed { get; set; }
    }
    public class RoundEndEvent : EventArgs
    {
        public RoundEndEvent(RoundSummary.LeadingTeam leadingTeam, RoundSummary.SumInfo_ClassList classList, int toRestart)
        {
            LeadingTeam = leadingTeam;
            ClassList = classList;
            ToRestart = toRestart;
        }
        public RoundSummary.LeadingTeam LeadingTeam { get; }
        public RoundSummary.SumInfo_ClassList ClassList { get; set; }
        public int ToRestart { get; set; }
    }
    public class TeamRespawnEvent : EventArgs
    {
        public TeamRespawnEvent(List<ReferenceHub> players, int maxRespAmount, SpawnableTeamType nextKnownTeam)
        {
            Players = players;
            MaxRespAmount = maxRespAmount;
            NextKnownTeam = nextKnownTeam;
        }
        public List<ReferenceHub> Players { get; }
        public int MaxRespAmount { get; set; }
        public SpawnableTeamType NextKnownTeam { get; set; }
    }
}