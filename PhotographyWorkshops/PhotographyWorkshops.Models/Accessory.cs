namespace PhotographyWorkshops.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Accessory
    {
        #region Constructor
        public Accessory()
        {

        }
        #endregion

        #region Properties
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public Photographer Owner { get; set; }
        #endregion
    }
}
