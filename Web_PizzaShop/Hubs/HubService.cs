using Microsoft.AspNetCore.SignalR;

namespace Web_PizzaShop.Hubs
{
    public class HubService : Hub
    {
        public void ReloadData()
        {
            Clients.All.SendAsync("ReloadData");
        }
    }
}
