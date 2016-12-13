namespace PhotographyWorkshops.Models.DTO
{
    using System.Collections.Generic;

    public class PhotographerDTO
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public List<int> Lenses { get; set; }
    }
}