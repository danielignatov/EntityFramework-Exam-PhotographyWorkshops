namespace PhotographyWorkshops.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Camera
    {
        #region Constructor
        public Camera()
        {

        }
        #endregion

        #region Properties
        [Key]
        public int Id { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string Make { get; set; }

        [Required]
        public string Model { get; set; }

        public bool? IsFullFrame { get; set; }
        
        public int? MinISO { get; set; }

        public int? MaxISO { get; set; }

        public int? MaxShutterSpeed { get; set; }

        public string MaxVideoResolution { get; set; }

        public int? MaxFrameRate { get; set; }
        #endregion
    }
}
