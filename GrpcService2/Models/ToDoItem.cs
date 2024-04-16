namespace GrpcService1.Models
{
    public class ToDoItem
    {
        public int Id { get; set; }
        public string? Title { get; set; }

        public string? Description { get; set; }
        public string? status { get; set; } = "NEW";
    }
}