using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace SustainableCambodiaETL.lib
{
    internal class Child
    {
        public string firstName;
        public string lastName;
        public string gender;
        public string biography;
        public bool fullySponsored;
        public DateTime dob;
        public DateTime dateCreated;
        public DateTime dateUpdated;
        public DateTime biographyUpdated;
        public bool deleted;
        public bool eligibleHomeSponsor;
        public bool eligibleSchoolSponsor;
        public bool eligibleScholarshipSponsor;
        public bool legacySponsored;


        public Child() { }

        public Child(string fname, string lname, string gender, string bio, bool fullySponsored)
        {
            this.firstName = fname;
            this.lastName = lname;
            this.gender = gender;
            this.biography = bio;
            this.fullySponsored = fullySponsored;
        }
    }
}
