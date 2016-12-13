using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotographyWorkshops.Models
{
    public class Workshop
    {
        #region Fields
        private ICollection<Photographer> participants;
        #endregion

        #region Constructor
        public Workshop()
        {
            this.participants = new HashSet<Photographer>();
        }
        #endregion

        #region Properties
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public decimal PricePerParticipant { get; set; }

        [Required]
        public Photographer Trainer { get; set; }

        public virtual ICollection<Photographer> Participants
        {
            get { return this.participants; }
            set { this.participants = value; }
        }
        #endregion
    }
}
