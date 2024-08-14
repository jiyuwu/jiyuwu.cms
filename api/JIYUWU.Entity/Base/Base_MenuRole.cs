using System.ComponentModel.DataAnnotations;

namespace JIYUWU.Entity.Base
{
    public class Base_Actions
    {
        [Key]
        public int Action_Id { get; set; }
        public int Menu_Id { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
    }
}
