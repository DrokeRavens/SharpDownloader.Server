using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace SharpDownloader.API.Wss
{
    public class ConnectionManager
    {
        private List<Client> _clients = new List<Client>();

        public Client GetClientById(string id)
        {
            return _clients.FirstOrDefault(p => p.Id == id);
        }

        public Client GetIdBySocket(WebSocket socket)
        {
            return _clients.FirstOrDefault(p => p.Socket == socket);
        }

        public List<Client> GetAll()
        {
            return _clients;
        }

        public void AddClient(WebSocket socket)
        {
            var client = new Client{
                Creation = DateTime.Now,
                Socket = socket,
                Id = CreateConnectionId()
            };
            _clients.Add(client);
            Console.WriteLine("new client");
        }

        public async Task RemoveSocket(WebSocket socket)
        {
            var client = _clients.FirstOrDefault(c => c.Socket == socket);

            if(client == null)
                return;

            _clients.Remove(client);

            await client.Socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                                    statusDescription: "Closed by server.",
                                    cancellationToken: CancellationToken.None);
        }

        private string CreateConnectionId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}