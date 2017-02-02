using System;
using kuujinbo.DataTables.DataTable;

namespace kuujinbo.DataTables.Models
{
    public enum Status
    {
        FullTime, PartTime
    }

    public class TestModel : IIdentifiable
    {
        public int Id { get; set; }
        [Column(DisplayOrder = 1)]
        public string Name { get; set; }
        [Column(DisplayOrder = 2)]
        public string Position { get; set; }
        [Column(DisplayOrder = 3)]
        public string Office { get; set; }
        [Column(DisplayOrder = 4)]
        public int Extension { get; set; }
        [Column(DisplayOrder = 5, DisplayName = "Start Date")]
        public DateTime? StartDate { get; set; }
        [Column(DisplayOrder = 6)]
        public string Salary { get; set; }
        [Column(DisplayOrder = 7)]
        public bool? Salaried { get; set; }
        [Column(DisplayOrder = 7)]
        public Status Status { get; set; }
    }
}