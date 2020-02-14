using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AlertApp
{
    class Community
    {

        [Key]
        public int CommunityId { get; set; }//PK
        public string CommunityName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }

        public int BuilderId { get; set; }//FK from Builder table

        public Builder Builder { get; set; }//navigation property

        //This allows Community to handle many AgentCommunities
        public IList<AgentCommunity> AgentCommunities { get; set; }

    }
}

