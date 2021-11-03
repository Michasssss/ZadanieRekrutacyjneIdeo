using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ZadanieRekrutacyjneIdeo.Models
{
    /// <summary>
    /// Class representing the node in tree
    /// </summary>
    public class TreeNode
    {
        [Key]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Name")]
        [StringLength(30)]
        [RegularExpression(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ\0-9\s.]*$", ErrorMessage = "Characters are not allowed.")]
        public string Name { get; set; }

        [Display(Name = "Parent")]
        public int? PID { get; set; }

        [ForeignKey("PID")]
        public TreeNode Parent { get; set; }

        public ICollection<TreeNode> Childs { get; set; }
    }
}
