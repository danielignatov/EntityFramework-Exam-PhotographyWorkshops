namespace PhotographyWorkshops.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Photographer
    {
        #region Fields
        private ICollection<Lens> lenses;
        private ICollection<Accessory> accessories;
        private ICollection<Workshop> workshops;
        #endregion

        #region Constructor
        public Photographer()
        {
            this.lenses = new HashSet<Lens>();
            this.accessories = new HashSet<Accessory>();
            this.workshops = new HashSet<Workshop>();
        }
        #endregion

        #region Properties
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required, MinLength(2), MaxLength(50)]
        public string LastName { get; set; }

        // REGEX
        public string Phone { get; set; }

        [Required]
        public Camera PrimaryCamera { get; set; }

        [Required]
        public Camera SecondaryCamera { get; set; }

        public virtual ICollection<Lens> Lenses
        {
            get { return this.lenses; }
            set { this.lenses = value; }
        }

        public virtual ICollection<Accessory> Accessories
        {
            get { return this.accessories; }
            set { this.accessories = value; }
        }

        public virtual ICollection<Workshop> Workshops
        {
            get { return this.workshops; }
            set { this.workshops = value; }
        }
        #endregion
    }
}
