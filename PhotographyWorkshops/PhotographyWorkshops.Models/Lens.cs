namespace PhotographyWorkshops.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Lens
    {
        #region Constructor
        public Lens()
        {

        }
        #endregion

        #region Properties
        [Key]
        public int Id { get; set; }

        public string Make { get; set; }

        public int? FocalLength { get; set; }

        public float? MaxAperture { get; set; }

        public string CompatibleWith { get; set; }

        public Photographer Owner { get; set; }
        #endregion
    }
}