using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AlertApp
{
    class CommunityData
    {

        [Key]
        public int CommunityDataId { get; set; }//PK

        public int CommunityId { get; set; }//FK from Community table
        public int AgentId { get; set; }//FK from Agent table

        public Community Community { get; set; }//navigation property
        public Agent Agent { get; set; }//navigation property
    }
}

