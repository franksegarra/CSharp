using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAPowerMVCApp.Models
{
    public sealed class UserAccount
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Address is Required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Zip is Required")]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid Zip")]
        public string Postal { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
    }
}
