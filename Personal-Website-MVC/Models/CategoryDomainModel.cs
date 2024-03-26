using System.ComponentModel.DataAnnotations;//added for access to Data Annotations

namespace Personal_Website_MVC.Models
{
    //This class was manually built to serve as a model for the 
    //data in Northwind's Categories table. Since we built it
    //manually and it IS directly tied to a database table, 
    //this class is refered to as a Domain Model.
    public class CategoryDomainModel
    {
        [Key]
        //The [Key] annotation designates this property as the primary key.
        //As such, it is automatically assigned & incremented (assuming
        //that is how it is set up in the Database).
        public int CategoryID { get; set; }

        [Required]//This field in the table does not allow nulls
        [Display(Name = "Category")]
        [StringLength(15, ErrorMessage = "*Must be 15 characters or less")]//Length restriction
        public string CategoryName { get; set; } = null!;

        [DisplayFormat(NullDisplayText = "[N/A]")]//will display [N/A] if description is null
        public string? Description { get; set; }

        //We don't intend to use this property in our app, so no attributes are needed
        public object? Picture { get; set; }
    }
}
