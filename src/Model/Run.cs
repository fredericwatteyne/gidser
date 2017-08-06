using System;
using System.ComponentModel.DataAnnotations;

namespace Gidser.Model
{
    public class Run
    {
        public Run()
		{
		}

		public int RunId { get; set; }
		public string Name { get; set; }
		public string City { get; set; }

		[Display(Name = "Event Date")]
		[DataType(DataType.Date)]
		public DateTime EventTime { get; set; }
		public Decimal Distance { get; set; }

        [EnumDataType(typeof(RunTypeEnum))]
		public RunTypeEnum? RunType { get; set; }
		public string Url { get; set; }

        public bool IsRace { get; set; }
    }
}
