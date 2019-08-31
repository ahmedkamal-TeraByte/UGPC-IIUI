namespace UGPC_IIUI.Models
{
    public class Marking
    {
        public int MarkingId { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public float PresentationMarks { get; set; }
        public float SupervisorMarks { get; set; }
        public float InternalMarks { get; set; }
        public float ExternalMarks { get; set; }

    }
}
