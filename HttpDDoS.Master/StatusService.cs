using System;
using System.IO;
using System.Text.Json;
using HttpDDoS.Shared;

namespace HttpDDoS.Master
{
    public class StatusService : IStatusService
    {
        const string StatusBackupFileName = "status.json";

        public Status Status { get; private set; }

        public StatusService()
        {
            try
            {
                Status = JsonSerializer.Deserialize<Status>(File.ReadAllText(StatusBackupFileName));
            }
            catch
            {
                Status = new();
            }
        }

        public void SetStatus(Status newStatus)
        {
            Status = newStatus;
            File.WriteAllText(StatusBackupFileName, JsonSerializer.Serialize(newStatus));
        }
    }
}
