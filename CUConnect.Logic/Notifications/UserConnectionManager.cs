namespace CUConnect.Logic.Notifications
{
    public class UserConnectionManager
    {
        private static UserConnectionManager _instance;
        private static readonly object _lockObject = new object();
        private Dictionary<string, List<string>> _emailConnectionMap;

        public static UserConnectionManager Instance
        {
            get
            {
                lock (_lockObject)
                {
                    if (_instance == null)
                    {
                        _instance = new UserConnectionManager();
                    }
                    return _instance;
                }
            }
        }

        private UserConnectionManager()
        {
            _emailConnectionMap = new Dictionary<string, List<string>>();
        }

        public void AddConnection(string userEmail, string connectionId)
        {
            lock (_lockObject)
            {
                if (_emailConnectionMap.ContainsKey(userEmail))
                {
                    _emailConnectionMap[userEmail].Add(connectionId);
                }
                else
                {
                    _emailConnectionMap[userEmail] = new List<string> { connectionId };
                }
            }
        }

        public void RemoveConnection(string userEmail, string connectionId)
        {
            lock (_lockObject)
            {
                if (_emailConnectionMap.ContainsKey(userEmail))
                {
                    _emailConnectionMap[userEmail].Remove(connectionId);
                    if (_emailConnectionMap[userEmail].Count == 0)
                    {
                        _emailConnectionMap.Remove(userEmail);
                    }
                }
            }
        }

        public List<string> GetConnections(string userEmail)
        {
            lock (_lockObject)
            {
                if (_emailConnectionMap.ContainsKey(userEmail))
                {
                    return _emailConnectionMap[userEmail];
                }
                return new List<string>();
            }
        }

        public void LogoutAllConnections(string userEmail)
        {
            lock (_lockObject)
            {
                if (_emailConnectionMap.ContainsKey(userEmail))
                {
                    List<string> connections = _emailConnectionMap[userEmail];
                    foreach (var connectionId in connections)
                    {
                        // Perform logout action for each connection, if needed
                    }
                    _emailConnectionMap.Remove(userEmail);
                }
            }
        }
    }
}
