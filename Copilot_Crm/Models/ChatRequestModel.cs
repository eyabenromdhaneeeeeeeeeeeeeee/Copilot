using Copilot_Crm.Models;

namespace Copilot_Crm.Models
{
    public class ChatRequestModel
    {
        public string Model { get; set; }
        public List<ChatMessageModel> Messages { get; set; }
          
    }
}
