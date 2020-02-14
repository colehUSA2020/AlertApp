using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertApp
{
    class Agent
    {

        [Key]
        public int AgentId { get; set; }//PK
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Mobile { get; set; }
        public Boolean NoCheckFlag { get; set; }

        //This allows for the creation of a AgentId Foreign Key in the AgentCommunity and CommunityData Tables
        //allowing Agent to handle multiples of AgentCommunities and CommunityData
        public IList<AgentCommunity> AgentCommunities { get; set; }
        public IList<CommunityData> CommunityData { get; set; }


    }
}

