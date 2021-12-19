#nullable disable

namespace LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities
{
    public partial class Binding
    {
        public int BindingId { get; set; }
        public int? PublisherInstanceId { get; set; }
        public int? ConsumerInstanceId { get; set; }
        public virtual ApplicationInstance ConsumerInstance { get; set; }
        public virtual ApplicationInstance PublisherInstance { get; set; }
    }
}
