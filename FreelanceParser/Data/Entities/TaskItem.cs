using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelanceParser.Core
{
    public class TaskItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Price { get; set; }
        public string CountBeats { get; set; }
        public string Date { get; set; }
        public string Uri { get; set; }
    }
}