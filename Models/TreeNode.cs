using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ZadanieRekrutacyjneIdeo.Models
{
    public class TreeNode
    {
        [Key]
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "ParentID")]
        public int? PID { get; set; }

        [ForeignKey("PID")]
        public TreeNode Parent { get; set; }

        public ICollection<TreeNode> Childs { get; set; }
    }
}
