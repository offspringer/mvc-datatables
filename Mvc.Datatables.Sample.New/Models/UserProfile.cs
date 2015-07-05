using System;

namespace Mvc.Datatables.Sample.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Nickname { get; set; }

        public string Email { get; set; }

        public string Country { get; set; }

        public DateTime BirthDate { get; set; }

        public UserProfile()
        {
        }
    }
}