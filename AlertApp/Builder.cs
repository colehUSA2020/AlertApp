using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AlertApp
{
    class Builder
    {


        [Key]
        public int BuilderId { get; set; }//PK
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }

        //This allows for the creation of a BuilderId Foreign Key in the Community Table
        //and handle multiple Communities
        public IList<Community> Communities { get; set; }

    }
}

