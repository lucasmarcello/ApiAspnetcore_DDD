using System.Text;

namespace ConsumirAPIRefit.Classes.Users
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CreateAt { get; set; }
        public string UpdateAt { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Id = {0}", Id));
            sb.AppendLine(string.Format("Name = {0}", Name));
            sb.AppendLine(string.Format("Email = {0}", Email));
            sb.AppendLine(string.Format("CreateAt = {0}", CreateAt));
            sb.AppendLine(string.Format("UpdateAt = {0}", UpdateAt));
            return sb.ToString();
        }
    }
}
